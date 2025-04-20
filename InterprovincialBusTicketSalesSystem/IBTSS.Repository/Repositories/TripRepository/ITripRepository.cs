using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.TripRepository
{
    public interface ITripRepository
    {
        Task<List<Trip>> GetAllAsync();
        Task<Trip?> GetByIdAsync(string id);
        Task<Trip> AddAsync(Trip trip);
        Task<Trip> UpdateAsync(Trip trip);
        Task<bool> DeleteAsync(string id);
        Task<List<Trip>> SearchTripsByDateAsync(string date);
        Task<List<Trip>> SearchTripsByLocationAndRouteAsync(string locationName, string routeName);
        Task<List<Trip>> SearchTripsByRouteNameAsync(string routeName);
        Task<List<Trip>> SearchTripsByLocationNameAsync(string locationName);

    }
}
