using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using Microsoft.AspNetCore.Identity;

public interface IUserRepository
{
    Task<List<User>> GetUsersAsync();
    Task<User> FindByNameAsync(string username);
    Task<IdentityResult> CreateAsync(User user, string password);
    Task AddToRoleAsync(User user, string role);
    Task<bool> CheckPasswordAsync(User user, string password);
}
