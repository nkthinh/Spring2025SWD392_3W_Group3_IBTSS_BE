using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.Repositories.RouteRepository;
using IBTSS.Service.DTO.Request.Route;
using IBTSS.Service.DTO.Response.Route;
using IBTSS.Service.DTO.Response.Transaction;

namespace IBTSS.Service.Services.RouteService
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _repository;
        private readonly IMapper _mapper;
        public RouteService(IRouteRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

    }
}
