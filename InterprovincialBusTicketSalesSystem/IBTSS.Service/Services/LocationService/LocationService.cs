using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Location;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.LocationService
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LocationResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Locations.GetAllAsync();
            return list.Select(x => new LocationResponse
            {
                LocationId = x.LocationId,
                LocationName = x.LocationName,             
                IsDelete = x.IsDelete
            }).ToList();
        }

        public async Task<LocationResponse?> GetByIdAsync(string id)
        {
            var x = await _unitOfWork.Locations.GetByIdAsync(id);
            if (x == null) return null;

            return new LocationResponse
            {
                LocationId = x.LocationId,
                LocationName = x.LocationName,              
                IsDelete = x.IsDelete
            };
        }

        public async Task<LocationResponse> AddAsync(LocationRequest request)
        {
            var exists = (await _unitOfWork.Locations.GetAllAsync())
      .Any(x => x.LocationName.Equals(request.LocationName, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new Exception("Location name already exists.");
            var entity = new Location
            {
                LocationName = request.LocationName,           
                IsDelete = false
            };

            var created = await _unitOfWork.Locations.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return new LocationResponse
            {
                LocationId = created.LocationId,
                LocationName = created.LocationName,         
                IsDelete = created.IsDelete
            };
        }

        public async Task<LocationResponse?> UpdateAsync(string id, LocationRequest request)
        {
            var existing = await _unitOfWork.Locations.GetByIdAsync(id);
            if (existing == null) return null;

            existing.LocationName = request.LocationName;            

            var updated = await _unitOfWork.Locations.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return new LocationResponse
            {
                LocationId = updated.LocationId,
                LocationName = updated.LocationName,            
                IsDelete = updated.IsDelete
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var deleted = await _unitOfWork.Locations.DeleteAsync(id);
            if (deleted) await _unitOfWork.CompleteAsync();
            return deleted;
        }
        public async Task<(List<LocationResponse>, int)> GetFilteredAsync(QueryParameters query)
        {
            var locations = await _unitOfWork.Locations.GetAllAsync();
            var filtered = locations.AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                filtered = filtered.Where(l =>
                    !string.IsNullOrEmpty(l.LocationName) &&
                    l.LocationName.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase));
            }

            // 🔽 Sort by LocationName
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                if (query.SortBy.Equals("LocationName", StringComparison.OrdinalIgnoreCase))
                {
                    filtered = filtered.OrderBy(l => l.LocationName);
                }
                else if (query.SortBy.Equals("-LocationName", StringComparison.OrdinalIgnoreCase))
                {
                    filtered = filtered.OrderByDescending(l => l.LocationName);
                }
            }
            var total = filtered.Count();

            if (query.PageSize == -1)
            {
                var allMapped = filtered.Select(x => new LocationResponse
                {
                    LocationId = x.LocationId,
                    LocationName = x.LocationName,
                    IsDelete = x.IsDelete
                }).ToList();
                return (allMapped, allMapped.Count);
            }

            var paged = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var mapped = paged.Select(x => new LocationResponse
            {
                LocationId = x.LocationId,
                LocationName = x.LocationName,
                IsDelete = x.IsDelete
            }).ToList();

            return (mapped, total);
        }

    }

}
