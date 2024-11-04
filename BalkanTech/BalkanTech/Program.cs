using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
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
        cfg.Password.RequireNonAlphanumeric = false;
        cfg.Password.RequireUppercase = false;
        cfg.Password.RequireLowercase = false;

    })
    .AddEntityFrameworkStores<BalkanDbContext>() 
    .AddUserManager<UserManager<AppUser>>()      
    .AddRoles<IdentityRole<Guid>>()               
    .AddSignInManager<SignInManager<AppUser>>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddTransient<SeedData>();
builder.Services.AddScoped<IRoomService, RoomService>();
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
 //seeding
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();
    seeder.Seed("roomCategories.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().RoomCategories);
    seeder.Seed("rooms.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().Rooms);
}
//roles
using (var scope = app.Services.CreateScope())
{
   
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = new[] { "Admin", "Manager", "Technician" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
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
