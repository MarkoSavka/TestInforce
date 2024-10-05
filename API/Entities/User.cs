using Microsoft.AspNetCore.Identity;

namespace API.DTO;

public class User : IdentityUser<int>
{
    public List<URL> Urls { get; set; }
}