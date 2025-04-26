using AutoMapper; // ✅ THÊM
using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.DTO.Request.Book;
using IBTSS.Service.DTO.Response;
using IBTSS.Service.DTO.Response.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.BookService
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
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
                Status = "Đang xử lý"
            };

            await _unitOfWork.Books.AddAsync(book);

            // ✅ Lấy thông tin giảm giá từ Membership và quota
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
            float discountRate = 0f;
            int discountQuotaLeft = customer?.DiscountQuotaLeft ?? 0;
            bool hasDiscount = customer?.MembershipId != null && discountQuotaLeft > 0;

            if (hasDiscount)
            {
                var membership = await _unitOfWork.Memberships.GetByIdAsync(customer.MembershipId);
                discountRate = membership?.DiscountRate ?? 0;
            }

            var tickets = new List<Ticket>();
            foreach (var seatId in request.Seats)
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(seatId);
                if (seat == null || seat.IsBooked)
                {
                    throw new Exception($"Seat {seatId} is not available");
                }
                //seat.IsBooked = true; //chưa thanh toán thì isBooked của seat vẫn bằng false
                await _unitOfWork.Seats.UpdateAsync(seat);

                bool applyDiscount = hasDiscount && discountQuotaLeft > 0;
                int discountedPrice = applyDiscount ? (int)(trip.Price * (1 - discountRate)) : trip.Price;

                if (applyDiscount)
                    discountQuotaLeft--;

                var ticket = new Ticket
                {
                    TicketId = Guid.NewGuid().ToString(),
                    BookId = book.BookId,
                    TripId = request.TripId,
                    SeatId = seatId,
                    OriginalPrice = trip.Price,
                    Price = discountedPrice,
                    isCancelled = false,
                    IsDelete = false,
                    CreatedAt = book.CreatedAt,
                    Status = "Đang xử lý"
                };

                tickets.Add(ticket);
            }

            book.TotalPrice = tickets.Sum(t => t.Price);
            book.Tickets = tickets;

            await _unitOfWork.Tickets.AddRangeAsync(tickets);

            // ✅ cập nhật quota còn lại của khách nếu có áp dụng giảm giá
            if (hasDiscount && customer != null)
            {
                customer.DiscountQuotaLeft = discountQuotaLeft;
                await _unitOfWork.Customers.UpdateAsync(customer);
            }

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookResponse>(book);
        }

        public async Task<Book?> UpdateAsync(string id, CreateBookRequest request)
        {
            var existing = await _unitOfWork.Books.GetByIdAsync(id);
            if (existing == null) return null;

            existing.CustomerId = request.CustomerId;
            existing.Status = "Đã cập nhật";
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
        public async Task<(List<BookResponse>, int)> GetFilteredAsync(BookQueryParameters query)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var filtered = books.AsQueryable();
            //keyword=custromerId
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                filtered = filtered.Where(b => b.Customer != null && b.Customer.CustomerId.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase));
            }

            // Sort
            filtered = query.SortBy switch
            {
                "date_asc" => filtered.OrderBy(b => b.CreatedAt),  
                "price_desc"=>filtered.OrderByDescending(b=>b.TotalPrice),
                "price_asc" => filtered.OrderBy(b => b.TotalPrice),
                _ => filtered.OrderByDescending(b => b.CreatedAt),
            };

            var totalCount = filtered.Count();

            if (query.PageSize == -1)
            {
                var allMapped = _mapper.Map<List<BookResponse>>(filtered.ToList());
                return (allMapped, allMapped.Count);
            }

            var items = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var mapped = _mapper.Map<List<BookResponse>>(items);
            return (mapped, totalCount);

        }



    }
}