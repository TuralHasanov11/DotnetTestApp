using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp.Domain.Configuration.Write;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(x => x.Id);

        builder.HasMany(e => e.Roles)
            .WithMany(p => p.Permissions)
            .UsingEntity<ApplicationRolePermission>(
                l => l.HasOne(e => e.Role).WithMany(e => e.RolePermissions).HasForeignKey(e => e.RoleId),
                r => r.HasOne(e => e.Permission).WithMany(e => e.RolePermissions).HasForeignKey(e => e.PermissionId));

        builder.HasData(
            Enum.GetValues<Permissions>()
                .Where(p => p is not Permissions.Null)
                .Select(p => new Permission((int)p, p.ToString())));
    }
}