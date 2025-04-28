using IBTSS.Repository.Entities;
using IBTSS.Repository.Repositories.MembershipRepository;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Membership;
using IBTSS.Service.DTO.Request.Trip;
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
        private readonly IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MembershipResponse>> GetAllAsync()
        {
            var memberships = await _unitOfWork.Memberships.GetAllAsync();
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
            var m = await _unitOfWork.Memberships.GetByIdAsync(id);
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
            var exists = (await _unitOfWork.Memberships.GetAllAsync())
      .Any(m => m.RankName.Equals(request.RankName, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new Exception("Rank name already exists.");

            var m = new Membership
            {
                RankName = request.RankName,
                MinTicketsRequired = request.MinTicketsRequired,
                DiscountRate = request.DiscountRate,
                Description = request.Description
            };

            var created = await _unitOfWork.Memberships.AddAsync(m);
            await _unitOfWork.CompleteAsync();

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
            var existing = await _unitOfWork.Memberships.GetByIdAsync(id);
            if (existing == null) return null;

            existing.RankName = request.RankName;
            existing.MinTicketsRequired = request.MinTicketsRequired;
            existing.DiscountRate = request.DiscountRate;
            existing.Description = request.Description;

            var updated = await _unitOfWork.Memberships.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

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
            var deleted = await _unitOfWork.Memberships.DeleteAsync(id);
            if (deleted) await _unitOfWork.CompleteAsync();
            return deleted;
        }
        public async Task<(List<MembershipResponse>, int)> GetFilteredAsync(QueryParameters query)
        {
            var memberships = await _unitOfWork.Memberships.GetAllAsync();
            var filtered = memberships.AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                filtered = filtered.Where(m => m.RankName.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase));
            }

            filtered = query.SortBy switch
            {
                "rank_desc" => filtered.OrderByDescending(m => m.RankName),
                "minTickets" => filtered.OrderBy(m => m.MinTicketsRequired),
                _ => filtered.OrderBy(m => m.RankName)
            };

            var total = filtered.Count();

            if (query.PageSize == -1)
            {
                var allMapped = filtered.Select(m => new MembershipResponse
                {
                    MembershipId = m.MembershipId,
                    RankName = m.RankName,
                    MinTicketsRequired = m.MinTicketsRequired,
                    DiscountRate = m.DiscountRate,
                    Description = m.Description
                }).ToList();

                return (allMapped, allMapped.Count);
            }

            var paged = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var mapped = paged.Select(m => new MembershipResponse
            {
                MembershipId = m.MembershipId,
                RankName = m.RankName,
                MinTicketsRequired = m.MinTicketsRequired,
                DiscountRate = m.DiscountRate,
                Description = m.Description
            }).ToList();

            return (mapped, total);
        }

    }

}
