using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IStateRepository : IRepository<State>
    {
        Task UpdateAsync(State entity);
    }
}
