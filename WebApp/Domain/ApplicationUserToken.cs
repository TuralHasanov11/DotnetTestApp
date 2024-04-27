using Microsoft.AspNetCore.Identity;

namespace WebApp.Domain;

public sealed class ApplicationUserToken : IdentityUserToken<Guid>
{
    public required ApplicationUser User { get; set; }
}