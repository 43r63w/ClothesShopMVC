using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClothesShop.DAL.Data;
using ClothesShop.DAL.DbInitializers;
using Microsoft.AspNetCore.Identity.UI.Services;
using ClothesShop.Serivices;
using Microsoft.Data.SqlClient;
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.DAL.Repository;
using Stripe;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDbConnection")));

builder.Services.AddIdentity
    <IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/IdentityAccount/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();
SeedDataBase();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedDataBase()
{

    using (var scope = app.Services.CreateScope())
    {
        var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        DbInitializer.Initialize();
    }

}