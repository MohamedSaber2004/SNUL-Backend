using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNUL.Shared.Domain.Models;

namespace SNUL.Shared.Persistance.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameEn)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Code)
                .HasMaxLength(10);

            builder.Property(x => x.PhoneCode)
                .HasMaxLength(15);

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.HasMany(x => x.Cities)
                .WithOne(c => c.Country)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
