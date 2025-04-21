using IBTSS.Service.DTO.Request.Bus;
using IBTSS.Service.DTO.Response.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.BusService
{
    public interface IBusService
    {
        Task<BusResponse> AddAsync(BusRequest request);
        Task<BusResponse?> UpdateAsync(string id, BusUpdateRequest request);
        Task<BusResponse?> GetByIdAsync(string id);
        Task<List<BusResponse>> GetAllAsync();
    }
}
