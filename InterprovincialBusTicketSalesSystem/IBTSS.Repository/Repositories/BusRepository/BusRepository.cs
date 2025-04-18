using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.BusRepository
{
    public class BusRepository: IBusRepository
    {
        private readonly AppDbContext _context;
        public BusRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
