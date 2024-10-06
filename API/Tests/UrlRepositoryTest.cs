
using API.Data;
using API.DTO;
using API.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace API.Tests
{
    [TestFixture]
    public class UrlRepositoryTest
    {
        private MyDbContext _context;
        private UrlRepository _urlRepository;
        private ILogger<UrlRepositoryTest> _logger;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MyDbContext(options);
            _urlRepository = new UrlRepository(_context);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<UrlRepositoryTest>();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddUrl_ShouldSaveUrlToDatabase()
        {
            _logger.LogInformation("Starting test: AddUrl_ShouldSaveUrlToDatabase");

            // Arrange
            var url = new URL { Id = 1, ShortUrl = "short1", FullUrl = "http://fullurl1.com" };

            // Act
            _context.Urls.Add(url);
            await _context.SaveChangesAsync();
            var result = await _context.Urls.FirstOrDefaultAsync(u => u.Id == 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ShortUrl, Is.EqualTo("short1"));
            Assert.That(result.FullUrl, Is.EqualTo("http://fullurl1.com"));

            _logger.LogInformation("Completed test: AddUrl_ShouldSaveUrlToDatabase");
        }

        [Test]
        public async Task GetUrlById_ShouldReturnNull_WhenUrlDoesNotExist()
        {
            _logger.LogInformation("Starting test: GetUrlById_ShouldReturnNull_WhenUrlDoesNotExist");

            // Act
            var result = await _urlRepository.GetUrlById(2);
            _logger.LogInformation("Result from GetUrlById: {Result}", result);

            // Assert
            Assert.That(result, Is.Null);

            _logger.LogInformation("Completed test: GetUrlById_ShouldReturnNull_WhenUrlDoesNotExist");
        }
        
        [Test]
        public async Task UpdateUrl_ShouldModifyUrlInDatabase()
        {
            _logger.LogInformation("Starting test: UpdateUrl_ShouldModifyUrlInDatabase");

            // Arrange
            var url = new URL { Id = 1, ShortUrl = "short1", FullUrl = "http://fullurl1.com" };
            _context.Urls.Add(url);
            await _context.SaveChangesAsync();

            // Act
            url.ShortUrl = "updatedShort";
            url.FullUrl = "http://updatedfullurl.com";
            _context.Urls.Update(url);
            await _context.SaveChangesAsync();
            var result = await _context.Urls.FirstOrDefaultAsync(u => u.Id == 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ShortUrl, Is.EqualTo("updatedShort"));
            Assert.That(result.FullUrl, Is.EqualTo("http://updatedfullurl.com"));

            _logger.LogInformation("Completed test: UpdateUrl_ShouldModifyUrlInDatabase");
        }
        
        [Test]
        public async Task DeleteUrl_ShouldRemoveUrlFromDatabase()
        {
            _logger.LogInformation("Starting test: DeleteUrl_ShouldRemoveUrlFromDatabase");

            // Arrange
            var url = new URL { Id = 1, ShortUrl = "short1", FullUrl = "http://fullurl1.com" };
            _context.Urls.Add(url);
            await _context.SaveChangesAsync();

            // Act
            _context.Urls.Remove(url);
            await _context.SaveChangesAsync();
            var result = await _context.Urls.FirstOrDefaultAsync(u => u.Id == 1);

            // Assert
            Assert.That(result, Is.Null);

            _logger.LogInformation("Completed test: DeleteUrl_ShouldRemoveUrlFromDatabase");
        }
    }
}