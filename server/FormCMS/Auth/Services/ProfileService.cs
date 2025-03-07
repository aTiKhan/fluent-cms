using System.Security.Claims;
using FormCMS.Auth.DTO;
using FormCMS.Utils.ResultExt;
using Microsoft.AspNetCore.Identity;

namespace FormCMS.Auth.Services;

public sealed record ProfileDto(string OldPassword, string Password);


public class ProfileService<TUser>(
    UserManager<TUser> userManager,
    IHttpContextAccessor contextAccessor,
    SystemResources systemResources
    ):IProfileService
where TUser :IdentityUser, new()
{
    public UserDto? GetInfo()
    {
        
        var claimsPrincipal = contextAccessor.HttpContext?.User;
        if (claimsPrincipal?.Identity?.IsAuthenticated != true) return null;

        string[] roles = [..claimsPrincipal.FindAll(ClaimTypes.Role).Select(x => x.Value)];

        return new UserDto
        (
            Id: claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            Name: claimsPrincipal.Identity.Name??"",
            Email: claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? "",
            Roles: roles,
            ReadWriteEntities: [..claimsPrincipal.FindAll(AccessScope.FullAccess).Select(x => x.Value)],
            RestrictedReadWriteEntities: [..claimsPrincipal.FindAll(AccessScope.RestrictedAccess).Select(x => x.Value)],
            ReadonlyEntities: [..claimsPrincipal.FindAll(AccessScope.FullRead).Select(x => x.Value)],
            RestrictedReadonlyEntities: [..claimsPrincipal.FindAll(AccessScope.RestrictedRead).Select(x => x.Value)],
            AllowedMenus: roles.Contains(RoleConstants.Sa) || roles.Contains(RoleConstants.Admin)
                ? systemResources.Menus.ToArray()
                : []
        );
    }
    
    public async Task ChangePassword(ProfileDto dto)
    {
        var user = await MustGetCurrentUser();
        var result =await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.Password);
        if (!result.Succeeded) throw new ResultException(IdentityErrMsg(result));
    }

    private async Task<TUser> MustGetCurrentUser()
    {
        var claims = contextAccessor.HttpContext?.User;
        if (claims?.Identity?.IsAuthenticated != true) throw new ResultException("Not logged in");
        var user =await userManager.GetUserAsync(claims);
        return user?? throw new ResultException("Not logged in");
    }
    private static string IdentityErrMsg(IdentityResult result
    ) =>  string.Join("\r\n", result.Errors.Select(e => e.Description));

}