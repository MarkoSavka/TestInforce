namespace API.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public List<URLDto> Urls { get; set; }
}
