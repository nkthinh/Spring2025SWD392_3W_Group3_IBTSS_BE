using AutoMapper; // ✅ THÊM
using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Book;
using IBTSS.Service.DTO.Response.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.BookService
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // ✅ THÊM

        public BookService(IUnitOfWork unitOfWork, IMapper mapper) // ✅ THÊM IMapper
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<Book>> GetAllAsync() =>
            await _unitOfWork.Books.GetAllAsync();

        public async Task<Book?> GetByIdAsync(string id) =>
            await _unitOfWork.Books.GetByIdAsync(id);

        public async Task<BookResponse> CreateAsync(CreateBookRequest request)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(request.TripId)
                       ?? throw new Exception("Trip not found");

            var book = new Book
            {
                BookId = Guid.NewGuid().ToString(),
                CustomerId = request.CustomerId,
                CreatedAt = DateTime.UtcNow,
                TicketCount = request.Seats.Count,
                TotalPrice = 0,
                Status = "Pending"
            };

            await _unitOfWork.Books.AddAsync(book);

            var tickets = new List<Ticket>();

            foreach (var seatId in request.Seats)
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(seatId);
                if (seat == null || seat.IsBooked)
                    throw new Exception($"Seat {seatId} is not available");

                seat.IsBooked = true;
                await _unitOfWork.Seats.UpdateAsync(seat);

                var ticket = new Ticket
                {
                    TicketId = Guid.NewGuid().ToString(),
                    BookId = book.BookId,
                    TripId = request.TripId,
                    SeatId = seatId,
                    Price = trip.Price,
                    isCancelled = false,
                    IsDelete = false,
                    CreatedAt = book.CreatedAt,
                    Status = "Pending"
                };

                tickets.Add(ticket);
            }

            book.TotalPrice = tickets.Sum(t => t.Price);
            book.Tickets = tickets; // ✅ GÁN tickets để AutoMapper map sang DTO

            await _unitOfWork.Tickets.AddRangeAsync(tickets);
            await _unitOfWork.CompleteAsync();

            // ✅ DÙNG AutoMapper
            return _mapper.Map<BookResponse>(book);
        }

        public async Task<Book?> UpdateAsync(string id, CreateBookRequest request)
        {
            var existing = await _unitOfWork.Books.GetByIdAsync(id);
            if (existing == null) return null;

            existing.CustomerId = request.CustomerId;
            existing.Status = "Updated";
            await _unitOfWork.Books.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return false;

            await _unitOfWork.Books.DeleteAsync(book);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<List<Ticket>> GenerateTicketsAsync(Book book, CreateBookRequest request)
        {
            var trip = await _unitOfWork.Trips.GetByIdAsync(request.TripId)
                       ?? throw new Exception("Trip not found");

            var tickets = new List<Ticket>();

            foreach (var seatId in request.Seats)
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(seatId);
                if (seat == null || seat.IsBooked)
                    throw new Exception($"Seat {seatId} is not available");

                seat.IsBooked = true;
                await _unitOfWork.Seats.UpdateAsync(seat);

                tickets.Add(new Ticket
                {
                    TicketId = Guid.NewGuid().ToString(),
                    BookId = book.BookId,
                    TripId = request.TripId,
                    SeatId = seatId,
                    Price = trip.Price,
                    isCancelled = false,
                    IsDelete = false
                });
            }

            return tickets;
        }
    }
}
