using NATS.Client.Core;

namespace FluentCMS.Utils.EventStreaming;

public class NatsProducer(ILogger<NatsProducer> logger,INatsConnection connection):IStringMessageProducer
{
    public async Task Produce(string topic, string msg)
    {
        await connection.PublishAsync(topic, msg);
    }
}