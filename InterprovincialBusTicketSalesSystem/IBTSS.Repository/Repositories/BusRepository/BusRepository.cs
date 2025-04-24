using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.BusRepository
{
    public class BusRepository : IBusRepository
    {
        private readonly AppDbContext _context;

        public BusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Bus>> GetAllAsync()
        {
            return await _context.Buses
                .Include(b => b.Seats)
                .ToListAsync();
        }

        public async Task<Bus?> GetByIdAsync(string id)
        {
            return await _context.Buses
                .Include(b => b.Seats)
                .FirstOrDefaultAsync(b => b.BusId == id && !b.IsDelete);
        }

        public async Task<Bus> AddAsync(Bus bus)
        {
            await _context.Buses.AddAsync(bus);
            return bus;
        }

        public Task<Bus> UpdateAsync(Bus bus)
        {
            _context.Buses.Update(bus);
            return Task.FromResult(bus);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null) return false;
            bus.IsDelete = true;
            return true;
        }
    }
}
