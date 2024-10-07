namespace API.DTO;

public class RegisterDto:LoginDto
{
    public string Username { get; set; }
    public string Email { get; set; }
}
