using Microsoft.AspNetCore.Identity;

namespace WebApp.Domain;

public sealed class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public required ApplicationUser User { get; set; }
}