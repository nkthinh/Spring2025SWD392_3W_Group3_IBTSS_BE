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
                Model = request.Model,
                ModelYear = request.ModelYear,
                Color = request.Color,
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
                Model = bus.Model,
                ModelYear = bus.ModelYear,
                Color = bus.Color,
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
                .Where(t => t.Trip != null && t.Trip.BusId == id && !t.isCancelled && !t.IsDelete)
                .Select(t => t.SeatId)
                .ToHashSet();

            return new BusResponse
            {
                BusId = bus.BusId,
                BusType = bus.BusType,
                SeatCount = bus.SeatCount,
                Model = bus.Model,
                ModelYear = bus.ModelYear,
                Color = bus.Color,
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
                Model = b.Model,
                ModelYear = b.ModelYear,
                Color = b.Color,
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

            var oldSeatCount = bus.SeatCount;
            var newSeatCount = request.SeatCount;

            // Cập nhật thông tin cơ bản
            bus.BusType = request.BusType;
            bus.Model = request.Model;
            bus.ModelYear = request.ModelYear;
            bus.Color = request.Color;

            // Tăng số lượng ghế
            if (newSeatCount > oldSeatCount)
            {
                var seatsToAdd = newSeatCount - oldSeatCount;
                var newSeats = new List<Seat>();

                for (int i = 0; i < seatsToAdd; i++)
                {
                    newSeats.Add(new Seat
                    {
                        BusId = bus.BusId,
                        IsBooked = false,
                        IsDelete = false
                    });
                }

                await _unitOfWork.Seats.AddMultipleAsync(newSeats);
            }
            // Giảm số lượng ghế
            else if (newSeatCount < oldSeatCount)
            {
                var countToRemove = oldSeatCount - newSeatCount;

                if (request.SeatIdsToRemove == null || request.SeatIdsToRemove.Count != countToRemove)
                {
                    throw new Exception($"Must provide exactly {countToRemove} seat IDs to remove.");
                }

                foreach (var seatId in request.SeatIdsToRemove)
                {
                    var seat = await _unitOfWork.Seats.GetByIdAsync(seatId);
                    if (seat == null || seat.BusId != id)
                    {
                        throw new Exception($"Invalid SeatId: {seatId} or seat does not belong to this bus.");
                    }

                    if (seat.IsBooked)
                    {
                        throw new Exception($"Cannot remove seat {seatId} because it is currently booked.");
                    }

                    seat.IsDelete = true;
                    await _unitOfWork.Seats.UpdateAsync(seat);
                }
            }

            // Cập nhật lại số ghế
            bus.SeatCount = newSeatCount;

            var updated = await _unitOfWork.Buses.UpdateAsync(bus);
            await _unitOfWork.CompleteAsync();

            return new BusResponse
            {
                BusId = updated.BusId,
                SeatCount = updated.SeatCount,
                BusType = updated.BusType,
                Model = updated.Model,
                ModelYear = updated.ModelYear,
                Color = updated.Color,
                Seats = updated.Seats
                    .Where(s => !s.IsDelete)
                    .Select(s => new SeatAvailabilityResponse
                    {
                        SeatId = s.SeatId,
                        IsBooked = s.IsBooked
                    }).ToList()
            };
        }

    }
}