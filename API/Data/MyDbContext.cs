using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MyDbContext:IdentityDbContext<User,Role,int>
{
    public MyDbContext(DbContextOptions opt):base(opt)
    {
    }
    public DbSet<URL> Urls { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasMany(u => u.Urls)
            .WithOne(u => u.CreatedBy)
            .HasForeignKey(u => u.CreatedById);
        
        builder.Entity<Role>()
            .HasData(
                new Role { Id = 1, Name = "Member", NormalizedName = "MEMBER" },
                new Role { Id = 2, Name = "Admin", NormalizedName = "ADMIN" });
    }
}
