using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.BookRepository
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync() =>
            await _context.Books.Include(b => b.Tickets).ToListAsync();

        public async Task<Book?> GetByIdAsync(string id) =>
            await _context.Books.Include(b => b.Tickets).FirstOrDefaultAsync(b => b.BookId == id);

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
        }

        public async Task DeleteAsync(Book book)
        {
            _context.Books.Remove(book);
        }
    }

}
