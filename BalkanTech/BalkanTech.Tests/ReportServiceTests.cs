using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Tests
{
    public class ReportServiceTests
    {
        private BalkanDbContext context;
        private ReportService reportService;
        private Mock<UserManager<AppUser>> userManager;
        private Mock<ITaskService> taskService;

        private Guid technicianId;
        private Guid taskCategoryId;
        private Guid roomId;
        private Guid taskId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BalkanDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var userStore = new Mock<IUserStore<AppUser>>();
            userManager = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            taskService = new Mock<ITaskService>();
            context = new BalkanDbContext(options);
            reportService = new ReportService(context, userManager.Object, taskService.Object);

            technicianId = Guid.NewGuid();
            taskCategoryId = Guid.NewGuid();
            roomId = Guid.NewGuid();
            taskId = Guid.NewGuid();

            context.Users.Add(new AppUser { Id = technicianId, UserName = "techuser", Email = "tech@example.com" });

            context.Rooms.Add(new Room { Id = roomId, RoomNumber = 101 });

            context.TaskCategories.Add(new TaskCategory { Id = taskCategoryId, Name = "Plumbing" });

            context.MaintananceTasks.Add(new MaintananceTask
            {
                Id = taskId,
                Name = "Fix Leak",
                Description = "Fix a leak in Room A",
                RoomId = roomId,
                TaskCategoryId = taskCategoryId,
                Status = "Pending",
                IsDeleted = false,
                DueDate = DateTime.Now,
            });

            context.AssignedTechniciansTasks.Add(new AssignedTechnicianTask
            {
                AppUserId = technicianId,
                MaintananceTaskId = taskId,
            });
           

            context.SaveChanges();
            foreach (var user in context.Users)
            {
                userManager.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                          .ReturnsAsync(user);
            }
           
        }

        [Test]
        public async Task GetTasksForUserAsync_ShouldReturnTasks()
        {
            var result = await reportService.GetTasksForUserAsync(technicianId.ToString());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToBeCompletedTasks.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetTasksForUserAsyncShouldThrowExceptionWhenInvalidUserId()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await reportService.GetTasksForUserAsync(""));
        }
        [Test]
        public async Task ChangeTaskStatusShouldUpdateStatusToInProcessSuccessfull()
        {
            await reportService.ChangeTaskStatus(taskId, "In Process", DateTime.Now, technicianId.ToString(), false);
            var task = await context.MaintananceTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            Assert.That(task, Is.Not.Null);
            Assert.That(task.Status, Is.EqualTo("In Process"));
        }
        [Test]
        public async Task ChangeTaskStatusShouldUpdateStatusToPendingAprovalSuccessfull()
        {
            var newDate = DateTime.Now;
            await reportService.ChangeTaskStatus(taskId, "Pending Manager Approval", newDate, technicianId.ToString(), false);
            var task = await context.MaintananceTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            Assert.That(task, Is.Not.Null);
            Assert.That(task.Status, Is.EqualTo("Pending Manager Approval"));
            Assert.That(task.CompletedDate, Is.EqualTo(newDate));
        }

        [Test]
        public async Task ChangeTaskStatusShouldThrowExceptionUnauthorized()
        {
            var secondTechnicianId = Guid.NewGuid();
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await reportService.ChangeTaskStatus(taskId, "Pending Manager Approval", DateTime.Now, secondTechnicianId.ToString(), false));
        }

        [Test]
        public async Task ChangeTaskStatusShouldBeCompletedWhenUserIsManager()
        {
            await reportService.ChangeTaskStatus(taskId, "Pending Manager Approval", DateTime.Now, technicianId.ToString(), false);
            await reportService.ChangeTaskStatus(taskId, "Completed", DateTime.Now, technicianId.ToString(), true);
            var task = await context.MaintananceTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            Assert.That(task, Is.Not.Null);
            Assert.That(task.Status, Is.EqualTo("Completed"));
        }

            [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
