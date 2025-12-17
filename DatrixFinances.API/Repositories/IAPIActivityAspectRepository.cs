
using DatrixFinances.API.Models.DTO;

namespace DatrixFinances.API.Repositories;

public interface IAPIActivityAspectRepository
{
    Task Add(APIActivityAspect activity);
    Task<List<long>> All();
    Task<APIActivityAspect> Get(long id);
}