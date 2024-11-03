using BalkanTech.Data;
using BalkanTech.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<BalkanDbContext>(options => 
{
    options.UseSqlServer(connectionString);

});

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(cfg =>
    {
        cfg.Password.RequiredLength = 0;
    })
    .AddEntityFrameworkStores<BalkanDbContext>() 
    .AddUserManager<UserManager<AppUser>>()      
    .AddRoles<IdentityRole<Guid>>()               
    .AddSignInManager<SignInManager<AppUser>>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddTransient<SeedData>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();  // Retrieve the registered SeedData service
    seeder.Seed("roomCategories.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().RoomCategories);
    seeder.Seed("rooms.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().Rooms);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
