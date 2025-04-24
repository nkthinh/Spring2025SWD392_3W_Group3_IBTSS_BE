using IBTSS.Service.DTO.Request.Seat;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.SeatService
{
    public interface ISeatService
    {
        Task<List<SeatResponse>> GetAllAsync();
        Task<SeatResponse?> GetByIdAsync(string id);
        Task<SeatResponse> AddAsync(SeatRequest request);
        Task<SeatResponse?> UpdateAsync(string id, SeatRequest request);
        Task<bool> DeleteAsync(string id);
        Task<SeatSummaryResponse> GetSeatAvailabilityByTripIdAsync(string tripId);
        Task<(List<SeatResponse>, int)> GetFilteredAsync(QueryParameters query);

    }
}
