using Microsoft.AspNetCore.Identity;

namespace WebApp.Domain;

public sealed class ApplicationUserRole : IdentityUserRole<Guid>
{
    public required ApplicationUser User { get; set; }
    public required ApplicationRole Role { get; set; }
}