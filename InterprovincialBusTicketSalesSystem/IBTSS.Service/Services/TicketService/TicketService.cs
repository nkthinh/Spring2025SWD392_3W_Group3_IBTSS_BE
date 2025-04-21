using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Ticket;
using IBTSS.Service.DTO.Response.Ticket;
using IBTSS.Service.Services.CustomerService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.TicketService
{
    public class TicketService(IUnitOfWork _unitOfWork, ILogger<TicketService> _logger) : ITicketService
    {
        public async Task<List<TicketResponse>> GetAllAsync()
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            return tickets.Select(t => new TicketResponse
            {
                TicketId = t.TicketId,
                TripId = t.TripId,
                CustomerId = t.CustomerId,
                SeatId = t.SeatId,
                CreatedAt = t.CreatedAt,
                IsCancelled = t.IsCancelled,
                Price = t.Price,
                Status = t.Status
            }).ToList();
        }

        public async Task<TicketResponse?> GetByIdAsync(string id)
        {
            var t = await _unitOfWork.Tickets.GetByIdAsync(id);
            if (t == null) return null;

            return new TicketResponse
            {
                TicketId = t.TicketId,
                TripId = t.TripId,
                CustomerId = t.CustomerId,
                SeatId = t.SeatId,
                CreatedAt = t.CreatedAt,
                IsCancelled = t.IsCancelled,
                Price = t.Price,
                Status = t.Status
            };
        }

        public async Task<TicketResponse> AddAsync(TicketRequest request)
        {
            // 1. Kiểm tra ghế có tồn tại và chưa được đặt
            var seat = await _unitOfWork.Seats.GetByIdAsync(request.SeatId);
            if (seat == null)
                throw new Exception("Seat is not existed.");

            if (seat.IsBooked)
                throw new Exception("Seat being booked.");

            // 2. Đánh dấu ghế là đã đặt
            seat.IsBooked = true;
            await _unitOfWork.Seats.UpdateAsync(seat);

            // 3. Lấy thông tin của chuyến đi để gán giá vé
            var trip = await _unitOfWork.Trips.GetByIdAsync(request.TripId);
            if (trip == null)
                throw new Exception("Trip is not existed.");

            // Gán giá vé từ chuyến đi vào vé
            var t = new Book
            {
                TripId = request.TripId,
                CustomerId = request.CustomerId,
                SeatId = request.SeatId,
                IsCancelled = false,
                IsDelete = false,
                Price = trip.Price,  // Gán giá từ chuyến đi
                Status = request.Status
            };

            var created = await _unitOfWork.Tickets.AddAsync(t);

            // 4. Lưu thay đổi vào database
            await _unitOfWork.CompleteAsync();

            // 5. Trả về response
            return new TicketResponse
            {
                TicketId = created.TicketId,
                TripId = created.TripId,
                CustomerId = created.CustomerId,
                SeatId = created.SeatId,
                CreatedAt = created.CreatedAt,
                IsCancelled = created.IsCancelled,
                Price = created.Price,  // Trả về giá vé đã được gán
                Status = created.Status
            };
        }



        public async Task<TicketResponse?> UpdateAsync(string id, TicketRequest request)
        {
            var existing = await _unitOfWork.Tickets.GetByIdAsync(id);
            if (existing == null) return null;

            existing.TripId = request.TripId;
            existing.CustomerId = request.CustomerId;
            existing.SeatId = request.SeatId;
            existing.Status = request.Status;
            existing.IsCancelled = request.IsCancelled;

            var updated = await _unitOfWork.Tickets.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return new TicketResponse
            {
                TicketId = updated.TicketId,
                TripId = updated.TripId,
                CustomerId = updated.CustomerId,
                SeatId = updated.SeatId,
                CreatedAt = updated.CreatedAt,
                IsCancelled = updated.IsCancelled,
                Price = updated.Price,
                Status = updated.Status
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var deleted = await _unitOfWork.Tickets.DeleteAsync(id);
            if (deleted) await _unitOfWork.CompleteAsync();
            return deleted;
        }
        public async Task<List<Book>> GetByCustomerIdAsync(string customerId)
        {
            return await _unitOfWork.Tickets.GetByCustomerIdAsync(customerId);
        }
    }
}
