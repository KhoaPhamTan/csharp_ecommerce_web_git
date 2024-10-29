using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.EndPoints;
using PetStoreAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext to use SQLite
builder.Services.AddDbContext<PetStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetStore")));

var app = builder.Build();

// Automatically run migrations
await app.MigrateDbAsync();

// Define endpoints
app.MapStoresEndPoints();
app.MapCartEndpoints();
app.MapRegistrationEndpoints();

// Run the application
app.Run();
