using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;

namespace BalkanTech.Tests
{
    public class AdminServiceTests
    {
        private BalkanDbContext context;
        private AdminService adminService;
        private Mock<UserManager<AppUser>> userManager;
        private Mock<RoleManager<IdentityRole<Guid>>> roleManager;

        private List<AppUser> users;
        private List<IdentityRole<Guid>> roles;

        private Guid userId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BalkanDbContext>()
                .UseInMemoryDatabase(databaseName: "BalkanTech")
                .Options;
            this.context = new BalkanDbContext(options);

            var userStore = new Mock<IUserStore<AppUser>>();
            userManager = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole<Guid>>>();
            roleManager = new Mock<RoleManager<IdentityRole<Guid>>>(roleStore.Object, null, null, null, null);

            adminService = new AdminService(context, userManager.Object, roleManager.Object);

            roles = new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin" },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Technician" },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Manager" }
            };

            roleManager.Setup(rm => rm.RoleExistsAsync(It.Is<string>(name => roles.Any(r => r.Name == name))))
                       .ReturnsAsync(true);
            roleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync((string roleName) => roles.FirstOrDefault(r => r.Name == roleName));
            userId =Guid.NewGuid();
            users = new List<AppUser>
            {
                new AppUser { Id = Guid.NewGuid(), UserName = "admin", Email = "admin@test.com" },
                new AppUser { Id = userId, UserName = "tech", Email = "tech@test.com" },
                new AppUser { Id = Guid.NewGuid(), UserName = "manager", Email = "manager@test.com" }
            };

            var userListQueryable = users.AsQueryable().BuildMock();

            userManager.Setup(um => um.Users).Returns(userListQueryable);

            userManager.Setup(um => um.GetUsersInRoleAsync(It.Is<string>(role => role == "Admin")))
                       .ReturnsAsync(users.Where(u => u.UserName == "admin").ToList());

            userManager.Setup(um => um.GetUsersInRoleAsync(It.Is<string>(role => role == "Technician")))
                       .ReturnsAsync(users.Where(u => u.UserName == "tech").ToList());

            userManager.Setup(um => um.GetUsersInRoleAsync(It.Is<string>(role => role == "Manager")))
                       .ReturnsAsync(users.Where(u => u.UserName == "manager").ToList());
            userManager.Setup(um => um.RemoveFromRolesAsync(It.IsAny<AppUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

            userManager.Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManager.Setup(um => um.DeleteAsync(It.IsAny<AppUser>()))
           .ReturnsAsync(IdentityResult.Success);

            foreach (var user in users)
            {
                userManager.Setup(um => um.FindByIdAsync(user.Id.ToString()))
                           .ReturnsAsync(user);

                userManager.Setup(um => um.GetRolesAsync(It.Is<AppUser>(u => u.Id == user.Id)))
                           .ReturnsAsync(user.UserName == "tech" ? new List<string> { "Technician" } :
                                         user.UserName == "manager" ? new List<string> { "Manager" } :
                                         new List<string> { "Admin" });
            }
        }


        [Test]

        public async Task IndexGetAllTechsAsyncShouldreturnSuccessfulTechsAndManager()
        {
            var result = await adminService.IndexGetAllTechsAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]

        public async Task FindAppUserByIdAsyncShouldReturnUserCorrect()
        {
            var result = await adminService.FindAppUserByIdAsync(userId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("tech@test.com"));
        }

        [Test]
        public async Task FindAppUserByIdAsyncShouldThrowExceptionWhenUserIdIsInvalid()
        {
            var userId = Guid.Empty;
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await adminService.FindAppUserByIdAsync(userId));
        }
        [Test]
        public async Task ChangeRoleOfUserAsyncShouldBeSuccessfull() 
        {
            var newRole = "Manager";
            var result = await adminService.ChangeRoleOfUserAsync(userId, newRole);
            Assert.That(result.Succeeded);
        }
        [Test]
        public async Task DeleteUserAsyncShouldBeSuccessfull() 
        {
            var result = await adminService.DeleteUserAsync(userId);
            Assert.That(result.Succeeded);

        }
        [TearDown]
        public void TearDown()
        {
            context.Dispose();
            roleManager.Object.Dispose();
        }
      
    }
}
