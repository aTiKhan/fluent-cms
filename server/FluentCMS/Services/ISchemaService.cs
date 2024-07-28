using FluentCMS.Models;
using FluentResults;
using FluentCMS.Utils.QueryBuilder;

namespace FluentCMS.Services;

public interface ISchemaService
{
    Task<Schema[]> GetAll(string type);
    Task<Result<Entity>> GetEntityByNameOrDefault(string name);
    Task<Schema> GetByIdOrName(string name, bool extend);
    Task<Schema?> GetByIdOrNameDefault(string name);
    Task<View> GetViewByName(string name);
    Task<Entity?> GetTableDefine(string tableName);
    Task<Schema> SaveTableDefine(Schema schemaDto);
    Task<Schema> Save(Schema schema);
    Task AddTopMenuBar();
    Task AddSchemaTable();
    Task<bool> Delete(int id);
}