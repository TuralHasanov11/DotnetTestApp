using Microsoft.AspNetCore.Identity;

namespace WebApp.Domain;

public enum ApplicationRoles
{
   Guest = 0,
   Administrator = 1,
   Instructor = 2,
}

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole(string name) : base(name) {
        Id = Guid.NewGuid();
    }

    public ICollection<ApplicationUserRole>? UserRoles { get; }
    public ICollection<ApplicationRoleClaim>? RoleClaims { get; }
    public ICollection<ApplicationRolePermission> RolePermissions { get; } = [];
    public ICollection<Permission> Permissions { get; } = [];
}