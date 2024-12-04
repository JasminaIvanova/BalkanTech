using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Tests
{
    public class ReportServiceTests
    {
        //private BalkanDbContext context;
        //private ReportService reportService;

        //private Guid technicianId;
        //private Guid taskCategoryId ;
        //private Guid roomId;
        //private Guid taskId;

        //[SetUp]
        //public void Setup()
        //{
        //    var options = new DbContextOptionsBuilder<BalkanDbContext>()
        //        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //        .Options;

        //    context = new BalkanDbContext(options);
        //    reportService = new ReportService(context);

        //    technicianId = Guid.NewGuid();
        //    taskCategoryId = Guid.NewGuid();
        //    roomId = Guid.NewGuid();
        //    taskId = Guid.NewGuid();

        //    context.Users.Add(new AppUser { Id = technicianId, UserName = "techuser", Email = "tech@example.com" });

        //    context.Rooms.Add(new Room { Id = roomId, RoomNumber = 101 });

        //    context.TaskCategories.Add(new TaskCategory { Id = taskCategoryId, Name = "Plumbing" });

        //    context.MaintananceTasks.Add(new MaintananceTask
        //    {
        //        Id = taskId,
        //        Name = "Fix Leak",
        //        Description = "Fix a leak in Room A",
        //        RoomId = roomId,
        //        TaskCategoryId = taskCategoryId,
        //        Status = "Pending",
        //        IsDeleted = false,
        //        DueDate = DateTime.Now,
        //    });

        //    context.AssignedTechniciansTasks.Add(new AssignedTechnicianTask
        //    {
        //        AppUserId = technicianId,
        //        MaintananceTaskId = taskId,
        //    });

        //    context.SaveChanges();
        //}
        //[TearDown]
        //public void TearDown()
        //{
        //    context.Dispose();
        //}
    }
}
