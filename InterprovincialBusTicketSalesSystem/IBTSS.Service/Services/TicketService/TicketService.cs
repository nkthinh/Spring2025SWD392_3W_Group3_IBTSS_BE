using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Ticket;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Ticket;
using IBTSS.Service.Services.CustomerService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                RouteName = t.Trip?.Route?.RouteName ?? "N/A",
                DepartureTime = t.Trip?.DepartureTime.ToString("HH:mm") ?? "N/A",
                Date = t.Trip?.Date,
                BusType = t.Trip?.Bus?.BusType ?? "Unknown",
                CreatedAt = t.Book?.CreatedAt ?? DateTime.MinValue,
                CustomerId = t.Book?.CustomerId ?? string.Empty,
                CustomerName = t.Book?.Customer?.Name ?? "",
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
                RouteName = t.Trip?.Route?.RouteName ?? "N/A",
                DepartureTime = t.Trip?.DepartureTime.ToString("HH:mm") ?? "N/A",
                Date = t.Trip?.Date,
                BusType = t.Trip?.Bus?.BusType ?? "Unknown",
                CreatedAt = t.Book?.CreatedAt ?? DateTime.MinValue,
                CustomerId = t.Book?.CustomerId ?? string.Empty,
                CustomerName = t.Book?.Customer?.Name ?? "",
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
                Status = "Đang xử lý"
            };

            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.CompleteAsync();

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                BookId = ticket.BookId,
                TripId = ticket.TripId,
                SeatId = ticket.SeatId,
                RouteName = ticket.Trip?.Route?.RouteName ?? "N/A",
                BusType = ticket.Trip?.Bus?.BusType ?? "Unknown",
                CreatedAt = ticket.CreatedAt,
                CustomerId = book.CustomerId,
                CustomerName = book?.Customer?.Name ?? "",
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
                RouteName = existing.Trip?.Route?.RouteName ?? "N/A",
                BusType = existing.Trip?.Bus?.BusType ?? "Unknown",
                CreatedAt = book?.CreatedAt ?? DateTime.MinValue,
                CustomerId = book?.CustomerId ?? string.Empty,
                CustomerName = book?.Customer?.Name ?? "",
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
                RouteName = t.Trip?.Route?.RouteName ?? "N/A",
                BusType = t.Trip?.Bus?.BusType ?? "Unknown",
                CustomerId = book.CustomerId,
                CustomerName = book?.Customer?.Name ?? "",
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
                RouteName = t.Trip?.Route?.RouteName ?? "N/A",
                DepartureTime = t.Trip?.DepartureTime.ToString("HH:mm") ?? "N/A",
                Date = t.Trip?.Date,
                BusType = t.Trip?.Bus?.BusType ?? "Unknown",
                CreatedAt = t.CreatedAt,
                CustomerId = customerId,
                CustomerName = t.Book?.Customer?.Name ?? "",
                IsCancelled = t.isCancelled,
                Price = t.Price,
                Status = t.Status
            }).ToList();
        }
        public async Task<TicketResponse?> CancelTicketAsync(string ticketId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null) return null;

            ticket.Status = "Vé Đã Hủy";
            ticket.isCancelled = true;

            if (!string.IsNullOrEmpty(ticket.SeatId))
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(ticket.SeatId);
                if (seat != null)
                {
                    seat.IsBooked = false;
                    await _unitOfWork.Seats.UpdateAsync(seat);
                }
                ticket.SeatId = null;
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
                RouteName = ticket.Trip?.Route?.RouteName ?? "N/A",
                BusType = ticket.Trip?.Bus?.BusType ?? "Unknown",
                CreatedAt = ticket.CreatedAt,
                CustomerId = book?.CustomerId ?? "",
                CustomerName = book?.Customer?.Name ?? "",
                IsCancelled = ticket.isCancelled,
                Price = ticket.Price,
                Status = ticket.Status
            };
        }
        public async Task<bool> ConfirmBoardingAsync(string ticketId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            ticket.Status = "Đã lên xe";
            await _unitOfWork.Tickets.UpdateAsync(ticket);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ChangeSeatAsync(string ticketId, string newSeatId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null || ticket.Status != "Hoàn Thành") return false;

            var book = await _unitOfWork.Books.GetByIdAsync(ticket.BookId);
            if (book == null) return false;

            var transaction = await _unitOfWork.Transactions.GetByIdAsync(book.TransactionId ?? "");
            if (transaction == null || transaction.PaymentStatus != "Đã Thanh Toán") return false;

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




        //filter
        public async Task<(List<TicketResponse>, int)> GetFilteredAsync(QueryParameters query)
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            var filtered = tickets.AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                var keyword = query.Keyword.Trim();
                filtered = filtered.Where(t =>
                     t.Book != null &&
                    (t.Book.CustomerId != null && t.Book.CustomerId.Contains(keyword, StringComparison.OrdinalIgnoreCase)) || // Lọc theo CustomerId
                     t.TripId.Contains(keyword)|| // Lọc theo TripId
                      (t.Trip != null && t.Trip.Route != null && t.Trip.Route.RouteName.ToLower().Contains(keyword)) // Lọc theo RouteName
                     );
            }
     
            // Sắp xếp vé theo các trường (Price, CreatedAt)

            filtered = query.SortBy switch
            {
                "price_asc" => filtered.OrderBy(t => t.Price),
                "price_desc" => filtered.OrderByDescending(t => t.Price),
                "created_desc" => filtered.OrderByDescending(t => t.CreatedAt),
                "created_asc" => filtered.OrderBy(t => t.CreatedAt),
                _ => filtered.OrderByDescending(t => t.CreatedAt)
            };

            var total = filtered.Count();

            if (query.PageSize == -1)
            {
                var all = filtered.ToList();
                var mapped = all.Select(t => new TicketResponse
                {
                    TicketId = t.TicketId,
                    BookId = t.BookId,
                    TripId = t.TripId,
                    SeatId = t.SeatId,
                    RouteName = t.Trip?.Route?.RouteName ?? "N/A",
                    DepartureTime = t.Trip?.DepartureTime.ToString("HH:mm") ?? "N/A",
                    Date = t.Trip?.Date,
                    BusType = t.Trip?.Bus?.BusType ?? "Unknown",
                    CreatedAt = t.Book?.CreatedAt ?? DateTime.MinValue,
                    CustomerId = t.Book?.CustomerId ?? string.Empty,
                    CustomerName = t.Book?.Customer?.Name ?? "",
                    IsCancelled = t.isCancelled,
                    Price = t.Price,
                    Status = t.Status
                }).ToList();

                return (mapped, mapped.Count);
            }

            var paged = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var result = paged.Select(t => new TicketResponse
            {
                TicketId = t.TicketId,
                BookId = t.BookId,
                TripId = t.TripId,
                SeatId = t.SeatId,
                RouteName = t.Trip?.Route?.RouteName ?? "N/A",
                DepartureTime = t.Trip?.DepartureTime.ToString("HH:mm") ?? "N/A",
                Date = t.Trip?.Date,
                BusType = t.Trip?.Bus?.BusType ?? "Unknown",
                CreatedAt = t.Book?.CreatedAt ?? DateTime.MinValue,
                CustomerId = t.Book?.CustomerId ?? string.Empty,
                CustomerName = t.Book?.Customer?.Name ?? "",
                IsCancelled = t.isCancelled,
                Price = t.Price,
                Status = t.Status
            }).ToList();

            return (result, total);
        }
        //Lọc vé theo tuyến đường
        public async Task<List<RouteBookingStatisticResponse>> GetRouteBookingStatisticsAsync(int year, int? month, string sortOrder)
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();

            var filteredTickets = tickets.Where(t =>
                t.CreatedAt.Year == year &&
                (!month.HasValue || t.CreatedAt.Month == month.Value)
            ).ToList();

            var groupedData = filteredTickets
                .GroupBy(t => t.Trip != null && t.Trip.Route != null ? t.Trip.Route.RouteName : "Unknown Route")
                .Select(g => new RouteBookingStatisticResponse
                {
                    RouterName = g.Key,
                    Count = g.Count()
                });

            var sortedData = sortOrder.ToLower() == "asc"
                ? groupedData.OrderBy(x => x.Count).ToList()
                : groupedData.OrderByDescending(x => x.Count).ToList();

            return sortedData;
        }


    }
}