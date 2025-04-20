using IBTSS.Repository.Entities;
using IBTSS.Repository.Enum;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.LocationRoute;
using IBTSS.Service.DTO.Response.Trip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.TripService
{
    public class TripService: ITripService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TripResponse>> GetAllAsync()
        {
            var Trips = await _unitOfWork.Trips.GetAllAsync();
            return Trips.Select(m => new TripResponse
            {
                TripId = m.TripId,
               RouteId = m.RouteId,
               BusId = m.BusId,
               DriverId = m.DriverId,
               DepartureTime = m.DepartureTime,
               Date = m.Date,
               Direction = m.Direction,
               IsDelete = m.IsDelete,
               Price = m.Price,
               Status = m.Status,
            }).ToList();
        }

        public async Task<TripResponse?> GetByIdAsync(string id)
        {
            var m = await _unitOfWork.Trips.GetByIdAsync(id);
            if (m == null) return null;

            return new TripResponse
            {
                TripId = m.TripId,
                RouteId = m.RouteId,
                BusId = m.BusId,
                DriverId = m.DriverId,
                DepartureTime = m.DepartureTime,
                Date = m.Date,
                Direction = m.Direction,
                IsDelete = m.IsDelete,
                Price = m.Price,
                Status = m.Status,
            };
        }

        public async Task<TripResponse> AddAsync(TripRequest request)
        {
            // 1. Kiểm tra thông tin Driver
            var driver = await _unitOfWork.Users.GetByIdAsync(request.DriverId);
            if (driver == null || driver.Role != UserRole.Driver)
                throw new Exception($"User with ID {request.DriverId} is invalid or not a driver.");

            // 2. Tạo Trip
            var trip = new Trip
            {
                RouteId = request.RouteId,
                BusId = request.BusId,
                DriverId = request.DriverId,
                DepartureTime = request.DepartureTime,
                Date = request.Date,
                Direction = request.Direction,
                IsDelete = false,
                Price = request.Price,
                Status = request.Status,
            };

            var createdTrip = await _unitOfWork.Trips.AddAsync(trip);

            // 3. Tạo các LocationRoute cho chuyến đi
            int stopOrder = 1; // Bắt đầu từ điểm dừng số 1
            foreach (var lr in request.LocationRoutes)
            {
                // Kiểm tra LocationId có tồn tại hay không
                var locationExists = await _unitOfWork.Locations.GetByIdAsync(lr.LocationId);
                if (locationExists == null)
                    throw new Exception($"Location with ID {lr.LocationId} does not exist.");

                var locationRoute = new LocationRoute
                {
                    RouteId = createdTrip.RouteId,
                    LocationId = lr.LocationId,
                    StopOrder = stopOrder++, // Tăng thứ tự điểm dừng
                    StopDuration = TimeSpan.FromMinutes(lr.StopDurationMinutes) // Thời gian nghỉ tại điểm dừng
                };

                await _unitOfWork.LocationRoutes.AddAsync(locationRoute);
            }

            await _unitOfWork.CompleteAsync();

            // 4. Lấy thông tin các LocationRoute đã tạo cho chuyến đi và trả về thông tin
         var locationRouteList = await _unitOfWork.LocationRoutes.GetByRouteIdAsync(createdTrip.RouteId);

var locationRoutes = locationRouteList.Select(lr => new LocationRouteResponse
{
    LocationId = lr.LocationId,
    LocationName = lr.Location?.LocationName ?? "", // hoặc lr.Location.Name
    StopOrder = lr.StopOrder,
    StopDuration = lr.StopDuration
}).ToList();

            return new TripResponse
            {
                TripId = createdTrip.TripId,
                RouteId = createdTrip.RouteId,
                BusId = createdTrip.BusId,
                DriverId = createdTrip.DriverId,
                DepartureTime = createdTrip.DepartureTime,
                Date = createdTrip.Date,
                Direction = createdTrip.Direction,
                IsDelete = createdTrip.IsDelete,
                Price = createdTrip.Price,
                Status = createdTrip.Status,
                LocationRoutes = locationRoutes // Trả về các điểm dừng cho chuyến đi
            };
        }


        public async Task<TripResponse?> UpdateAsync(string id, TripRequest request)
        {
            var existing = await _unitOfWork.Trips.GetByIdAsync(id);
            if (existing == null) return null;

            existing.RouteId = request.RouteId;
            existing.BusId = request.BusId;
            existing.DriverId = request.DriverId;
            existing.DepartureTime = request.DepartureTime;
            existing.Date = request.Date;
            existing.Direction = request.Direction;
            existing.IsDelete = request.IsDelete;
            existing.Price = request.Price;
            existing.Status = request.Status;


            var updated = await _unitOfWork.Trips.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return new TripResponse
            {
               TripId = updated.TripId,
                RouteId = updated.RouteId,
                BusId = updated.BusId,
                DriverId = updated.DriverId,
                DepartureTime = updated.DepartureTime,
                Date = updated.Date,
                Direction = updated.Direction,
                IsDelete = updated.IsDelete,
                Price = updated.Price,
                Status = updated.Status,
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var deleted = await _unitOfWork.Trips.DeleteAsync(id);
            if (deleted) await _unitOfWork.CompleteAsync();
            return deleted;
        }
        public async Task<IEnumerable<TripSearchDto>> SearchByDateAsync(string date)
        {
            var trips = await _unitOfWork.Trips.SearchTripsByDateAsync(date);

            return trips.Select(t => new TripSearchDto
            {
                RouteName = t.Route?.RouteName ?? "",
                DepartureTime = DateTime.Today.Add(t.DepartureTime.ToTimeSpan()), // chuyển TimeOnly
                Date = t.Date,
                Price = t.Price,
                Stops = t.Route?.LocationRoutes?
                    .OrderBy(lr => lr.StopOrder)
                    .Select(lr => new LocationStopDto
                    {
                        LocationName = lr.Location?.LocationName ?? "",
                        StopOrder = lr.StopOrder,
                        StopDuration = lr.StopDuration
                    }).ToList() ?? new List<LocationStopDto>()
            });
        }
        //public async Task<IEnumerable<TripSearchDto>> SearchByLocationAndRouteAsync(string locationName, string routeName)
        //{
        //    var trips = await _unitOfWork.Trips.SearchTripsByLocationAndRouteAsync(locationName, routeName);

        //    return trips.Select(t => new TripSearchDto
        //    {
        //        RouteName = t.Route?.RouteName ?? "",
        //        DepartureTime = DateTime.Today.Add(t.DepartureTime.ToTimeSpan()),
        //        Date = t.Date,
        //        Price = t.Price,
        //        Stops = t.Route?.LocationRoutes?
        //            .OrderBy(lr => lr.StopOrder)
        //            .Select(lr => new LocationStopDto
        //            {
        //                LocationName = lr.Location?.LocationName ?? "",
        //                StopOrder = lr.StopOrder,
        //                StopDuration = lr.StopDuration
        //            }).ToList() ?? new List<LocationStopDto>()
        //    });
        //}
        public async Task<IEnumerable<TripSearchDto>> SearchByKeywordAsync(string keyword)
        {
            // Tìm theo RouteName
            var tripsByRoute = await _unitOfWork.Trips.SearchTripsByRouteNameAsync(keyword);
            if (tripsByRoute.Any())
            {
                return tripsByRoute.Select(t => ConvertTripToDto(t));
            }

            // Nếu không thấy route phù hợp, thử LocationName
            var tripsByLocation = await _unitOfWork.Trips.SearchTripsByLocationNameAsync(keyword);
            return tripsByLocation.Select(t => ConvertTripToDto(t));
        }

        private TripSearchDto ConvertTripToDto(Trip t)
        {
            return new TripSearchDto
            {
                RouteName = t.Route?.RouteName ?? "",
                DepartureTime = DateTime.Today.Add(t.DepartureTime.ToTimeSpan()),
                Date = t.Date,
                Price = t.Price,
                Stops = t.Route?.LocationRoutes?
                    .OrderBy(lr => lr.StopOrder)
                    .Select(lr => new LocationStopDto
                    {
                        LocationName = lr.Location?.LocationName ?? "",
                        StopOrder = lr.StopOrder,
                        StopDuration = lr.StopDuration
                    }).ToList() ?? new List<LocationStopDto>()
            };
        }

    }
}

