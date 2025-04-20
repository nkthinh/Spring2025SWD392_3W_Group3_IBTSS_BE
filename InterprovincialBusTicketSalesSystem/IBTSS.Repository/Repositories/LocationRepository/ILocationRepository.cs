using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.LocationRepository
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(string id);
        Task<Location> AddAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task<bool> DeleteAsync(string id);
    }

}
