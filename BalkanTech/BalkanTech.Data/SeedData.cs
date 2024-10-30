using BalkanTech.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Data
{
    public class SeedData
    {
        private readonly BalkanDbContext _context;

        public SeedData(BalkanDbContext context)
        {
            _context = context;
        }

        public void SeedCategories()
        {
            
            if (!_context.RoomCategories.Any())
            {
                var categoriesJsonPath = Path.Combine(AppContext.BaseDirectory, "Datasets/roomCategories.json");

                if (File.Exists(categoriesJsonPath))
                {
                    var categoriesJsonFile = File.ReadAllText(categoriesJsonPath);
                    var categories = JsonConvert.DeserializeObject<List<RoomCategory>>(categoriesJsonFile);
                    _context.RoomCategories.AddRange(categories);
                    _context.SaveChanges();
                }
                else
                {
                    throw new FileNotFoundException("File not found");
                }
            }
        }
        public void SeedRooms()
        {

            if (!_context.Rooms.Any())
            {
                var roomsJsonPath = Path.Combine(AppContext.BaseDirectory, "Datasets/rooms.json");

                if (File.Exists(roomsJsonPath))
                {
                    var roomsJsonFile = File.ReadAllText(roomsJsonPath);
                    var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsJsonFile);
                    _context.Rooms.AddRange(rooms);
                    _context.SaveChanges();
                }
                else
                {
                    throw new FileNotFoundException("File not found");
                }
            }
        }
    }
}
