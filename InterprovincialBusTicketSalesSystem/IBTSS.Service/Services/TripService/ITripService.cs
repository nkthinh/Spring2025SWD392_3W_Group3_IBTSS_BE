using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.DTO.Response.Trip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.TripService
{
    public interface ITripService
    {
        Task<List<TripResponse>> GetAllAsync();
        Task<TripResponse?> GetByIdAsync(string id);
        Task<TripResponse> AddAsync(TripRequest request);
        Task<TripResponse?> UpdateAsync(string id, TripRequest request);
        Task<bool> DeleteAsync(string id);
        Task<List<TripSearchDto>> SearchTripsAsync(string keyword, string date, string type);

        Task<IEnumerable<TripSearchDto>> SearchByDateAsync(string date);
        Task<IEnumerable<TripSearchDto>> SearchByKeywordAsync(string keyword);
        //Task<IEnumerable<TripSearchDto>> SearchByLocationAndRouteAsync(string locationName, string routeName);
        Task<List<TripResponse>> GetTripsByDriverIdAsync(string driverId);
        Task<List<CustomerResponseByTrip>> GetCustomersByTripAsync(string tripId);

        Task<TripResponse?> CompleteTripAsync(string tripId);
        Task<TripResponse?> AssignDriverAsync(string tripId, string driverId);
        Task<List<object>> GetTripsForCalendarAsync();
    }
}
