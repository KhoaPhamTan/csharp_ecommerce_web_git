using PetStoreAPI.Data; // Ensure namespace for PetStoreDbContext
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetStoreUI.Services;
using Microsoft.EntityFrameworkCore; // Add to use EF Core
using PetStoreLibrary.DTOs; // Add to use DTOs from PetStoreLibrary
using Microsoft.AspNetCore.Builder; // Thêm chỉ thị này
using Microsoft.Extensions.Configuration; // Thêm chỉ thị này

var builder = WebApplication.CreateBuilder(args);

// Configure the connection string and register DbContext
builder.Services.AddDbContext<PetStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetStore"))); // Use "PetStore" instead of "DefaultConnection"

// Register necessary services into the DI container
builder.Services.AddScoped<IPetService, PetService>();  // Register PetService for IPetService

// Register MVC services (Controllers and Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure middleware
app.UseStaticFiles(); // Ensure serving static files like js, css, images
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();