using FormCMS.Core.Descriptors;
using FormCMS.Infrastructure.RelationDbDao;
using FormCMS.Utils.ResultExt;

namespace FormCMS.Cms.Workers;

public record DataPublishingWorkerOptions(int DelaySeconds);
public sealed class DataPublishingWorker(
    ILogger<DataPublishingWorker> logger,
    KateQueryExecutor queryExecutor,
    DataPublishingWorkerOptions options
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var items = await queryExecutor.Many(SchemaHelper.ByNameAndType(SchemaType.Entity, null),ct);
                var count = 0;
                foreach (var item in items)
                {
                    if (!SchemaHelper.RecordToSchema(item).Try(out var entity, out var error))
                    {
                        logger.LogError(
                            "Fail to Parse entity, error={err}",
                            string.Join(",", error!.Select(x => x.Message))
                        );
                    }
                    else
                    {
                        try
                        {
                            count += await queryExecutor.ExecAffected(entity.Settings.Entity!.PublishAllScheduled(), ct);
                        }
                        catch (Exception e)
                        {
                            logger.LogError("Fail to publish {entity}, error = {error}",
                                entity.Name,
                                e.Message);
                        }
                    }
                }
                logger.LogInformation($"{count} records published");
            }
            catch (Exception e)
            {
                logger.LogError("Fail to publish, error = {error}", e.Message);
            }
            Thread.Sleep(TimeSpan.FromSeconds(options.DelaySeconds));
        }
    }
}