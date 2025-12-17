using DatrixFinances.API.Models.DTO;
using DatrixFinances.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace DatrixFinances.API.Repositories;

public class UserRepository(DatabaseContext context) : IUserRepository
{

    private readonly DatabaseContext _context = context;

    public async Task<User> GetUserByCredentials(string clientId, string clientSecret)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ClientId == clientId && u.ClientSecret == clientSecret) ?? null!;
    }

    public async Task Update(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}