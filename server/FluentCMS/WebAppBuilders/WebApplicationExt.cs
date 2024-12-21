using FluentCMS.Types;
using FluentCMS.Utils.HookFactory;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FluentCMS.WebAppBuilders;

public static class WebApplicationExt
{
    public static async Task UseCmsAsync(this WebApplication app)
    {
        await app.Services.GetRequiredService<CmsBuilder>().UseCmsAsync(app);
        app.Services.GetService<IAuthBuilder>()?.UseCmsAuth(app);
        app.Services.GetService<MongoQueryBuilder>()?.UserMongoDbQuery(app);
        app.Services.GetService<EventProduceBuilder>()?.UseEventProducer(app);
    }

    public static HookRegistry GetHookRegistry(this WebApplication app) =>
        app.Services.GetRequiredService<HookRegistry>();

    public static async Task<Result> EnsureCmsUser(
        this WebApplication app, string email, string password, string[] role
    ) => await app.Services.GetRequiredService<IAuthBuilder>().EnsureCmsUser(app, email, password, role);

    public static IServiceCollection AddMongoDbQuery(
        this IServiceCollection services, IEnumerable<QueryLinks> queryCollectionLinks
        )=>MongoQueryBuilder.AddMongoDbQuery(services, queryCollectionLinks);
    
    public static IServiceCollection AddPostgresCms(
        this IServiceCollection services, string connectionString, Action<CmsOptions>? action = null
        ) => CmsBuilder.AddCms(services, DatabaseProvider.Postgres, connectionString,action);

    public static IServiceCollection AddSqliteCms(
        this IServiceCollection services, string connectionString, Action<CmsOptions>? action = null
    ) => CmsBuilder.AddCms(services, DatabaseProvider.Sqlite, connectionString, action);

    public static IServiceCollection AddSqlServerCms(
        this IServiceCollection services, string connectionString, Action<CmsOptions>? action = null
    ) => CmsBuilder.AddCms(services, DatabaseProvider.SqlServer, connectionString, action);

    public static IServiceCollection AddCmsAuth<TUser, TRole, TContext>(this IServiceCollection services)
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
        where TContext : IdentityDbContext<TUser>
        => AuthBuilder<TUser>.AddCmsAuth<TUser, TRole, TContext>(services);

    public static IServiceCollection AddKafkaMessageProducer(
        this IServiceCollection services, string[] entities
    ) => EventProduceBuilder.AddKafkaMessageProducer(services, entities);

    public static IServiceCollection AddNatsMessageProducer(
        this IServiceCollection services,string[] entities
    ) => EventProduceBuilder.AddNatsMessageProducer(services,entities);
}