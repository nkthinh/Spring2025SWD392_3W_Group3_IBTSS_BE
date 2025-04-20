using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBTSS.Repository.Repositories.RouteRepository
{
    public class RouteRepository : IRouteRepository
    {
        private readonly AppDbContext _context;

        public RouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Route>> GetAllAsync()
        {
            return await _context.Routes
                .Where(r => !r.IsDelete)
                .Include(r => r.LocationRoutes)
                .Include(r => r.Trips)
                .ToListAsync();
        }

        public async Task<Route?> GetByIdAsync(string id)
        {
            return await _context.Routes
                .Include(r => r.LocationRoutes)
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.RouteId == id && !r.IsDelete);
        }

        public async Task<Route> AddAsync(Route route)
        {
            var lastRoute = await _context.Routes.OrderByDescending(r => r.RouteId).FirstOrDefaultAsync();
            string newId = "R001";
            if (lastRoute != null)
            {
                int number = int.Parse(lastRoute.RouteId.Substring(1));
                newId = "R" + (number + 1).ToString("D3");
            }
            route.RouteId = newId;
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
            return route;
        }

        public async Task<Route?> UpdateAsync(string id, Route route)
        {
            var existing = await _context.Routes.FindAsync(id);
            if (existing == null || existing.IsDelete) return null;

            existing.RouteName = route.RouteName;
            existing.Distance = route.Distance;
            existing.EstimatedDuration = route.EstimatedDuration;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null || route.IsDelete) return false;

            route.IsDelete = true; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
