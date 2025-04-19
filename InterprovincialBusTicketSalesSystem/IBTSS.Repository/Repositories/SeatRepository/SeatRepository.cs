using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.SeatRepository
{
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;
        public SeatRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Seat>> GetAllAsync() =>
            await _context.Seats.Where(s => !s.IsDelete).ToListAsync();

        public async Task<Seat?> GetByIdAsync(string id) =>
            await _context.Seats.FirstOrDefaultAsync(s => s.SeatId == id && !s.IsDelete);

        public async Task<Seat> AddAsync(Seat seat)
        {
            var lastSeat = await _context.Seats.OrderByDescending(s => s.SeatId).FirstOrDefaultAsync();
            string newId = "S001";
            if (lastSeat != null)
            {
                int number = int.Parse(lastSeat.SeatId.Substring(1));
                newId = "S" + (number + 1).ToString("D3");
            }
            seat.SeatId = newId;
            await _context.Seats.AddAsync(seat);
            await _context.SaveChangesAsync();
            return seat;
        }

        public async Task<Seat> UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
            return seat;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat == null) return false;
            seat.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
