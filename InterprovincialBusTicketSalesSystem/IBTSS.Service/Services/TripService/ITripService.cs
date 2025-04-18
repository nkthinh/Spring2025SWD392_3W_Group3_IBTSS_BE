using IBTSS.Service.DTO.Request.Trip;
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
    }
}
