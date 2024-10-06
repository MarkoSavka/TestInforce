using API.Data;
using API.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UrlRepository:IUrlRepository
{
    private readonly MyDbContext _context;

    public UrlRepository(MyDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<URL>> GetAllUrls()
    {
        return await _context.Urls.ToListAsync();
    }

    public async Task<URL> GetUrlById(int id)
    {
        return await _context.Urls
            .Include(u => u.CreatedBy) // Eagerly load the CreatedBy property
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddUrlAsync(URL url)
    {
        await _context.Urls.AddAsync(url);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUrlAsync(URL url)
    {
        var existingUrl = await _context.Urls.FindAsync(url.Id);
        if (existingUrl != null)
        {
            _context.Urls.Update(url);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteUrlAsync(URL url)
    {
        var existingUrl = await _context.Urls.FindAsync(url.Id);
        if (existingUrl != null)
        {
            _context.Urls.Remove(existingUrl);
            await _context.SaveChangesAsync();
        }
    }
}
