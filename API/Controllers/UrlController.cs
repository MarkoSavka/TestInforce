using API.DTO;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class UrlController : BaseApiController
    {
        private readonly UrlService _urlService;
        private readonly UserManager<User> _userManager;

        public UrlController(UrlService urlService,UserManager<User> userManager)
        {
            _urlService = urlService;
            _userManager = userManager;
        }

        [HttpGet("get")]
        public async Task<ActionResult<List<URL>>> GetUrls()
        {
            var urls = await _urlService.GetAllAsync();
            return Ok(urls.ToList());
        }

        [HttpPost("add")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult> AddUrl(URLDto urlDto)
        {
            if (User.Identity?.Name == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _urlService.AddAsync(urlDto, user);
            return new ContentResult { StatusCode = 201, Content = "Url successfully added" };
        }

        [HttpPut("update")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<ActionResult> UpdateUrl(URLDto urlDto)
        {
            if (User.Identity?.Name == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var url = await _urlService.GetByIdAsync(urlDto.Id);
            if (url == null)
            {
                return NotFound("Url not found.");
            }

            if (User.IsInRole("Admin") || url.CreatedBy == user.Id)
            {
                await _urlService.UpdateAsync(urlDto);
                return new ContentResult { StatusCode = 201, Content = "Url successfully updated" };
            }

            return new ContentResult { Content = "You do not have permission to update this URL.", StatusCode = 403 };
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<ActionResult> DeleteUrl(int id)
        {
            var url = await _urlService.GetByIdAsync(id);
            if (url == null)
            {
                return NotFound("Url not found.");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            if (User.IsInRole("Admin") || url.CreatedBy == user.Id)
            {
                await _urlService.DeleteAsync(new URLDto { Id = id });
                return new ContentResult { Content = "Url successfully deleted" };
            }

            return new ContentResult { Content = "You do not have permission to delete this URL.", StatusCode = 403 };
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<URLResponseDto>> GetUrl(int id)
        {
            var url = await _urlService.GetByIdAsync(id);
            if (url == null) return NotFound();
            return url;
        }
    }
}
