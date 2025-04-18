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
            await _context.Memberships.Where(m => !m.IsDelete).ToListAsync();

        public async Task<Membership?> GetByIdAsync(string id) =>
            await _context.Memberships.FirstOrDefaultAsync(m => m.MembershipId == id && !m.IsDelete);

        public async Task<Membership> AddAsync(Membership membership)
        {
            membership.MembershipId = Guid.NewGuid().ToString();
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
