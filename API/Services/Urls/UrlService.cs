using API.DTO;
using AutoMapper;

namespace API.Services;

public class UrlService
{
    private readonly IUrlRepository _urlRepository;
    private readonly IMapper _mapper;

    public UrlService(IUrlRepository urlRepository,IMapper mapper)
    {
        _urlRepository = urlRepository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<URL>> GetAllAsync()
    {
        return await _urlRepository.GetAllUrls();
    }

    public async Task<URLResponseDto> GetByIdAsync(int id)
    {
        var url = await _urlRepository.GetUrlById(id);
        if (url == null) return null;

        var urlResponseDto = _mapper.Map<URLResponseDto>(url);
        urlResponseDto.CreatedBy = url.CreatedById;
        return urlResponseDto;
    }

    public async Task AddAsync(URLDto urlDto,DTO.User user)
    {
        var url = _mapper.Map<URL>(urlDto);
        url.CreatedBy = user;
        await _urlRepository.AddUrlAsync(url);
    }

    public async Task UpdateAsync(URLDto urlDto)
    {
       
        var existingUrl = await _urlRepository.GetUrlById(urlDto.Id);
        if (existingUrl == null)
        {
            throw new InvalidOperationException("URL not found.");
        }

        _mapper.Map(urlDto, existingUrl);
        await _urlRepository.UpdateUrlAsync(existingUrl);
    }

    public async Task DeleteAsync(URLDto urlDto)
    {
        var url = _mapper.Map<URL>(urlDto);
        await _urlRepository.DeleteUrlAsync(url);
    }


    public async Task GenerateShortUrl(URLDto urlDto)
    {
        var cleanedUrl = urlDto.FullUrl.Replace("https://", "").Replace(".com", "");

        urlDto.ShortUrl = cleanedUrl;

        // Save the URL with the short URL
        var url = _mapper.Map<URL>(urlDto);
        await _urlRepository.AddUrlAsync(url);
    }
    
}
