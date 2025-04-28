using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.Repositories.RouteRepository;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Route;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Membership;
using IBTSS.Service.DTO.Response.Route;
using IBTSS.Service.DTO.Response.Transaction;

namespace IBTSS.Service.Services.RouteService
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
   

        public RouteService(IRouteRepository repository, IMapper mapper,IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<RouteResponse>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return _mapper.Map<List<RouteResponse>>(data);
        }
        public async Task<RouteResponse?> GetByIdAsync(string id)
        {
            var route = await _repository.GetByIdAsync(id);
            return route == null ? null : _mapper.Map<RouteResponse>(route);
        }
        public async Task<RouteResponse> AddAsync(RouteRequest request)
        {
            var exists = (await _repository.GetAllAsync())
        .Any(r => r.RouteName.Equals(request.RouteName, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new Exception("Route name already exists.");

            var route = _mapper.Map<Route>(request);
            var newRoute = await _repository.AddAsync(route);
            return _mapper.Map<RouteResponse>(newRoute);
        }
        public async Task<RouteResponse> UpdateAsync(string id,RouteRequest request)
        {
            var updated = await _repository.UpdateAsync(id, _mapper.Map<Route>(request));
            return updated == null ? null : _mapper.Map<RouteResponse>(updated);
        }
        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
       public async Task<(List<RouteResponse>, int)> GetFilteredAsync(QueryParameters query)
{
    var routes = await _repository.GetAllAsync();
    var filtered = routes.AsQueryable();

    // Lọc theo từ khóa nếu có
    if (!string.IsNullOrEmpty(query.Keyword))
    {
        filtered = filtered.Where(r =>
            !string.IsNullOrEmpty(r.RouteName) &&
            r.RouteName.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase));
    }

    // Sắp xếp
    filtered = query.SortBy switch
    {
        "name_desc" => filtered.OrderByDescending(r => r.RouteName),
        _ => filtered.OrderBy(r => r.RouteName) // mặc định là tăng dần
    };

    var total = filtered.Count();

    // ✅ Nếu PageSize = -1: trả toàn bộ
    if (query.PageSize == -1)
    {
        var allMapped = _mapper.Map<List<RouteResponse>>(filtered.ToList());
        return (allMapped, allMapped.Count);
    }

    var paged = filtered
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToList();

    var mapped = _mapper.Map<List<RouteResponse>>(paged);
    return (mapped, total);
}


    }
}
