using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Seat;
using IBTSS.Service.DTO.Response.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.SeatService
{
    public class SeatService : ISeatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<List<SeatResponse>> GetAllAsync()
        //{
        //    var seats = await _unitOfWork.Seats.GetAllAsync();
        //    return seats.Select(s => new SeatResponse
        //    {
        //        SeatId = s.SeatId,
        //        BusId = s.BusId,
        //        IsDelete = s.IsBooked
        //    }).ToList();
        //}

        //public async Task<SeatResponse?> GetByIdAsync(string id)
        //{
        //    var seat = await _unitOfWork.Seats.GetByIdAsync(id);
        //    if (seat == null) return null;

        //    return new SeatResponse
        //    {
        //        SeatId = seat.SeatId,
        //        BusId = seat.BusId,
        //        IsDelete = seat.IsBooked
        //    };
        //}

        //public async Task<SeatResponse> AddAsync(SeatRequest request)
        //{
        //    var seat = new Seat
        //    {
        //        BusId = request.BusId,
        //        IsBooked = false
        //    };

        //    var created = await _unitOfWork.Seats.AddAsync(seat);
        //    await _unitOfWork.CompleteAsync();

        //    return new SeatResponse
        //    {
        //        SeatId = created.SeatId,
        //        BusId = created.BusId,
        //        IsDelete = created.IsBooked
        //    };
        //}

        //public async Task<SeatResponse?> UpdateAsync(string id, SeatRequest request)
        //{
        //    var existing = await _unitOfWork.Seats.GetByIdAsync(id);
        //    if (existing == null) return null;

        //    existing.BusId = request.BusId;

        //    var updated = await _unitOfWork.Seats.UpdateAsync(existing);
        //    await _unitOfWork.CompleteAsync();

        //    return new SeatResponse
        //    {
        //        SeatId = updated.SeatId,
        //        BusId = updated.BusId,
        //        IsDelete = updated.IsBooked
        //    };
        //}

        //public async Task<bool> DeleteAsync(string id)
        //{
        //    var deleted = await _unitOfWork.Seats.DeleteAsync(id);
        //    if (deleted) await _unitOfWork.CompleteAsync();
        //    return deleted;
        //}
        //public async Task<SeatSummaryResponse> GetSeatAvailabilityByTripIdAsync(string tripId)
        //{
        //    var trip = await _unitOfWork.Trips.GetByIdAsync(tripId);
        //    if (trip == null || trip.IsDelete) throw new Exception("Trip not found");

        //    var busId = trip.BusId;

        //    var seats = await _unitOfWork.Seats.GetAllAsync();
        //    var busSeats = seats.Where(s => s.BusId == busId && !s.IsBooked).ToList();
        //    var seatIds = busSeats.Select(s => s.SeatId).ToList();

        //    var tickets = await _unitOfWork.Tickets.GetAllAsync();
        //    var activeTickets = tickets
        //        .Where(t => !t.IsCancelled && !t.IsDelete && t.TripId == tripId && seatIds.Contains(t.SeatId))
        //        .Select(t => t.SeatId)
        //        .Distinct()
        //        .ToList();

        //    var seatResponses = busSeats.Select(s => new SeatAvailabilityResponse
        //    {
        //        SeatId = s.SeatId,
        //        IsBooked = activeTickets.Contains(s.SeatId)
        //    }).ToList();

        //    return new SeatSummaryResponse
        //    {
        //        BusId = busId,
        //        TotalSeats = seatResponses.Count,
        //        BookedSeats = seatResponses.Count(s => s.IsBooked),
        //        Seats = seatResponses
        //    };
        //}

    }
}

