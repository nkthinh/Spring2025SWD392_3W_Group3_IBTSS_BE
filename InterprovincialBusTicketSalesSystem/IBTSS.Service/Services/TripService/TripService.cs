using IBTSS.Repository.Entities;
using IBTSS.Repository.Enum;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.DTO.Response.LocationRoute;
using IBTSS.Service.DTO.Response.Trip;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
namespace IBTSS.Service.Services.TripService
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TripResponse>> GetAllAsync()
        {
            var trips = await _unitOfWork.Trips.GetAllAsync();
            return trips.Select(m => new TripResponse
            {
                TripId = m.TripId,
                RouteId = m.RouteId,
                BusId = m.BusId,
                DriverId = m.DriverId,
                DepartureTime = m.DepartureTime.ToString("HH:mm"),
                Date = m.Date,
                Direction = m.Direction,
                IsDelete = m.IsDelete,
                Price = m.Price,
                Status = m.Status,
            }).ToList();
        }

        public async Task<TripResponse?> GetByIdAsync(string id)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(id);
            if (trip == null) return null;

            var route = trip.Route;
            var bus = trip.Bus;

            return new TripResponse
            {
                TripId = trip.TripId,
                RouteId = trip.RouteId,
                RouteName = route?.RouteName ?? "",
                BusId = trip.BusId,
                BusType = bus?.BusType ?? "",
                DriverId = trip.DriverId,
                DepartureTime = trip.DepartureTime.ToString("HH:mm"),
                Date = trip.Date,
                Direction = trip.Direction,
                IsDelete = trip.IsDelete,
                Price = trip.Price,
                Status = trip.Status,
                LocationRoutes = route?.LocationRoutes?
                    .OrderBy(lr => lr.StopOrder)
                    .Select(lr => new LocationRouteResponse
                    {
                        LocationId = lr.LocationId,
                        LocationName = lr.Location?.LocationName ?? "",
                        StopOrder = lr.StopOrder,
                        StopDuration = lr.StopDuration
                    }).ToList() ?? new()
            };
        }

        public async Task<TripResponse> AddAsync(TripRequest request)
        {
            // 1. Check driver hợp lệ
            var driver = await _unitOfWork.Users.GetByIdAsync(request.DriverId);
            if (driver == null || driver.Role != UserRole.Driver)
                throw new Exception($"User with ID {request.DriverId} is invalid or not a driver.");

            // 2. Parse thời gian khởi hành mới
            var newDeparture = TimeOnly.ParseExact(request.DepartureTime, "HH:mm", null);

            // 3. Lấy tất cả trips cùng ngày của driver để kiểm tra xung đột
            var existingTrips = await _unitOfWork.Trips.GetAllAsync();

            // 3a. Xung đột nếu cùng bus & driver, chênh lệch < 60 phút
            bool conflictBusDriver = existingTrips.Any(t =>
                t.BusId == request.BusId &&
                t.DriverId == request.DriverId &&
                t.Date == request.Date &&
                Math.Abs((t.DepartureTime.ToTimeSpan() - newDeparture.ToTimeSpan()).TotalHours) < 6
            );

            // 3b. Xung đột nếu cùng driver, cùng ngày, cùng giờ (dù khác bus)
            bool conflictDriverTime = existingTrips.Any(t =>
                t.DriverId == request.DriverId &&
                t.Date == request.Date &&
                Math.Abs((t.DepartureTime.ToTimeSpan() - newDeparture.ToTimeSpan()).TotalHours) < 6
            );
            // 3c. Xung đột nếu cùng bus, cùng ngày, cùng giờ (dù khác driver)
            bool conflictBusTime = existingTrips.Any(t =>
                t.BusId == request.BusId &&
                t.Date == request.Date &&
                Math.Abs((t.DepartureTime.ToTimeSpan() - newDeparture.ToTimeSpan()).TotalHours) < 6
            );

            if (conflictBusDriver || conflictDriverTime || conflictBusTime)
                throw new Exception(
                    "Cannot create trip: conflict detected (either same bus & driver within 6h, or same driver at exact same time)."
                );

            // 4. Tạo entity Trip
            var trip = new Trip
            {
                TripId = Guid.NewGuid().ToString(),
                RouteId = request.RouteId,
                BusId = request.BusId,
                DriverId = request.DriverId,
                DepartureTime = newDeparture,
                Date = string.IsNullOrEmpty(request.Date)
            ? DateTime.UtcNow.ToString("yyyy-MM-dd") // ✅ Nếu rỗng thì tự động gán ngày hiện tại
            : request.Date,
                Direction = request.Direction,
                IsDelete = false,
                Price = request.Price,
                Status = request.Status,
                Tickets = new List<Ticket>()
            };


            var createdTrip = await _unitOfWork.Trips.AddAsync(trip);

            // 5. Tạo các LocationRoute
            int stopOrder = 1;
            foreach (var lr in request.LocationRoutes)
            {
                var loc = await _unitOfWork.Locations.GetByIdAsync(lr.LocationId);
                if (loc == null)
                    throw new Exception($"Location with ID {lr.LocationId} does not exist.");

                var locationRoute = new LocationRoute
                {
                    LocationRouteId = Guid.NewGuid().ToString(),
                    RouteId = createdTrip.RouteId,
                    LocationId = lr.LocationId,
                    StopOrder = stopOrder++,
                    StopDuration = TimeSpan.FromMinutes(lr.StopDurationMinutes)
                };
                await _unitOfWork.LocationRoutes.AddAsync(locationRoute);
            }

            // 6. Lưu và trả về response
            await _unitOfWork.CompleteAsync();

            var lrList = await _unitOfWork.LocationRoutes.GetByRouteIdAsync(createdTrip.RouteId);
            var locationRoutes = lrList
                .OrderBy(lr => lr.StopOrder)
                .Select(lr => new LocationRouteResponse
                {
                    LocationId = lr.LocationId,
                    LocationName = lr.Location?.LocationName ?? "",
                    StopOrder = lr.StopOrder,
                    StopDuration = lr.StopDuration
                })
                .ToList();

            return new TripResponse
            {
                TripId = createdTrip.TripId,
                RouteId = createdTrip.RouteId,
                BusId = createdTrip.BusId,
                DriverId = createdTrip.DriverId,
                DepartureTime = createdTrip.DepartureTime.ToString("HH:mm"),
                Date = createdTrip.Date,
                Direction = createdTrip.Direction,
                IsDelete = createdTrip.IsDelete,
                Price = createdTrip.Price,
                Status = createdTrip.Status,
                LocationRoutes = locationRoutes
            };
        }


        public async Task<TripResponse?> UpdateAsync(string id, TripRequest request)
        {
            var existing = await _unitOfWork.Trips.GetByIdAsync(id);
            if (existing == null) return null;

            // Update thông tin chuyến
            existing.RouteId = request.RouteId;
            existing.BusId = request.BusId;
            existing.DriverId = request.DriverId;
            existing.DepartureTime = TimeOnly.ParseExact(request.DepartureTime, "HH:mm", null);
            existing.Date = string.IsNullOrEmpty(request.Date)
                ? DateTime.UtcNow.ToString("yyyy-MM-dd")
                : request.Date;
            existing.Direction = request.Direction;
            existing.Price = request.Price;
            existing.Status = request.Status;

            // Update Trip entity
            var updatedTrip = await _unitOfWork.Trips.UpdateAsync(existing);

            // Xử lý LocationRoutes
            var oldLocationRoutes = await _unitOfWork.LocationRoutes.GetByRouteIdAsync(request.RouteId);

            // Xóa hết các LocationRoute cũ
            foreach (var lr in oldLocationRoutes)
            {
                await _unitOfWork.LocationRoutes.DeleteAsync(lr.LocationRouteId);
            }

            // Thêm mới LocationRoutes từ request
            int stopOrder = 1;
            foreach (var lr in request.LocationRoutes)
            {
                var locationRoute = new LocationRoute
                {
                    LocationRouteId = Guid.NewGuid().ToString(),
                    RouteId = request.RouteId,
                    LocationId = lr.LocationId,
                    StopOrder = stopOrder++,
                    StopDuration = TimeSpan.FromMinutes(lr.StopDurationMinutes)
                };

                await _unitOfWork.LocationRoutes.AddAsync(locationRoute);
            }

            await _unitOfWork.CompleteAsync();

            var locationRouteList = await _unitOfWork.LocationRoutes.GetByRouteIdAsync(updatedTrip.RouteId);

            var locationRoutes = locationRouteList.Select(lr => new LocationRouteResponse
            {
                LocationId = lr.LocationId,
                LocationName = lr.Location?.LocationName ?? "",
                StopOrder = lr.StopOrder,
                StopDuration = lr.StopDuration
            }).OrderBy(lr => lr.StopOrder).ToList();

            return new TripResponse
            {
                TripId = updatedTrip.TripId,
                RouteId = updatedTrip.RouteId,
                BusId = updatedTrip.BusId,
                DriverId = updatedTrip.DriverId,
                DepartureTime = updatedTrip.DepartureTime.ToString("HH:mm"),
                Date = updatedTrip.Date,
                Direction = updatedTrip.Direction,
                IsDelete = updatedTrip.IsDelete,
                Price = updatedTrip.Price,
                Status = updatedTrip.Status,
                LocationRoutes = locationRoutes
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
                TripId = t.TripId,  // Ánh xạ TripId
                BusId = t.BusId,    // Ánh xạ BusId
                BusType = t.Bus != null ? t.Bus.BusType : "Unknown", // Ánh xạ BusType từ Bus nếu có
                RouteName = t.Route.RouteName,  // Ánh xạ RouteName
                DepartureTime = DateTime.Today.Add(t.DepartureTime.ToTimeSpan()), // Chuyển TimeOnly sang DateTime
                Date = DateTime.TryParse(t.Date, out DateTime parsedDate) ? parsedDate : DateTime.MinValue, // Chuyển từ string sang DateTime
                Price = t.Price, // Ánh xạ Price
                Stops = t.Route.LocationRoutes
                    .Where(lr => lr.Location != null)  // Kiểm tra để đảm bảo Location không phải null
                    .Select(lr => new LocationStopDto
                    {
                        LocationName = lr.Location?.LocationName ?? string.Empty,  // Đảm bảo không có lỗi nếu LocationName là null
                        StopOrder = lr.StopOrder,
                        StopDuration = lr.StopDuration
                    }).ToList()
            });
        }

        public async Task<IEnumerable<TripSearchDto>> SearchByKeywordAsync(string keyword)
        {
            var tripsByRoute = await _unitOfWork.Trips.SearchTripsByRouteNameAsync(keyword);
            if (tripsByRoute.Any())
            {
                return tripsByRoute.Select(t => ConvertTripToDto(t));
            }

            var tripsByLocation = await _unitOfWork.Trips.SearchTripsByLocationNameAsync(keyword);
            return tripsByLocation.Select(t => ConvertTripToDto(t));
        }

        public async Task<List<TripSearchDto>> SearchTripsAsync(string keyword, string? date, string? type)
        {
            // Tìm chuyến đi theo từ khóa và ngày
            var trips = await _unitOfWork.Trips.SearchTripsByKeywordAndDateAsync(keyword, date);

            // Áp dụng sắp xếp theo yêu cầu
            trips = type switch
            {
                "earliest" => trips.OrderBy(t => t.DepartureTime).ToList(),
                "latest" => trips.OrderByDescending(t => t.DepartureTime).ToList(),
                "priceAsc" => trips.OrderBy(t => t.Price).ToList(),
                "priceDesc" => trips.OrderByDescending(t => t.Price).ToList(),
                _ => trips
            };

            // Ánh xạ từ trips thành TripSearchDto
            var tripDtos = trips.Select(t => new TripSearchDto
            {
                TripId = t.TripId,  // Ánh xạ TripId
                BusId = t.BusId,    // Ánh xạ BusId
                BusType = t.Bus != null ? t.Bus.BusType : "Unknown", // Ánh xạ BusType từ Bus nếu có
                RouteName = t.Route.RouteName,  // Ánh xạ RouteName
                DepartureTime = DateTime.Today.Add(t.DepartureTime.ToTimeSpan()), // Chuyển TimeOnly sang DateTime
                Date = DateTime.TryParse(t.Date, out DateTime parsedDate) ? parsedDate : DateTime.MinValue, // Chuyển từ string sang DateTime
                Price = t.Price, // Ánh xạ Price
                Stops = t.Route.LocationRoutes
                    .Where(lr => lr.Location != null)  // Kiểm tra để đảm bảo Location không phải null
                    .Select(lr => new LocationStopDto
                    {
                        LocationName = lr.Location?.LocationName ?? string.Empty,  // Đảm bảo không có lỗi nếu LocationName là null
                        StopOrder = lr.StopOrder,
                        StopDuration = lr.StopDuration
                    }).ToList()
            }).ToList();

            return tripDtos;  // Trả về kết quả
        }


        private TripSearchDto ConvertTripToDto(Trip t)
        {
            return new TripSearchDto
            {
                TripId = t.TripId,  // Ánh xạ TripId
                BusId = t.BusId,    // Ánh xạ BusId
                BusType = t.Bus != null ? t.Bus.BusType : "Unknown", // Ánh xạ BusType từ Bus nếu có
                RouteName = t.Route.RouteName,  // Ánh xạ RouteName
                DepartureTime = DateTime.Today.Add(t.DepartureTime.ToTimeSpan()), // Chuyển TimeOnly sang DateTime
                Date = DateTime.TryParse(t.Date, out DateTime parsedDate) ? parsedDate : DateTime.MinValue, // Chuyển từ string sang DateTime
                Price = t.Price, // Ánh xạ Price
                Stops = t.Route.LocationRoutes
                    .Where(lr => lr.Location != null)  // Kiểm tra để đảm bảo Location không phải null
                    .Select(lr => new LocationStopDto
                    {
                        LocationName = lr.Location?.LocationName ?? string.Empty,  // Đảm bảo không có lỗi nếu LocationName là null
                        StopOrder = lr.StopOrder,
                        StopDuration = lr.StopDuration
                    }).ToList()
            };
        }
        public async Task<List<TripResponse>> GetTripsByDriverIdAsync(string driverId)
        {
            var trips = await _unitOfWork.Trips.GetAllAsync();
            return trips
                .Where(t => t.DriverId == driverId)
                .Select(t => new TripResponse
                {
                    TripId = t.TripId,
                    RouteId = t.RouteId,
                    BusId = t.BusId,
                    BusType = t.Bus != null ? t.Bus.BusType : "Unknown",
                    RouteName = t.Route.RouteName,
                    DriverId = t.DriverId,
                    DriverName = t.Driver != null ? t.Driver.Name : "Unknown",
                    DepartureTime = t.DepartureTime.ToString("HH:mm"),
                    Date = t.Date,
                    Direction = t.Direction,
                    IsDelete = t.IsDelete,
                    Price = t.Price,
                    Status = t.Status
                }).ToList();
        }
        public async Task<List<CustomerResponseByTrip>> GetCustomersByTripAsync(string tripId)
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            var tripTickets = tickets.Where(t => t.TripId == tripId && !t.isCancelled);

            var bookIds = tripTickets.Select(t => t.BookId).Distinct().ToList();
            var books = await _unitOfWork.Books.GetAllAsync();
            var customerIds = books
                .Where(b => bookIds.Contains(b.BookId))
                .Select(b => b.CustomerId)
                .Distinct()
                .ToList();

            var customers = await _unitOfWork.Customers.GetAllAsync();

            var result = customers
                .Where(c => customerIds.Contains(c.CustomerId))
                .Select(c => new CustomerResponseByTrip
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    Score = c.Score,
                    RankName = c.Membership?.RankName ?? "Bronve"
                })
                .ToList();

            return result;
        }

        public async Task<TripResponse?> CompleteTripAsync(string tripId)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(tripId);
            if (trip == null) return null;

            trip.Status = "Hoàn Thành";
            await _unitOfWork.Trips.UpdateAsync(trip);

            // Lấy BusId từ trip và reset toàn bộ ghế của bus đó
            var busId = trip.BusId;
            var seats = await _unitOfWork.Seats.GetAllAsync();
            var seatsOfBus = seats.Where(s => s.BusId == busId && !s.IsDelete).ToList();

            foreach (var seat in seatsOfBus)
            {
                seat.IsBooked = false;
                await _unitOfWork.Seats.UpdateAsync(seat);
            }

            await _unitOfWork.CompleteAsync();

            return new TripResponse
            {
                TripId = trip.TripId,
                RouteId = trip.RouteId,
                BusId = trip.BusId,
                DriverId = trip.DriverId,
                DepartureTime = trip.DepartureTime.ToString("HH:mm"),
                Date = trip.Date,
                Direction = trip.Direction,
                IsDelete = trip.IsDelete,
                Price = trip.Price,
                Status = trip.Status
            };
        }
        public async Task<TripResponse?> AssignDriverAsync(string tripId, string driverId)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(tripId);
            if (trip == null) return null;

            trip.DriverId = driverId;
            var updated = await _unitOfWork.Trips.UpdateAsync(trip);
            await _unitOfWork.CompleteAsync();

            return new TripResponse
            {
                TripId = updated.TripId,
                RouteId = updated.RouteId,
                BusId = updated.BusId,
                DriverId = updated.DriverId,
                DepartureTime = updated.DepartureTime.ToString("HH:mm"),
                //Date = TryFormatDate(updated.Date),
                Date = updated.Date,
                Direction = updated.Direction,
                IsDelete = updated.IsDelete,
                Price = updated.Price,
                Status = updated.Status
            };
        }
        public async Task<List<object>> GetTripsForCalendarAsync(string? month)
        {
            var trips = await _unitOfWork.Trips.GetAllAsync();

            if (!string.IsNullOrEmpty(month))
            {
                var parts = month.Split('/');
                if (parts.Length == 2 && int.TryParse(parts[0], out int m) && int.TryParse(parts[1], out int y))
                {
                    trips = trips.Where(t =>
                    {
                        if (DateTime.TryParseExact(t.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            return date.Month == m && date.Year == y;
                        }
                        return false;
                    }).ToList();
                }
            }

            return trips.Select(t => new
            {
                id = t.TripId,
                title = $"{t.Route?.RouteName ?? "Chuyến"} ({t.DepartureTime:hh\\:mm})",
                start = DateTime.ParseExact(t.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                end = DateTime.ParseExact(t.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                hasDriver = !string.IsNullOrEmpty(t.DriverId)
            }).Cast<object>().ToList();
        }
        //phan trang
        public async Task<(List<TripResponse>, int)> GetFilteredAsync(QueryParameters query)
        {
            var trips = await _unitOfWork.Trips.GetAllAsync();

            foreach (var t in trips)
            {
                t.Route ??= await _unitOfWork.Routes.GetByIdAsync(t.RouteId);
                t.Bus ??= await _unitOfWork.Buses.GetByIdAsync(t.BusId);
                if (!string.IsNullOrEmpty(t.DriverId))
                {
                    t.Driver ??= await _unitOfWork.Users.GetByIdAsync(t.DriverId);
                }
            }

            var filtered = trips.AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                filtered = filtered.Where(t =>
                      (t.Route != null && t.Route.RouteName != null && t.Route.RouteName.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(t.DriverId) && t.DriverId.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase))||
                      (t.Route != null && t.Route.RouteName != null && t.Route.RouteName.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase)) ||
            (t.Date != null && t.Date.Contains(query.Keyword))
                    );

            }

            filtered = query.SortBy switch
            {
                "date_asc" => filtered.OrderBy(t => DateTime.ParseExact(t.Date, "dd-MM-yyyy", null)),
                "price_asc" => filtered.OrderBy(t => t.Price),
                "price_desc" => filtered.OrderByDescending(t => t.Price),
                _ => filtered.OrderByDescending(t => DateTime.ParseExact(t.Date, "dd-MM-yyyy", null))
            };


            var total = filtered.Count();

            if (query.PageSize == -1)
            {
                var allMapped = filtered.Select(t => MapToResponse(t)).ToList();
                return (allMapped, allMapped.Count);
            }

            var paged = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var mapped = paged.Select(t => MapToResponse(t)).ToList();
            return (mapped, total);
        }

        private TripResponse MapToResponse(Trip t)
        {
            var locationRoutes = new List<LocationRouteResponse>();
            if (t.Route?.LocationRoutes != null)
            {
                locationRoutes = t.Route.LocationRoutes
                    .OrderBy(lr => lr.StopOrder)
                    .Select(lr => new LocationRouteResponse
                    {
                        LocationId = lr.LocationId,
                        LocationName = lr.Location?.LocationName ?? "",
                        StopOrder = lr.StopOrder,
                        StopDuration = lr.StopDuration
                    }).ToList();
            }

            return new TripResponse
            {
                TripId = t.TripId,
                RouteId = t.RouteId,
                RouteName = t.Route?.RouteName ?? "",
                BusId = t.BusId,
                BusType = t.Bus?.BusType ?? "",
                DriverId = t.DriverId ?? "",
                DriverName = t.Driver?.Name ?? "",
                DepartureTime = t.DepartureTime.ToString("HH:mm"),
                //Date = TryFormatDate(t.Date),
                Date = t.Date,
                Direction = t.Direction,
                IsDelete = t.IsDelete,
                Price = t.Price,
                Status = t.Status,
                LocationRoutes = locationRoutes
            };
        }


    }
}
