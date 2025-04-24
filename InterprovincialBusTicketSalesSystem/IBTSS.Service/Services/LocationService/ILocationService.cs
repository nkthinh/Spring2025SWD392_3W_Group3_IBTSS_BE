using IBTSS.Service.DTO.Request.Location;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.LocationService
{
    public interface ILocationService
    {
        Task<List<LocationResponse>> GetAllAsync();
        Task<LocationResponse?> GetByIdAsync(string id);
        Task<LocationResponse> AddAsync(LocationRequest request);
        Task<LocationResponse?> UpdateAsync(string id, LocationRequest request);
        Task<bool> DeleteAsync(string id);
        Task<(List<LocationResponse>, int)> GetFilteredAsync(QueryParameters query);

    }

}
