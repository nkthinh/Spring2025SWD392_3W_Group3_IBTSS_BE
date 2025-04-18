using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.LocationRouteRepository
{
    public class LocationRouteRepository : ILocationRouteRepository
    {
        private readonly AppDbContext _context;
        public LocationRouteRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
