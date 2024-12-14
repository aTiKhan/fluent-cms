using FluentCMS.Auth.models;
using FluentCMS.Exceptions;

namespace FluentCMS.Auth.Services;

public class DummyProfileService: IProfileService
{
    public UserDto GetInfo()
    {
        return new UserDto
        (
            Id:"",
            Email : "sadmin@cms.com",
            Roles : [RoleConstants.Sa],
            AllowedMenus : [UserConstants.MenuSchemaBuilder],
            ReadonlyEntities:[],
            RestrictedReadonlyEntities:[],
            ReadWriteEntities:[],
            RestrictedReadWriteEntities:[]
        );
    }
    
    public Task ChangePassword(ProfileDto dto)
    {
        throw new ServiceException("Not implemented yet");
    } 
}