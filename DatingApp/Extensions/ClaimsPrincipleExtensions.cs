using System.Security.Claims;

namespace DatingApp.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        // it represents UniqueName in tokenService
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        // it represents UniqueName in tokenService
        return new Guid(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }
}