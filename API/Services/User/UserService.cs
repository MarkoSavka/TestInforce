using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;
    private readonly MyDbContext _context;

    public UserService(IUserRepository userRepository, TokenService tokenService,MyDbContext context)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User { UserName = registerDto.Username, Email = registerDto.Email };
        var result = await _userRepository.CreateAsync(user, registerDto.Password);
        if (result.Succeeded)
        {
            await _userRepository.AddToRoleAsync(user, "Member");
        }
        return result;
    }

    public async Task<UserDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userRepository.CheckPasswordAsync(user, loginDto.Password))
        {
            return null;
        }

        var userUrls = await RetrieveUrls(user);

        var urlDtos = userUrls.Select(url => new URLDto
        {
            Id = url.Id,
            ShortUrl = url.ShortUrl,
            FullUrl = url.FullUrl,
            CreatedDate = url.CreatedDate
        }).ToList();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.UserName,
            Token = await _tokenService.GenerateToken(user),
            Urls = urlDtos
        };
    }

    public async Task<UserDto> GetCurrentUserAsync(string email)
    {
        var user = await _userRepository.FindByEmailAsync(email);

        var userUrls = await RetrieveUrls(user);

        var urlDtos = userUrls.Select(url => new URLDto
        {
            Id = url.Id,
            ShortUrl = url.ShortUrl,
            FullUrl = url.FullUrl,
            CreatedDate = url.CreatedDate
        }).ToList();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.UserName,
            Token = await _tokenService.GenerateToken(user),
            Urls = urlDtos
        };
    }

    private async Task<List<URL>> RetrieveUrls(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        if (user.Urls == null)
        {
            return new List<URL>();
        }

        return user.Urls.Select(url => new URL
        {
            Id = url.Id,
            ShortUrl = url.ShortUrl,
            FullUrl = url.FullUrl,
            CreatedDate = url.CreatedDate,
            CreatedById = url.CreatedById
        }).ToList();
    }
}