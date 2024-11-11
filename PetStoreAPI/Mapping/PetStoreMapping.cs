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

            // Khóa chính
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PetName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Gender)
                .IsRequired()
                .HasMaxLength(10); // Giới tính có thể là "Male" hoặc "Female", với độ dài tối đa là 10 ký tự

            builder.Property(p => p.PetDescription)
                .IsRequired()
                .HasMaxLength(500); // Mô tả thú cưng, tối đa 500 ký tự

            builder.Property(p => p.Price)
                .HasPrecision(18, 2); // Đảm bảo kiểu tiền tệ có 18 chữ số, trong đó có 2 chữ số thập phân

            builder.Property(p => p.BirthDay)
                .IsRequired(); // Ngày sinh là bắt buộc, có thể dùng DateTime hoặc DateOnly tùy thuộc vào yêu cầu

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
