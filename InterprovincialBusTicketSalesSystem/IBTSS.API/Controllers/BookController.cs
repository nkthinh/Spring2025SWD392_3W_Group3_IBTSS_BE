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
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var book = await _service.GetByIdAsync(id);
                if (book == null) return NotFound(new { message = "Not Found" });

                var bookResponse = _mapper.Map<BookResponse>(book);
                return Ok(bookResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateBookRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, request);
                if (updated == null)
                    return NotFound(new { message = "Not Found" });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Not Found" });

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
