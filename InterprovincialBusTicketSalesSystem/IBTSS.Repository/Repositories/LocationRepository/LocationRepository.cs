using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.LocationRepository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;
        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }    
    public async Task<List<Location>> GetAllAsync() =>
        await _context.Locations.Where(l => !l.IsDelete).ToListAsync();

        public async Task<Location?> GetByIdAsync(string id) =>
            await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == id && !l.IsDelete);

        public async Task<Location> AddAsync(Location location)
        {
            var last = await _context.Locations.OrderByDescending(x => x.LocationId).FirstOrDefaultAsync();
            string newId = "L001";
            if (last != null)
            {
                int number = int.Parse(last.LocationId.Substring(1)) + 1;
                newId = "L" + number.ToString("D3");
            }
            location.LocationId = newId;

            await _context.Locations.AddAsync(location);
            return location;
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            return location;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            location.IsDelete = true;
            return true;
        }
    }
}