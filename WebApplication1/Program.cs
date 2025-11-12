using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// ? Configure PostgreSQL for Render
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// ? Register your services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<PredictionService>();

var app = builder.Build();

// ? Configure the HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ? Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Currency}/{action=Index}/{id?}");

app.Run();
