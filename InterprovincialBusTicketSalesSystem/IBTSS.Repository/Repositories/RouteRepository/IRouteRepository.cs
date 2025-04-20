using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.RouteRepository
{
    public interface IRouteRepository
    {
        Task<List<Route>> GetAllAsync();
        Task<Route?> GetByIdAsync(string id);
        Task<Route> AddAsync(Route route);
        Task<Route?> UpdateAsync(string id, Route route);
        Task<bool> DeleteAsync(string id);
    }
}
