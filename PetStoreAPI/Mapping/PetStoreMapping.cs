using PetStoreAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PetStoreAPI.Mappings
{
    public class PetStoreMapping : IEntityTypeConfiguration<PetStoreEntity>
    {
        public void Configure(EntityTypeBuilder<PetStoreEntity> builder)
        {
            builder.ToTable("PetStores");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PetName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Gender)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(p => p.PetDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Property(p => p.BirthDay)
                .IsRequired(); // Assuming BirthDay is a required field

            builder.Property(p => p.ImageUrl)
                .IsRequired()
                .HasMaxLength(200);

            // Configure the relationship with CategoryEntity
            builder.HasOne(p => p.Category)
                   .WithMany()
                   .HasForeignKey("CategoryId");
        }
    }
}
