using System.Security.Claims;

namespace DatingApp.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}