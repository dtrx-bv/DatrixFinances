using DatrixFinances.API.Models.DTO;

namespace DatrixFinances.API.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByCredentials(string clientId, string clientSecret);
    Task Update(User user);
}