using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Web.ViewModels.Room;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Tests
{
    public class RoomServiceTests
    {
        private BalkanDbContext context;
        private RoomService roomService;
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
            this.roomService = new RoomService(context);

            doubleRoomCategoryId = Guid.NewGuid();
            SuitRoomCategoryId = Guid.NewGuid();
            doubleRoomId = Guid.NewGuid();
            SuitRoomId = Guid.NewGuid();

            var categories = new List<RoomCategory>() {
                new RoomCategory(){ Id = doubleRoomCategoryId, RoomType = "DoubleRoom"},
                new RoomCategory(){ Id = SuitRoomCategoryId, RoomType = "Suit"}
            };
            var rooms = new List<Room>() {
                new Room(){Id = doubleRoomId, RoomNumber = 101, Floor = 1, RoomCategoryId = doubleRoomCategoryId, isAvailable = true},
                new Room(){Id = SuitRoomId, RoomNumber = 202, Floor = 2, RoomCategoryId = SuitRoomCategoryId, isAvailable = false }
            };

            this.context.RoomCategories.AddRangeAsync(categories);
            this.context.Rooms.AddRangeAsync(rooms);
            this.context.SaveChanges();
        }
       

        [Test]
        public async Task LoadRoomCategoriesAsyncShouldReturnTotalOf2()
        {
           var categories = await roomService.LoadRoomCategoriesAsync();
            Assert.That(categories, Is.Not.Empty);
            Assert.That(categories.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task LoadRoomsBySearchShouldReturnCorrectCountWhenSearchByNumberOfRoom()
        {
            var rooms = await roomService.LoadRoomsBySearch("0");
            Assert.That(rooms, Is.Not.Empty);
            Assert.That(rooms.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task LoadRoomsBySearchShouldReturnCorrectCountWhenSearchByCategory()
        {
            var rooms = await roomService.LoadRoomsBySearch("suit");
            Assert.That(rooms, Is.Not.Empty);
            Assert.That(rooms.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task IndexGetAllRoomsAsyncShouldReturnCorrectCount()
        {
            var roomsIndex = await roomService.IndexGetAllRoomsAsync("", 1, 9);
            Assert.That(roomsIndex.Items, Is.Not.Empty);
            Assert.That(roomsIndex.Items.Count(), Is.EqualTo(2));
            Assert.That(roomsIndex.TotalItems, Is.EqualTo(2));
            Assert.That(roomsIndex.TotalPages, Is.EqualTo(1));
        }

        [Test]
        public async Task AddRoomAsyncShouldReturnTrueWhenSuccessfull()
        {
            var room = new RoomAddViewModel
            {
                RoomNumber = 333,
                Floor = 3,
                RoomCategoryId = doubleRoomCategoryId,
                isAvailable = true
            };
            var result = await roomService.AddRoomAsync(room);
            var foundRoom = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == 333);
            Assert.That(result, Is.True);
            Assert.That(foundRoom, Is.Not.Null);
            Assert.That(foundRoom.RoomNumber, Is.EqualTo(333));
        }
        [Test]
        public async Task AddRoomAsyncShouldReturnFalseWhenSameNumberRoomTriedToBeAdded()
        {
            var room = new RoomAddViewModel
            {
                RoomNumber = 101,
                Floor = 1,
                RoomCategoryId = doubleRoomCategoryId,
                isAvailable = true
            };
            var result = await roomService.AddRoomAsync(room);
            Assert.That(result, Is.False);
        }
        [Test]
        public async Task ChangeRoomStatusShouldBeSuccessfull()
        {
            await roomService.ChangeRoomStatus(doubleRoomId);
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == doubleRoomId);
            Assert.That(room, Is.Not.Null);
            Assert.That(room.isAvailable, Is.False);
        }

       [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
