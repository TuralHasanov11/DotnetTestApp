using Microsoft.AspNetCore.Identity;

namespace WebApp.Domain;

public sealed class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public required ApplicationRole Role { get; set; }
}