using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Entities;
using PetStoreAPI.Mappings;

namespace PetStoreAPI.Data
{
    public class PetStoreDbContext : DbContext
    {
        public PetStoreDbContext(DbContextOptions<PetStoreDbContext> options) : base(options) { }

        public DbSet<PetStoreEntity>? PetStores { get; set; }
        public DbSet<CartItemEntity>? CartItems { get; set; }
        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<CategoryEntity>? Categories { get; set; }
        // ...existing code...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply the PetStoreMapping configuration
            modelBuilder.ApplyConfiguration(new PetStoreMapping());

            // Set precision and scale for PetType and other configurations as needed
            modelBuilder.Entity<PetStoreEntity>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Seed default Category records
            modelBuilder.Entity<CategoryEntity>().HasData(
                new CategoryEntity { Id = 1, Name = "Cat" },
                new CategoryEntity { Id = 2, Name = "Dog" },
                new CategoryEntity { Id = 3, Name = "Fish" },
                new CategoryEntity { Id = 4, Name = "Insect" },
                new CategoryEntity { Id = 5, Name = "Bird" },
                new CategoryEntity { Id = 6, Name = "Others" }
            );

            // Configure entity properties and relationships
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired(); // Ensure Id is required
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Address).IsRequired();
                entity.Property(e => e.Role).IsRequired();
            });

            // Configure CartItemEntity
            modelBuilder.Entity<CartItemEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PetId).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.DateAdded).IsRequired();
                entity.Property(e => e.UserId).IsRequired(); // Ensure UserId matches the type of the primary key in UserEntity

                // Configure relationship with PetStoreEntity
                entity.HasOne(e => e.Pet)
                      .WithMany()  // If Pet can have many CartItems
                      .HasForeignKey(e => e.PetId)
                      .OnDelete(DeleteBehavior.Cascade); // Add cascade delete behavior if needed

                // Configure relationship with UserEntity
                entity.HasOne(e => e.User)
                      .WithMany()  // If User can have many CartItems
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Add cascade delete behavior if needed
            });

            // Configure other entities...
        }
    }
}
