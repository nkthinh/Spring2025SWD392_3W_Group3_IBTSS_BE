using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
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
        // Get all LocationRoutes
        public async Task<List<LocationRoute>> GetAllAsync()
        {
            return await _context.LocationRoutes
                                 .Include(lr => lr.Location)  // Include Location details
                                 .Include(lr => lr.Route)     // Include Route details
                                 .Where(lr => !lr.Location.IsDelete)  // Exclude deleted Locations
                                 .ToListAsync();
        }

        // Get a LocationRoute by its ID
        public async Task<LocationRoute?> GetByIdAsync(string locationRouteId)
        {
            return await _context.LocationRoutes
                                 .Include(lr => lr.Location)
                                 .Include(lr => lr.Route)
                                 .FirstOrDefaultAsync(lr => lr.LocationRouteId == locationRouteId && !lr.Location.IsDelete);
        }
        public async Task<List<LocationRoute>> GetByRouteIdAsync(string routeId)
        {
            return await _context.LocationRoutes
                                 .Include(lr => lr.Location)
                                 .Where(lr => lr.RouteId == routeId)
                                 .OrderBy(lr => lr.StopOrder)
                                 .ToListAsync();
        }


        // Add a new LocationRoute
        public async Task<LocationRoute> AddAsync(LocationRoute locationRoute)
        {
            // Generate a new LocationRouteId
            var lastLocationRoute = await _context.LocationRoutes
                .OrderByDescending(x => x.LocationRouteId)
                .FirstOrDefaultAsync();

            string newId = "LR001";
            if (lastLocationRoute != null)
            {
                int number = int.Parse(lastLocationRoute.LocationRouteId.Substring(2)) + 1;
                newId = "LR" + number.ToString("D3");
            }
            locationRoute.LocationRouteId = newId;

            await _context.LocationRoutes.AddAsync(locationRoute);
            return locationRoute;
        }

        // Update an existing LocationRoute
        public async Task<LocationRoute> UpdateAsync(LocationRoute locationRoute)
        {
            _context.LocationRoutes.Update(locationRoute);
            return locationRoute;
        }

        // Delete a LocationRoute by setting IsDelete to true (soft delete)
        public async Task<bool> DeleteAsync(string locationRouteId)
        {
            var locationRoute = await _context.LocationRoutes.FindAsync(locationRouteId);
            if (locationRoute == null) return false;

      
            _context.LocationRoutes.Update(locationRoute);
            return true;
        }

        // Soft delete all related LocationRoutes for a specific Location (optional)
        public async Task<bool> DeleteByLocationAsync(string locationId)
        {
            var locationRoutes = await _context.LocationRoutes
                                                .Where(lr => lr.LocationId == locationId)
                                                .ToListAsync();

            foreach (var locationRoute in locationRoutes)
            {
     
                _context.LocationRoutes.Update(locationRoute);
            }

            return true;
        }
    }
}
