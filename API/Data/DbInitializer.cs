using API.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class DbInitializer
{
    public static async Task Initialize(MyDbContext context, UserManager<User> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new User
            {
                UserName = "ivan",
                Email = "ivan@test.com",
                Urls = new List<URL>()
            };

            user.Urls.AddRange(new List<URL>
            {
                new URL { ShortUrl = "someurl1", FullUrl = "http://someurl1.com", CreatedBy = user, CreatedDate = DateTime.UtcNow },
                new URL { ShortUrl = "someurl2", FullUrl = "http://someurl2.com", CreatedBy = user, CreatedDate = DateTime.UtcNow }
            });

            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");

            var admin = new User
            {
                UserName = "artur",
                Email = "artur@test.com",
                Urls = new List<URL>()
            };

            admin.Urls.AddRange(new List<URL>
            {
                new URL { ShortUrl = "short3", FullUrl = "http://someurl3.com", CreatedBy = admin, CreatedDate = DateTime.UtcNow },
                new URL { ShortUrl = "short4", FullUrl = "http://someurl4.com", CreatedBy = admin, CreatedDate = DateTime.UtcNow }
            });

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Member", "Admin" });
        }
        await context.SaveChangesAsync();
    }
}