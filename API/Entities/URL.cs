namespace API.DTO;

public class URL
{
    public int Id { get; set; }
    public string ShortUrl { get; set; }
    public string FullUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; }
}
