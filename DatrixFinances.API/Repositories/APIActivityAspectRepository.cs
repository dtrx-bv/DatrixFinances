using DatrixFinances.API.Models.DTO;
using DatrixFinances.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace DatrixFinances.API.Repositories;

public class APIActivityAspectRepository(DatabaseContext context) : IAPIActivityAspectRepository
{

    private readonly DatabaseContext _context = context;
    
    public async Task Add(APIActivityAspect activity)
    {
        _context.APIActivityLogs.Add(activity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<long>> All()
    {
        return await _context.APIActivityLogs
            .Select(a => a.Id)
            .ToListAsync();
    }

    public async Task<APIActivityAspect> Get(long id)
    {
        return await _context.APIActivityLogs
            .FirstOrDefaultAsync(a => a.Id == id) ?? new();
    }
}