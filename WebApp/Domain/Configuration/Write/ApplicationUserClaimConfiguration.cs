using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp.Domain.Configuration.Write;

internal sealed class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
    {
        builder.ToTable("ApplicationUserClaims");

        builder.HasKey(u => u.Id);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Claims)
            .HasForeignKey(u => u.UserId)
            .IsRequired();
    }
}
