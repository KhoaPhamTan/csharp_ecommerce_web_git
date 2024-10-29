using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Entities;
using PetStoreAPI.Mappings;

namespace PetStoreAPI.Data
{
    public class PetStoreDbContext : DbContext
    {
        public PetStoreDbContext(DbContextOptions<PetStoreDbContext> options) : base(options) { }

        public DbSet<PetStoreEntity> PetStores { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PetTypeEntity> PetTypes { get; set; } // DbSet for PetTypeEntity

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply the PetStoreMapping configuration
            modelBuilder.ApplyConfiguration(new PetStoreMapping());

            // Set precision and scale for PetTypeEntity and other configurations as needed
            modelBuilder.Entity<PetStoreEntity>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Seed default PetType records
            modelBuilder.Entity<PetTypeEntity>().HasData(
                new PetTypeEntity { Id = 1, Name = "Cat" },
                new PetTypeEntity { Id = 2, Name = "Dog" },
                new PetTypeEntity { Id = 3, Name = "Fish" },
                new PetTypeEntity { Id = 4, Name = "Insert" },
                new PetTypeEntity { Id = 5, Name = "Others" }
            );
        }
    }
}
