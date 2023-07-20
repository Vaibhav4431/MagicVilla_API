using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task UpdateAsync(Country entity);
    }
}
