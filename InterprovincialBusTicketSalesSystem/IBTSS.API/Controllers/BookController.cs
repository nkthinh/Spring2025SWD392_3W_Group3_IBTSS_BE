using AutoMapper;
using IBTSS.Service.DTO.Request.Book;
using IBTSS.Service.DTO.Response.Book;
using IBTSS.Service.Services.BookService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;
        private readonly IMapper _mapper;

        public BookController(IBookService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] BookQueryParameters? query)
        {
            if (query == null ||
                (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
            {
                var books = await _service.GetAllAsync();
                var mapped = _mapper.Map<List<BookResponse>>(books);
                return Ok(mapped);
            }

            if (query.Page == 0) query.Page = 1;
            if (query.PageSize == 0) query.PageSize = 10;

            var (data, totalCount) = await _service.GetFilteredAsync(query);

            var pagination = new
            {
                TotalCount = totalCount,
                PageSize = query.PageSize,
                CurrentPage = query.Page,
                TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
            };

            return Ok(new
            {
                Data = data,
                Pagination = pagination
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var book = await _service.GetByIdAsync(id);
            if (book == null) return NotFound();

            var bookResponse = _mapper.Map<BookResponse>(book);
            return Ok(bookResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateBookRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);
            return updated == null ? NotFound() : Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
