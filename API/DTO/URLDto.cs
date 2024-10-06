namespace API.DTO;


public class URLDto
{
    public int Id { get; set; }
    public string ShortUrl { get; set; }
    public string FullUrl { get; set; }
    public DateTime CreatedDate { get; set; }
}