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
    .AddRoles<IdentityRole<Guid>>()               
    .AddUserManager<UserManager<AppUser>>()     
    .AddSignInManager<SignInManager<AppUser>>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddTransient<SeedData>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
   app.UseStatusCodePagesWithReExecute("/Error/{0}");
}
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
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();
    seeder.Seed("roomCategories.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().RoomCategories);
    seeder.Seed("rooms.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().Rooms);
    seeder.Seed("taskCategories.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().TaskCategories);
    seeder.Seed("tasks.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().MaintananceTasks);
    await SeedUsers.SeedUsersAsync(scope.ServiceProvider);
    seeder.Seed("assignedTechniciansTasks.json", scope.ServiceProvider.GetRequiredService<BalkanDbContext>().AssignedTechniciansTasks);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePages();

app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
app.MapControllerRoute(
               name: "Errors",
               pattern: "{controller=Home}/{action=Index}/{statusCode?}");

app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
