using Microsoft.AspNetCore.Authentication.Cookies;
using InitialSetupMVC.Data;
using InitialSetupMVC.Repositories;
using InitialSetupMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Custom Database & Repository Layers
builder.Services.AddSingleton<DbConnection>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<MedicineRepository>();
builder.Services.AddScoped<RequestRepository>();

// Register Custom Business Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<ApprovalService>();
builder.Services.AddScoped<DashboardService>();

// Register Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error"); // Use dashboard as entry
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // Added authentication
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
