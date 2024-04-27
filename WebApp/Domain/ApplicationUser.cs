using Microsoft.AspNetCore.Identity;

namespace WebApp.Domain;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<ApplicationUserClaim> Claims { get; set; }
    public ICollection<ApplicationUserLogin> Logins { get; set; }
    public ICollection<ApplicationUserToken> Tokens { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
}