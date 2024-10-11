
using BalkanTech.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BalkanTech.Data
{
    public class BalkanDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public BalkanDbContext(DbContextOptions options)
           : base(options)
        {
        }
        public BalkanDbContext()
        {
            
        }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomCategory> RoomCategories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        
    }

}
