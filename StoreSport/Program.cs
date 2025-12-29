using Microsoft.EntityFrameworkCore;
using StoreSport.Models;

var builder = WebApplication.CreateBuilder(args);

// Add MVC and Razor Pages services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure DbContext
builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    opts.UseSqlServer(
        builder.Configuration["ConnectionStrings:StoreSportConnection"]);
});

// Register repositories
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

// Configure cart
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseSession();

// MapDefaultControllerRoute must be before custom routes
app.MapDefaultControllerRoute();

app.MapControllerRoute(
    name: "catpage",
    pattern: "{category}/Page{productPage:int}",
 defaults: new { Controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "page",
    pattern: "Page{productPage:int}",
    defaults: new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute(
    name: "category",
    pattern: "{category}",
    defaults: new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute(
  name: "pagination",
    pattern: "Products/Page{productPage}",
    defaults: new { Controller = "Home", action = "Index" });

app.MapRazorPages();

// Initialize database
SeedData.EnsurePopulated(app);

app.Run();

// Make Program class accessible for tests
public partial class Program { }
