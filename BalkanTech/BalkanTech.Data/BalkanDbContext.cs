
using BalkanTech.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace BalkanTech.Data
{
    public class BalkanDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public BalkanDbContext(DbContextOptions<BalkanDbContext> options)
           : base(options)
        {
        }

        public  DbSet<Room> Rooms { get; set; }
        public  DbSet<RoomCategory> RoomCategories { get; set; }
        public  DbSet<MaintananceTask> MaintananceTasks { get; set; }
        public DbSet<AssignedTechnicianTask> AssignedTechniciansTasks { get; set; }

        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<Note> Notes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

       



    }

}
