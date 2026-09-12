using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNUL.Shared.Domain.Models;

namespace SNUL.Shared.Persistance.Configurations
{
    public class WelcoProviderMapConfiguration : IEntityTypeConfiguration<WelcoProviderMap>
    {
        public void Configure(EntityTypeBuilder<WelcoProviderMap> builder)
        {
            builder.ToTable("WelcoProviderMaps");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.WelcoProviderId).IsRequired();
            builder.Property(x => x.System).IsRequired().HasMaxLength(32);
            builder.HasIndex(x => new { x.WelcoProviderId, x.System }).IsUnique();
            builder.HasIndex(x => x.CompanyId);
            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.SetNull);
            builder.Property(x => x.CreatedBy).IsRequired();
        }
    }
}
