using Microsoft.AspNetCore.Authorization;
using WebApp.Domain;

namespace WebApp.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permissions permission) : base(policy: nameof(permission))
    {

    }
}