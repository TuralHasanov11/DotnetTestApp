using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;

namespace WebApp.Domain.Configuration.Write;
internal sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("ApplicationRoles");

        builder.HasKey(u => u.Id);

        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

        // Limit the size of columns to use efficient database types
        builder.Property(u => u.Name).HasMaxLength(256);
        builder.Property(u => u.NormalizedName).HasMaxLength(256);

        builder.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

        builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.Role)
            .HasForeignKey(e => e.RoleId)
            .IsRequired();

        builder.HasMany(e => e.RoleClaims)
            .WithOne(e => e.Role)
            .HasForeignKey(e => e.RoleId)
            .IsRequired();

        builder.HasMany(e => e.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity<ApplicationRolePermission>(
                l => l.HasOne(e => e.Permission).WithMany(e => e.RolePermissions).HasForeignKey(e => e.PermissionId),
                r => r.HasOne(e => e.Role).WithMany(e => e.RolePermissions).HasForeignKey(e => e.RoleId));

        //builder.HasData(
        //    [
        //        new ApplicationRole(nameof(ApplicationRoles.Administrator)),
        //        new ApplicationRole(nameof(ApplicationRoles.Instructor))
        //    ]);
    }
}
