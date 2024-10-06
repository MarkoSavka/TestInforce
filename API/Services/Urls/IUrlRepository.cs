using API.DTO;

namespace API.Services;

public interface IUrlRepository
{
     Task<IEnumerable<URL>> GetAllUrls();
     Task<URL> GetUrlById(int id);
     Task AddUrlAsync(URL url);
     Task UpdateUrlAsync(URL url);
     Task DeleteUrlAsync(URL url);
}
