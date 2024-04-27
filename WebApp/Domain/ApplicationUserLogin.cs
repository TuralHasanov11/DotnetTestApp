using Microsoft.AspNetCore.Identity;

namespace WebApp.Domain;

public sealed class ApplicationUserLogin : IdentityUserLogin<Guid>
{
    public required ApplicationUser User { get; set; }
}