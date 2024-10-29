using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load the configuration from appsettings.json
var configuration = builder.Configuration;

// Add HttpClient with the base address from configuration
builder.Services.AddHttpClient("PetStoreAPI", client =>
{
    var petStoreApiUrl = configuration["ApiSettings:PetStoreApiUrl"];
    client.BaseAddress = new Uri(petStoreApiUrl);
});

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();
app.Run();
