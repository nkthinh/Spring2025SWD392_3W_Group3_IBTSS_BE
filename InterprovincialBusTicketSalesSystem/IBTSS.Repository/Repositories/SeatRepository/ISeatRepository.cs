using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.SeatRepository
{
    public interface ISeatRepository
    {
        Task<List<Seat>> GetAllAsync();
        Task<Seat?> GetByIdAsync(string id);
        Task<Seat> AddAsync(Seat seat);
        Task<Seat> UpdateAsync(Seat seat);
        Task<bool> DeleteAsync(string id);
    }
}
