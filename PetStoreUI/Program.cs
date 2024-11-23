using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Endpoints;
using PetStoreAPI.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure DbContext with SQLite
builder.Services.AddDbContext<PetStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetStore")));

// Add authentication and authorization services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";               // Đường dẫn trang đăng nhập
        options.LogoutPath = "/Account/Logout";             // Đường dẫn trang đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn trang từ chối quyền truy cập

        // Ensure cookies are sent with cross-origin requests
        options.Cookie.SameSite = SameSiteMode.None;  // Configure cookie for cross-origin
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Use cookie only with HTTPS
    });

builder.Services.AddAuthorization(); // Add authorization services

// Add CORS policy to allow access from the UI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:5135")  // Ensure this matches the UI origin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Allow sending cookies
        });
});

// Add Razor Pages services
builder.Services.AddRazorPages();

// Register HttpClient service (để gọi API từ UI)
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5134"); // Ensure this matches the API backend address
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();  // Bật HSTS trong production để sử dụng HTTPS
}

app.UseStaticFiles(); // Đảm bảo phục vụ file tĩnh

app.UseRouting();

// Use CORS policy
app.UseCors("AllowAll"); // Ensure CORS is used before authentication and authorization

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization(); // Ensure authorization middleware is used

// Map Razor Pages (để xử lý trang Razor Pages)
app.MapRazorPages();

// Map API endpoints (giả sử bạn có những endpoint như MapStoresEndPoints, MapUserEndpoints, v.v.)
app.MapStoresEndPoints();
app.MapCartEndpoints();
app.MapRegistrationEndpoints();
app.MapPetEndpoints();
app.MapCategoryEndpoints(); // Đảm bảo ánh xạ endpoint cho Category
app.MapUserEndpoints(); // Ánh xạ các endpoint cho người dùng (đảm bảo chỉ ánh xạ một lần)

// Run the application
app.Run();
