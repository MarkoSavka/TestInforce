using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Tests
{
    [TestFixture]
    public class UserRepositoryTest
    {
        private MyDbContext _context;
        private UserRepository _userRepository;
        private ILogger<UserRepositoryTest> _logger;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MyDbContext(options);

            var userStore = new UserStore<User, Role, MyDbContext, int>(_context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);

            _userRepository = new UserRepository(_context, userManager);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<UserRepositoryTest>();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddUser_ShouldSaveUserToDatabase()
        {
            _logger.LogInformation("Starting test: AddUser_ShouldSaveUserToDatabase");

            // Arrange
            var user = new User { Id = 1, UserName = "testuser", Email = "testuser@example.com" };

            // Act
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.UserName, Is.EqualTo("testuser"));
            Assert.That(result.Email, Is.EqualTo("testuser@example.com"));

            _logger.LogInformation("Completed test: AddUser_ShouldSaveUserToDatabase");
        }

        [Test]
        public async Task UpdateUser_ShouldModifyUserInDatabase()
        {
            _logger.LogInformation("Starting test: UpdateUser_ShouldModifyUserInDatabase");

            // Arrange
            var user = new User { Id = 1, UserName = "testuser", Email = "testuser@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.UserName = "updateduser";
            user.Email = "updateduser@example.com";
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo("updateduser"));
            Assert.That(result.Email, Is.EqualTo("updateduser@example.com"));

            _logger.LogInformation("Completed test: UpdateUser_ShouldModifyUserInDatabase");
        }

        [Test]
        public async Task DeleteUser_ShouldRemoveUserFromDatabase()
        {
            _logger.LogInformation("Starting test: DeleteUser_ShouldRemoveUserFromDatabase");

            // Arrange
            var user = new User { Id = 1, UserName = "testuser", Email = "testuser@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);

            // Assert
            Assert.That(result, Is.Null);

            _logger.LogInformation("Completed test: DeleteUser_ShouldRemoveUserFromDatabase");
        }
    }
}