using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.MembershipRepository
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly AppDbContext _context;

        public MembershipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Membership>> GetAllAsync() =>
            await _context.Memberships.ToListAsync();

        public async Task<Membership?> GetByIdAsync(string id) =>
            await _context.Memberships.FirstOrDefaultAsync(m => m.MembershipId == id && !m.IsDelete);
        public async Task<Membership> AddAsync(Membership membership)
        {
            // Lấy MembershipId lớn nhất đang có
            var lastMembership = await _context.Memberships
                .OrderByDescending(m => m.MembershipId)
                .FirstOrDefaultAsync();

            int nextId = 1;

            if (lastMembership != null && int.TryParse(lastMembership.MembershipId, out int currentId))
            {
                nextId = currentId + 1;
            }

            membership.MembershipId = nextId.ToString("D3"); // format "001", "002", ...

            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }


        public async Task<Membership> UpdateAsync(Membership membership)
        {
            _context.Memberships.Update(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var membership = await _context.Memberships.FindAsync(id);
            if (membership == null) return false;
            membership.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
