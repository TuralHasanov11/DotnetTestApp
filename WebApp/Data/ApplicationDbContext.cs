using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain;

namespace WebApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("application");
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly, WriteConfigurationFilter);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql($"Host=localhost;Username=postgres;Password=hidraC137;Database=dotnet_web_app",
            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
    }

    private static bool WriteConfigurationFilter(Type type)
    {
        return type.FullName?.Contains("Domain.Configuration.Write") ?? false;
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }
}