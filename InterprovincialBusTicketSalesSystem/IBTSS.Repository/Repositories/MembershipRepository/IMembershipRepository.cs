using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.MembershipRepository
{
    public interface IMembershipRepository
    {
        Task<List<Membership>> GetAllAsync();
        Task<Membership?> GetByIdAsync(string id);
        Task<Membership> AddAsync(Membership membership);
        Task<Membership> UpdateAsync(Membership membership);
        Task<bool> DeleteAsync(string id);
    }

}
