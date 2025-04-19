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
    }
}
