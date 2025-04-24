using IBTSS.Service.DTO.Request.Route;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.RouteService
{
    public interface IRouteService
    {
        Task<List<RouteResponse>> GetAllAsync();
        Task<RouteResponse?> GetByIdAsync(string id);
        Task<RouteResponse> AddAsync(RouteRequest request);
        Task<RouteResponse> UpdateAsync(string id,RouteRequest request);
        Task<bool> DeleteAsync(string id);
        Task<(List<RouteResponse>, int)> GetFilteredAsync(QueryParameters query);
    }
}
