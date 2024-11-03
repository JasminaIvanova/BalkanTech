using BalkanTech.Data.Models;
using Microsoft.EntityFrameworkCore;
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

        public void Seed<T>(string datasetFileName, DbSet<T> dbSet) where T : class
        {
           
            if (!dbSet.Any())
            {
                var jsonFilePath = Path.Combine(AppContext.BaseDirectory, "Datasets", datasetFileName);

                if (File.Exists(jsonFilePath))
                {

                    var jsonData = File.ReadAllText(jsonFilePath);
                    var data = JsonConvert.DeserializeObject<List<T>>(jsonData);

                    dbSet.AddRange(data);
                    _context.SaveChanges();
                }
                else
                {
                    throw new FileNotFoundException($"File {datasetFileName} not found");
                }
            }
        }
    }
}
