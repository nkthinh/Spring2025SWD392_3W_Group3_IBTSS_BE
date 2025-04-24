using AutoMapper;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Request.Membership;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.DTO.Response.Membership;
using IBTSS.Service.Services.CustomerService;
using IBTSS.Service.Services.MembershipService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    //admin only
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _service;
        private readonly IMapper _mapper;

        public MembershipController(IMembershipService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }    


        [HttpGet]
        //admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFiltered([FromQuery] QueryParameters? query)
        {
            try
            {
                if (query == null || (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var all = await _service.GetAllAsync();
                    var mapped = _mapper.Map<IEnumerable<MembershipResponse>>(all);
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
                var result = await _service.GetByIdAsync(id);
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //admin only
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(MembershipRequest request)
        {
            try
            {
                var result = await _service.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.MembershipId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, MembershipRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //admin only
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
