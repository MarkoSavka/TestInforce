using API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers;

public class UserController : BaseApiController
{
    private readonly UserService _userService;
    private readonly UserManager<User> _userManager;

    public UserController(UserService userService,UserManager<User> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        if (User.Identity.IsAuthenticated)
        {
            return await _userService.GetUsersAsync();
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _userService.RegisterAsync(registerDto);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem();
        }

        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var userDto = await _userService.LoginAsync(loginDto);
        if (userDto == null)
        {
            return Unauthorized();
        }

        return userDto;
    }

    [Authorize(Roles = "Member,Admin")]
    [HttpGet("currentUser")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var username = User.Identity.Name;
        var user = await _userManager.FindByNameAsync(username);
        var userDto = await _userService.GetCurrentUserAsync(user.Email);
        if (userDto == null)
        {
            return Unauthorized();
        }

        return userDto;
    }
}
