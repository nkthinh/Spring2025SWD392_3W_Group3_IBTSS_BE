using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.TicketRepository
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(string id);
        Task AddAsync(Ticket ticket);
        Task AddRangeAsync(List<Ticket> tickets);
        Task UpdateAsync(Ticket ticket);
        Task<bool> DeleteAsync(string id);
    }
}
