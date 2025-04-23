using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Book;
using IBTSS.Service.DTO.Response;
using IBTSS.Service.DTO.Response.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.BookService
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(string id);
        Task<(List<BookResponse>, int)> GetFilteredAsync(BookQueryParameters query);

        Task<BookResponse> CreateAsync(CreateBookRequest request);
        Task<Book?> UpdateAsync(string id, CreateBookRequest request);
        Task<bool> DeleteAsync(string id);
        Task<List<Ticket>> GenerateTicketsAsync(Book book, CreateBookRequest request);
    }

}
