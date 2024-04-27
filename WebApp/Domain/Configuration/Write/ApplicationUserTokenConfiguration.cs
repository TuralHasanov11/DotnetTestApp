using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp.Domain.Configuration.Write;

internal sealed class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
{
    public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
    {
        builder.ToTable("ApplicationUserTokens");

        builder.Property(t => t.LoginProvider).HasMaxLength(255);
        builder.Property(t => t.Name).HasMaxLength(255);
    }
}
