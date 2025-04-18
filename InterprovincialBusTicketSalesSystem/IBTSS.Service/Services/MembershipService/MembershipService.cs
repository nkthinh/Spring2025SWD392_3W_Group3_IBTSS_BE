using IBTSS.Repository.Entities;
using IBTSS.Repository.Repositories.MembershipRepository;
using IBTSS.Service.DTO.Request.Membership;
using IBTSS.Service.DTO.Response.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.MembershipService
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _repository;

        public MembershipService(IMembershipRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MembershipResponse>> GetAllAsync()
        {
            var memberships = await _repository.GetAllAsync();
            return memberships.Select(m => new MembershipResponse
            {
                MembershipId = m.MembershipId,
                RankName = m.RankName,
                MinTicketsRequired = m.MinTicketsRequired,
                DiscountRate = m.DiscountRate,
                Description = m.Description
            }).ToList();
        }

        public async Task<MembershipResponse?> GetByIdAsync(string id)
        {
            var m = await _repository.GetByIdAsync(id);
            if (m == null) return null;
            return new MembershipResponse
            {
                MembershipId = m.MembershipId,
                RankName = m.RankName,
                MinTicketsRequired = m.MinTicketsRequired,
                DiscountRate = m.DiscountRate,
                Description = m.Description
            };
        }

        public async Task<MembershipResponse> AddAsync(MembershipRequest request)
        {
            var m = new Membership
            {
                RankName = request.RankName,
                MinTicketsRequired = request.MinTicketsRequired,
                DiscountRate = request.DiscountRate,
                Description = request.Description
            };

            var created = await _repository.AddAsync(m);

            return new MembershipResponse
            {
                MembershipId = created.MembershipId,
                RankName = created.RankName,
                MinTicketsRequired = created.MinTicketsRequired,
                DiscountRate = created.DiscountRate,
                Description = created.Description
            };
        }

        public async Task<MembershipResponse?> UpdateAsync(string id, MembershipRequest request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.RankName = request.RankName;
            existing.MinTicketsRequired = request.MinTicketsRequired;
            existing.DiscountRate = request.DiscountRate;
            existing.Description = request.Description;

            var updated = await _repository.UpdateAsync(existing);

            return new MembershipResponse
            {
                MembershipId = updated.MembershipId,
                RankName = updated.RankName,
                MinTicketsRequired = updated.MinTicketsRequired,
                DiscountRate = updated.DiscountRate,
                Description = updated.Description
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }

}
