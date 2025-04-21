using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Bus;
using IBTSS.Service.DTO.Response.Bus;
using IBTSS.Service.DTO.Response.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.BusService
{
    public class BusService : IBusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BusResponse> AddAsync(BusRequest request)
        {
            var bus = new Bus
            {
                BusId = request.BusId,
                BusType = request.BusType,
                SeatCount = request.SeatCount,
                IsDelete = false
            };

            await _unitOfWork.Buses.AddAsync(bus);
            await _unitOfWork.CompleteAsync();

            // Tu dong tao seat theo seatcount khi tao bus
            var seats = new List<Seat>();
            for (int i = 0; i < request.SeatCount; i++)
            {
                seats.Add(new Seat
                {
                    BusId = bus.BusId,
                    IsBooked = false
                });
            }

            await _unitOfWork.Seats.AddMultipleAsync(seats);
            await _unitOfWork.CompleteAsync();

            return new BusResponse
            {
                BusId = bus.BusId,
                BusType = bus.BusType,
                SeatCount = bus.SeatCount,
                Seats = seats.Select(s => new SeatAvailabilityResponse
                {
                    SeatId = s.SeatId,
                    IsBooked = false
                }).ToList()
            };
        }

        public async Task<BusResponse?> GetByIdAsync(string id)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            if (bus == null || bus.IsDelete) return null;

            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            var bookedSeatIds = tickets
                .Where(t => t.Trip != null && t.Trip.BusId == id && !t.IsCancelled && !t.IsDelete)
                .Select(t => t.SeatId)
                .ToHashSet();

            return new BusResponse
            {
                BusId = bus.BusId,
                BusType = bus.BusType,
                SeatCount = bus.SeatCount,
                Seats = bus.Seats
                    .Where(s => !s.IsBooked)
                    .Select(s => new SeatAvailabilityResponse
                    {
                        SeatId = s.SeatId,
                        IsBooked = bookedSeatIds.Contains(s.SeatId)
                    }).ToList()
            };
        }

        public async Task<List<BusResponse>> GetAllAsync()
        {
            var buses = await _unitOfWork.Buses.GetAllAsync();
            return buses.Select(b => new BusResponse
            {
                BusId = b.BusId,
                BusType = b.BusType,
                SeatCount = b.SeatCount,
                Seats = b.Seats
                    .Where(s => !s.IsBooked)
                    .Select(s => new SeatAvailabilityResponse
                    {
                        SeatId = s.SeatId,
                        IsBooked = false // Optional: can calculate from tickets
                    }).ToList()
            }).ToList();
        }
        public async Task<BusResponse?> UpdateAsync(string id, BusUpdateRequest request)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            if (bus == null) return null;

            // SeatCount KHÔNG được cập nhật
            bus.BusType = request.BusType;

            var updated = await _unitOfWork.Buses.UpdateAsync(bus);
            await _unitOfWork.CompleteAsync();

            return new BusResponse
            {
                BusId = updated.BusId,
                SeatCount = updated.SeatCount,
                BusType = updated.BusType
            };
        }
    }
}