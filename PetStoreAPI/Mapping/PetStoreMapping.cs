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

            // Các thuộc tính ItemId, ProductId, Gender, PetDescription, Price, và BirthDay
            builder.Property(p => p.ItemId)
                .IsRequired() // Bắt buộc
                .HasMaxLength(50); // Độ dài tối đa

            builder.Property(p => p.ProductId)
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

            // Cấu hình quan hệ giữa PetStoreEntity và PetTypeEntity
            builder.HasOne(p => p.PetType) // Mỗi PetStore có một PetType
                .WithMany() // Mỗi PetType có thể có nhiều PetStore
                .HasForeignKey(p => p.PetTypeId) // PetTypeId là khóa ngoại
                .OnDelete(DeleteBehavior.Restrict); // Nếu muốn giữ các PetStore mặc dù PetType bị xóa
        }
    }
}
