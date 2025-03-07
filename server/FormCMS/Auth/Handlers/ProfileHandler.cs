using FormCMS.Auth.Services;
using FormCMS.Utils.ResultExt;

namespace FormCMS.Auth.Handlers;

public static class ProfileHandler
{
    public static void MapProfileHandlers(this RouteGroupBuilder app)
    {
        app.MapPost("/password", async (
            IProfileService svc, ProfileDto dto
        ) => await svc.ChangePassword(dto));

        app.MapGet("/info", (
            IProfileService svc
        ) => svc.GetInfo() ?? throw new ResultException("Unauthorized"));
    }
}