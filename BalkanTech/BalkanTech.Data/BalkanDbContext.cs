
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

        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomCategory> RoomCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

       



    }

}
