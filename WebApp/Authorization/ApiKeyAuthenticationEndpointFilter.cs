using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
internal sealed class ApiKeyAuthenticationEndpointFilter : Attribute, IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string ApiKeySectionName = "Authentication:ApiKey";

    public void OnAuthorization(AuthorizationFilterContext context)
    {

        if (!IsApiKeyValid(context.HttpContext))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private static bool IsApiKeyValid(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey)) {
            return false;
        }

        string actualApiKey = context.RequestServices.GetRequiredService<IConfiguration>().GetValue<string>(ApiKeySectionName)!;

        return apiKey.Equals(actualApiKey);
    }
}