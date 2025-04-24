using IBTSS.Service.DTO.Request.Membership;
using IBTSS.Service.Services.MembershipService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _service;

        public MembershipController(IMembershipService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var memberships = await _service.GetAllAsync();
                return Ok(memberships);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Membership with ID '{id}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MembershipRequest request)
        {
            try
            {
                var result = await _service.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.MembershipId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MembershipRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                if (result == null)
                    return NotFound(new { message = $"Membership with ID '{id}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Membership with ID '{id}' not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
