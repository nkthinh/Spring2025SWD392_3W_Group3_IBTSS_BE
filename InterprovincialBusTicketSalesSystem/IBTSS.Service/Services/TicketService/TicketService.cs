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
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TicketService> _logger;

        public TicketService(IUnitOfWork unitOfWork, ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<TicketResponse>> GetAllAsync()
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();

            return tickets.Select(t => new TicketResponse
            {
                TicketId = t.TicketId,
                BookId = t.BookId,
                TripId = t.TripId,
                SeatId = t.SeatId,
                CreatedAt = t.Book?.CreatedAt ?? DateTime.MinValue,
                CustomerId = t.Book?.CustomerId ?? string.Empty,
                IsCancelled = t.isCancelled,
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
                BookId = t.BookId,
                TripId = t.TripId,
                SeatId = t.SeatId,
                CreatedAt = t.Book?.CreatedAt ?? DateTime.MinValue,
                CustomerId = t.Book?.CustomerId ?? string.Empty,
                IsCancelled = t.isCancelled,
                Price = t.Price,
                Status = t.Status
            };
        }

        public async Task<TicketResponse> AddAsync(TicketRequest request)
        {
            var seat = await _unitOfWork.Seats.GetByIdAsync(request.SeatId);
            if (seat == null || seat.IsBooked)
                throw new Exception("Seat is not available.");

            seat.IsBooked = true;
            await _unitOfWork.Seats.UpdateAsync(seat);

            var trip = await _unitOfWork.Trips.GetByIdAsync(request.TripId)
                       ?? throw new Exception("Trip not found");

            var book = await _unitOfWork.Books.GetByIdAsync(request.BookId)
                       ?? throw new Exception("Book not found");

            var ticket = new Ticket
            {
                TicketId = Guid.NewGuid().ToString(),
                BookId = request.BookId,
                TripId = request.TripId,
                SeatId = request.SeatId,
                CreatedAt = book.CreatedAt,
                isCancelled = false,
                IsDelete = false,
                Price = trip.Price,
                Status = "Pending"
            };

            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.CompleteAsync();

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                BookId = ticket.BookId,
                TripId = ticket.TripId,
                SeatId = ticket.SeatId,
                CreatedAt = ticket.CreatedAt,
                CustomerId = book.CustomerId,
                IsCancelled = ticket.isCancelled,
                Price = ticket.Price,
                Status = ticket.Status
            };
        }

        public async Task<TicketResponse?> UpdateAsync(string id, TicketRequest request)
        {
            var existing = await _unitOfWork.Tickets.GetByIdAsync(id);
            if (existing == null) return null;

            existing.TripId = request.TripId;
            existing.SeatId = request.SeatId;
            existing.Status = request.Status;

            await _unitOfWork.Tickets.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            var book = await _unitOfWork.Books.GetByIdAsync(existing.BookId);

            return new TicketResponse
            {
                TicketId = existing.TicketId,
                BookId = existing.BookId,
                TripId = existing.TripId,
                SeatId = existing.SeatId,
                CreatedAt = book?.CreatedAt ?? DateTime.MinValue,
                CustomerId = book?.CustomerId ?? string.Empty,
                IsCancelled = existing.isCancelled,
                Price = existing.Price,
                Status = existing.Status
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var deleted = await _unitOfWork.Tickets.DeleteAsync(id);
            if (deleted) await _unitOfWork.CompleteAsync();
            return deleted;
        }

        public async Task<List<TicketResponse>> AddMultipleAsync(MultiTicketRequest request)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(request.TripId)
                       ?? throw new Exception("Trip not found");

            var book = await _unitOfWork.Books.GetByIdAsync(request.BookId)
                       ?? throw new Exception("Book not found");

            var tickets = new List<Ticket>();

            foreach (var seatId in request.SeatIds)
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(seatId);
                if (seat == null || seat.IsBooked)
                    throw new Exception($"Seat {seatId} is already booked or does not exist");

                seat.IsBooked = true;
                await _unitOfWork.Seats.UpdateAsync(seat);

                tickets.Add(new Ticket
                {
                    TicketId = Guid.NewGuid().ToString(),
                    BookId = request.BookId,
                    TripId = request.TripId,
                    SeatId = seatId,
                    isCancelled = false,
                    IsDelete = false,
                    Price = request.Price > 0 ? request.Price : trip.Price,
                    CreatedAt = book.CreatedAt,
                    Status = book.Status
                });
            }

            await _unitOfWork.Tickets.AddRangeAsync(tickets);
            await _unitOfWork.CompleteAsync();

            return tickets.Select(t => new TicketResponse
            {
                TicketId = t.TicketId,
                BookId = t.BookId,
                TripId = t.TripId,
                SeatId = t.SeatId,
                CustomerId = book.CustomerId,
                CreatedAt = t.CreatedAt,
                Price = t.Price,
                IsCancelled = t.isCancelled,
                Status = t.Status
            }).ToList();
        }
        public async Task<List<TicketResponse>> GetByCustomerIdAsync(string customerId)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var bookIds = books.Where(b => b.CustomerId == customerId).Select(b => b.BookId).ToList();

            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            var filtered = tickets.Where(t => bookIds.Contains(t.BookId)).ToList();

            return filtered.Select(t => new TicketResponse
            {
                TicketId = t.TicketId,
                BookId = t.BookId,
                TripId = t.TripId,
                SeatId = t.SeatId,
                CreatedAt = t.CreatedAt,
                CustomerId = customerId,
                IsCancelled = t.isCancelled,
                Price = t.Price,
                Status = t.Status
            }).ToList();
        }
        public async Task<TicketResponse?> CancelTicketAsync(string ticketId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null) return null;

            ticket.Status = "Cancel";
            ticket.isCancelled = true;

            if (!string.IsNullOrEmpty(ticket.SeatId))
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(ticket.SeatId);
                if (seat != null)
                {
                    seat.IsBooked = false;
                    await _unitOfWork.Seats.UpdateAsync(seat);
                }
            }

            await _unitOfWork.Tickets.UpdateAsync(ticket);
            await _unitOfWork.CompleteAsync();

            var book = await _unitOfWork.Books.GetByIdAsync(ticket.BookId);

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                BookId = ticket.BookId,
                TripId = ticket.TripId,
                SeatId = ticket.SeatId,
                CreatedAt = ticket.CreatedAt,
                CustomerId = book?.CustomerId ?? "",
                IsCancelled = ticket.isCancelled,
                Price = ticket.Price,
                Status = ticket.Status
            };
        }
        public async Task<bool> ConfirmBoardingAsync(string ticketId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            ticket.Status = "Boarded";
            await _unitOfWork.Tickets.UpdateAsync(ticket);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ChangeSeatAsync(string ticketId, string newSeatId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null || ticket.Status != "Complete") return false;

            var book = await _unitOfWork.Books.GetByIdAsync(ticket.BookId);
            if (book == null) return false;

            var transaction = await _unitOfWork.Transactions.GetByIdAsync(book.TransactionId ?? "");
            if (transaction == null || transaction.PaymentStatus != "Paid") return false;

            var newSeat = await _unitOfWork.Seats.GetByIdAsync(newSeatId);
            if (newSeat == null || newSeat.IsBooked) return false;

            var oldSeat = await _unitOfWork.Seats.GetByIdAsync(ticket.SeatId ?? "");
            if (oldSeat != null)
            {
                oldSeat.IsBooked = false;
                await _unitOfWork.Seats.UpdateAsync(oldSeat);
            }

            newSeat.IsBooked = true;
            ticket.SeatId = newSeatId;

            await _unitOfWork.Seats.UpdateAsync(newSeat);
            await _unitOfWork.Tickets.UpdateAsync(ticket);
            await _unitOfWork.CompleteAsync();

            return true;
        }

    }
}