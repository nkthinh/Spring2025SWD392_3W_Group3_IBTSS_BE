using AutoMapper;
using IBTSS.Service.DTO.Request.Book;
using IBTSS.Service.DTO.Response.Book;
using IBTSS.Service.Services.BookService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetAll()
        {
            var books = await _service.GetAllAsync();

            // Ánh xạ sang danh sách DTO
            var bookResponses = _mapper.Map<List<BookResponse>>(books);

            return Ok(bookResponses);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var book = await _service.GetByIdAsync(id);
            if (book == null) return NotFound();

            var bookResponse = _mapper.Map<BookResponse>(book);
            return Ok(bookResponse); // ✅ Trả về DTO, không vòng lặp
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
