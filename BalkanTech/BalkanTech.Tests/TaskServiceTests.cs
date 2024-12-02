using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Globalization;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Tests
{
    public class TaskServiceTests
    {
        private BalkanDbContext context;
        private TaskService taskService;
        private Mock<UserManager<AppUser>> userManager;

        private MaintananceTask task;
        private Room room;
        private TaskCategory category;
        private MaintananceTask completedTask;

        private Guid doubleRoomCategoryId;
        private Guid SuitRoomCategoryId;
        private Guid doubleRoomId;
        private Guid SuitRoomId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BalkanDbContext>()
            .UseInMemoryDatabase(databaseName: "BalkanTech")
            .Options;
            this.context = new BalkanDbContext(options);

            var userStore = new Mock<IUserStore<AppUser>>();
            userManager = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);


            this.taskService = new TaskService(context, userManager.Object);

        
            room = new Room { Id = Guid.NewGuid(), RoomNumber = 101 };
            category = new TaskCategory { Id = Guid.NewGuid(), Name = "Plumbing" };
            var secondCategory = new TaskCategory { Id = Guid.NewGuid(), Name = "Electrical" };
            task = new MaintananceTask
            {
                Id = Guid.NewGuid(),
                Name = "Test Task",
                Description = "Details",
                Room = room,
                TaskCategory = category,
                DueDate = DateTime.Now,
            };
             completedTask = new MaintananceTask
            {
                Id = Guid.NewGuid(),
                Name = "Test Task Completed",
                Description = "Details",
                Room = room,
                TaskCategory = secondCategory,
                DueDate = DateTime.Now,
                Status = "Completed"
            };
            context.Rooms.Add(room);
            context.TaskCategories.Add(category);
            context.MaintananceTasks.Add(task);
            context.MaintananceTasks.Add(completedTask);
            context.SaveChangesAsync();

        }


        [Test]

        public async Task AddTaskAsyncShouldAddTaskSuccessfully()
        {
            var model = new TaskAddViewModel
            {
                Name = "Test Task",
                Description = "Task Description",
                RoomId = Guid.NewGuid(),
                TaskCategoryId = Guid.NewGuid(),
                AssignedTechniciansIDs = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                 DueDate = DateTime.UtcNow.ToString(dateFormat)
            };
            var dueDate = DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
                   DateTimeStyles.None, out DateTime parsedDueDate);
            await taskService.AddTaskAsync(model, parsedDueDate);
            var taskFound = await context.MaintananceTasks.FirstOrDefaultAsync(t => t.Name == model.Name);
            Assert.That(taskFound, Is.Not.Null);
            Assert.That(context.MaintananceTasks.Count, Is.EqualTo(2));

        }

        [Test]
        public async Task LoadTaskDetailsAsyncShouldReturnTaskDetailsSuccessfull()
        {
            
            var resultModel = await taskService.LoadTaskDetailsAsync(task.Id);
            Assert.That(resultModel, Is.Not.Null);
            Assert.That(resultModel.Name, Is.EqualTo(task.Name));
            Assert.That(resultModel.Description, Is.EqualTo(task.Description));

        }

        [Test]
        public async Task EditTaskAsyncShouldUpdateTaskDetailsSuccessfull()
        {
            var model = new TaskAddViewModel
            {
                Id = task.Id,
                Name = "Updated Task Test",
                Description = "Updated Description Test",
                RoomId = task.RoomId,
                TaskCategoryId = task.TaskCategoryId,
                DueDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                AssignedTechniciansIDs = new List<Guid>()
            };
            var dueDate = DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
                   DateTimeStyles.None, out DateTime parsedDueDate);

            await taskService.EditTaskAsync(model, parsedDueDate);
            var updatedTask = await context.MaintananceTasks.FindAsync(task.Id);
            Assert.That(updatedTask, Is.Not.Null);
            Assert.That(updatedTask.Name, Is.EqualTo(model.Name));
            Assert.That(updatedTask.Description, Is.EqualTo(model.Description));

        }
        [Test]
        public async Task DeleteTaskAsyncShouldBeSuccessfull()
        {
            var model = new TaskDeleteViewModel
            {

                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                RoomNumber = task.Room.RoomNumber,
                RoomId = task.RoomId,
            };
           var result =  await taskService.DeleteTaskAsync(model);
            Assert.That(result, Is.True);
        }
        [Test]
        public async Task LoadEditTaskAsyncShouldThrowExceptionWhenTaskCompleted()
        {
            task.Status = "Completed";
            Assert.ThrowsAsync<ArgumentException>(() => taskService.LoadEditTaskAsync(task.Id));
        }
        [Test]
        public async Task IndexGetAllTasksAsyncShouldReturnValidCount()
        {
            
            var result = await taskService.IndexGetAllTasksAsync(room.Id, room.RoomNumber, null, null, 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToBeCompletedTasks.TotalItems, Is.EqualTo(1)); 
            Assert.That(result.CompletedTasks.TotalItems, Is.EqualTo(1)); 
        }
        [Test]
        public async Task IndexGetAllTasksAsyncFilterTasksSuccessfull()
        {
            var result = await taskService.IndexGetAllTasksAsync(room.Id, room.RoomNumber, null, null, 10, "Electrical");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToBeCompletedTasks.Items, Is.Empty); 
            Assert.That(result.CompletedTasks.Items.Count, Is.EqualTo(1));
            
        }
        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

    }
}
