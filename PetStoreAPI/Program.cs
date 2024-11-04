using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Endpoints;
using PetStoreAPI.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// Thêm cấu hình DbContext với SQLite
builder.Services.AddDbContext<PetStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetStore")));

// Thêm CORS để cho phép truy cập từ UI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin() // Cho phép tất cả các nguồn
                   .AllowAnyMethod() // Cho phép tất cả các phương thức (GET, POST, v.v.)
                   .AllowAnyHeader(); // Cho phép tất cả các header
        });
});

// Thêm các dịch vụ khác nếu cần
// Ví dụ: builder.Services.AddScoped<IMyService, MyService>();

var app = builder.Build();

// Chạy migrations tự động (nếu cần)
await app.MigrateDbAsync(); // Phương thức tùy chỉnh để di chuyển cơ sở dữ liệu

// Sử dụng CORS
app.UseCors("AllowAll");

// Ánh xạ các endpoints
app.MapStoresEndPoints();
app.MapCartEndpoints();
app.MapRegistrationEndpoints();
app.MapPetTypeEndpoints(); // Ánh xạ các endpoint cho PetType
app.MapPetEndpoints();

// Chạy ứng dụng
app.Run();
