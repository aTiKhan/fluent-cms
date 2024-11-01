using System.Collections.Immutable;
using FluentCMS.Cms.Models;
using FluentCMS.Utils.QueryBuilder;
using FluentResults;

namespace FluentCMS.Cms.Services;

public interface IEntitySchemaService
{
    
    Task<Result<LoadedEntity>> GetLoadedEntity(string name, CancellationToken cancellationToken = default);
    Task<Entity?> GetTableDefine(string tableName, CancellationToken cancellationToken);
    Task<Schema> SaveTableDefine(Schema schemaDto, CancellationToken cancellationToken);
    Task<Schema> AddOrUpdate(Entity entity, CancellationToken cancellationToken);
    Task<Result<LoadedAttribute>> LoadOneRelated(LoadedEntity entity, LoadedAttribute attribute, CancellationToken cancellationToken);
    Task<Result<AttributeVector>> ResolveAttributeVector(LoadedEntity entity, string fieldName);
    Task<LoadedAttribute?> FindAttribute(string entityName, string attributeName, CancellationToken cancellationToken);
}