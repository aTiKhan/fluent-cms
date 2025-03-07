using FormCMS.Core.Descriptors;

namespace FormCMS.Auth.Services;

public interface ISchemaPermissionService
{
    string[] GetAll();
    void GetOne(Schema schema);
    Task Delete(int id);
    Task<Schema> BeforeSave(Schema schema);
    Task AfterSave(Schema schema);
}