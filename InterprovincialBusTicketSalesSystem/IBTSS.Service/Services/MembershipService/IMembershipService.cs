using IBTSS.Service.DTO.Request.Membership;
using IBTSS.Service.DTO.Response.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.MembershipService
{
    public interface IMembershipService
    {
        Task<List<MembershipResponse>> GetAllAsync();
        Task<MembershipResponse?> GetByIdAsync(string id);
        Task<MembershipResponse> AddAsync(MembershipRequest request);
        Task<MembershipResponse?> UpdateAsync(string id, MembershipRequest request);
        Task<bool> DeleteAsync(string id);
    }

}
