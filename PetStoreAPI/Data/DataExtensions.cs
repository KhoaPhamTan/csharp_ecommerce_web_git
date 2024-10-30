using Microsoft.EntityFrameworkCore;

namespace PetStoreAPI.Data
{
    public static class DataExtensions
    {
        public static async Task MigrateDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PetStoreDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
