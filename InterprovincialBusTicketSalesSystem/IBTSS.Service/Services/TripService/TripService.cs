using IBTSS.Repository.Entities;
using IBTSS.Repository.Enum;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Trip;
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
            // Kiểm tra xem DriverId có tồn tại hay không
            var driver = await _unitOfWork.Users.GetByIdAsync(request.DriverId);
            if (driver == null)
            {
                throw new Exception($"User with ID {request.DriverId} not found.");
            }

            // Kiểm tra Role của user
            if (driver.Role != UserRole.Driver)
            {
                throw new Exception($"User with ID {request.DriverId} is not a driver.");
            }

            var m = new Trip
            {
                RouteId = request.RouteId,
                BusId = request.BusId,
                DriverId = request.DriverId, // Sử dụng ID được truyền vào
                DepartureTime = request.DepartureTime,
                Date = request.Date,
                Direction = request.Direction,
                IsDelete = false,
                Price = request.Price,
                Status = request.Status,
            };

            var created = await _unitOfWork.Trips.AddAsync(m);
            await _unitOfWork.CompleteAsync();

            return new TripResponse
            {
                TripId = created.TripId,
                RouteId = created.RouteId,
                BusId = created.BusId,
                DriverId = created.DriverId,
                DepartureTime = created.DepartureTime,
                Date = created.Date,
                Direction = created.Direction,
                IsDelete = created.IsDelete,
                Price = created.Price,
                Status = created.Status,
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
    }
}

