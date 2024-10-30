var builder = WebApplication.CreateBuilder(args);

// Load the configuration from appsettings.json
var configuration = builder.Configuration;
var petStoreApiUrl = configuration["ApiSettings:PetStoreApiUrl"];
if (string.IsNullOrEmpty(petStoreApiUrl))
{
    throw new InvalidOperationException("The PetStoreApiUrl configuration is missing or empty.");
}

// Add HttpClient with the base address from configuration
builder.Services.AddHttpClient("PetStoreAPI", client =>
{
    client.BaseAddress = new Uri(petStoreApiUrl);
});

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Cấu hình chuyển hướng từ / sang /Home/Index
app.MapGet("/", context =>
{
    context.Response.Redirect("/Home/Index");
    return Task.CompletedTask;
});

// Map Razor Pages//update
app.MapRazorPages();

app.Run();
