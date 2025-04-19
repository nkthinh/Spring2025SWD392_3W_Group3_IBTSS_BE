using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.TripRepository
{
    public class TripRepository : ITripRepository
    {
        private readonly AppDbContext _context;
        public TripRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Trip>> GetAllAsync() =>
          await _context.Trips.Where(m => !m.IsDelete).ToListAsync();

        public async Task<Trip?> GetByIdAsync(string id) =>
            await _context.Trips.FirstOrDefaultAsync(m => m.TripId == id && !m.IsDelete);
        public async Task<Trip> AddAsync(Trip Trip)
        {
            // Tìm TripId lớn nhất (theo định dạng T001, T002, ...)
            var lastTrip = await _context.Trips
                .OrderByDescending(u => u.TripId)
                .FirstOrDefaultAsync();

            string newId = "T001";

            if (lastTrip != null)
            {
                string lastId = lastTrip.TripId; // ví dụ: "U005"
                int number = int.Parse(lastId.Substring(1)); // bỏ "U" => 5
                number++;
                newId = "T" + number.ToString("D3"); // "U006"
            }

            Trip.TripId = newId;

            await _context.Trips.AddAsync(Trip);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
            return Trip;
        }


        public async Task<Trip> UpdateAsync(Trip Trip)
        {
            _context.Trips.Update(Trip);
            await _context.SaveChangesAsync();
            return Trip;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var Trip = await _context.Trips.FindAsync(id);
            if (Trip == null) return false;
            Trip.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Trip>> SearchTripsByDateAsync(string date)
        {
            return await _context.Trips
                .Where(t => !t.IsDelete && t.Date == date)
                .Include(t => t.Route)
                    .ThenInclude(r => r.LocationRoutes)
                        .ThenInclude(lr => lr.Location)
                .ToListAsync();
        }
    }
}
