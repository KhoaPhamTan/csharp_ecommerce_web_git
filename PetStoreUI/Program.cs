using PetStoreAPI.Data; // Ensure namespace for PetStoreDbContext
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetStoreUI.Services;
using Microsoft.EntityFrameworkCore; // Add to use EF Core
using PetStoreLibrary.DTOs; // Add to use DTOs from PetStoreLibrary
using Microsoft.AspNetCore.Builder; // Thêm chỉ thị này
using Microsoft.Extensions.Configuration; // Thêm chỉ thị này

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // Register HttpClient
builder.Services.AddDbContext<PetStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetStore"))); // Use defined connection string

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Ensure static files are served

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{categoryName?}/{id?}");

app.Run();