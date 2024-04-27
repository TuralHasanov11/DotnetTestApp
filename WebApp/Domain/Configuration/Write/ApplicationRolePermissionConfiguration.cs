using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain;

namespace MainApplication.Persistence.Configurations.Write;

public class ApplicationRolePermissionConfiguration : IEntityTypeConfiguration<ApplicationRolePermission>
{
    public void Configure(EntityTypeBuilder<ApplicationRolePermission> builder)
    {
        builder?.HasKey(e => new { e.RoleId, e.PermissionId });
    }

    private static ApplicationRolePermission Create(ApplicationRole role, Permissions permission)
    {
        return new ApplicationRolePermission
        {
            RoleId = role.Id,
            PermissionId = (int)permission
        };
    }
}