using IBTSS.Service.DTO.Request.Bus;
using IBTSS.Service.Services.BusService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/buses")]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusController(IBusService busService)
        {
            _busService = busService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBus([FromBody] BusRequest request)
        {
            try
            {
                var result = await _busService.AddAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusById(string id)
        {
            try
            {
                var result = await _busService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = "Not Found Bus" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBuses()
        {
            try
            {
                var result = await _busService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBus(string id, [FromBody] BusUpdateRequest request)
        {
            try
            {
                var result = await _busService.UpdateAsync(id, request);
                if (result == null)
                    return NotFound(new { message = "Not Found Bus To Update" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
