using System.Security.Claims;

namespace WebApp.Extensions;

public static class UserExtensions
{
    public static string GetId(this ClaimsPrincipal principal)
    {
        if(principal is null)
        {
            return string.Empty;
        }

        return principal.Claims.Single(c => c.Type == "id").Value;
    }
}
