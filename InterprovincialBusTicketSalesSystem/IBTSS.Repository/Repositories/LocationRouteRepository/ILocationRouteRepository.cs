using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.LocationRouteRepository
{
    public interface ILocationRouteRepository
    {
        Task<List<LocationRoute>> GetAllAsync();
        Task<LocationRoute?> GetByIdAsync(string locationRouteId);
        Task<List<LocationRoute>> GetByRouteIdAsync(string routeId);
        Task<LocationRoute> AddAsync(LocationRoute locationRoute);
        Task<LocationRoute> UpdateAsync(LocationRoute locationRoute);
        Task<bool> DeleteAsync(string locationRouteId);
    }
}
