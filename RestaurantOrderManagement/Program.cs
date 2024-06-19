using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantOrderManagement.DB;
using RestaurantOrderManagement.Services;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpClient("MenuService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5055"); 
});
builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5067"); 
});

var supportedCultures = new[] { new CultureInfo("en-US") };
var localizationOptions = new RequestLocalizationOptions
{
	DefaultRequestCulture = new RequestCulture("en-US"),
	SupportedCultures = supportedCultures,
	SupportedUICultures = supportedCultures
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	options.DefaultRequestCulture = new RequestCulture("en-US");
	options.SupportedCultures = supportedCultures;
	options.SupportedUICultures = supportedCultures;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer",
            ValidAudience = "yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey"))
        };
    });

var app = builder.Build();
app.UseRequestLocalization(localizationOptions);
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Маршрут по умолчанию

app.MapControllerRoute(
    name: "users",
    pattern: "Users/{action=Index}/{id?}"); // Дополнительный маршрут для Users

app.Run();
