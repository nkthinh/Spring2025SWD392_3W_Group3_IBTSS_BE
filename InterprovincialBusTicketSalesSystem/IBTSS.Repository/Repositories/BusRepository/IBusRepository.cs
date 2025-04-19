using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.BusRepository
{
    public interface IBusRepository
    {
        Task<List<Bus>> GetAllAsync();
        Task<Bus?> GetByIdAsync(string id);
        Task<Bus> AddAsync(Bus bus);
        public Task<Bus> UpdateAsync(Bus bus);
        Task<bool> DeleteAsync(string id);
    }
}
