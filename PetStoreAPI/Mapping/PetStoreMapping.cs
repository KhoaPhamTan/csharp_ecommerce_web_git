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

            builder.Property(p => p.ItemId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.ProductId)
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

            // Configure the relationship with PetTypeEntity if necessary
            builder.HasOne(p => p.PetType)
                   .WithMany() // Adjust according to your model
                   .HasForeignKey(p => p.PetTypeId);
        }
    }
}
