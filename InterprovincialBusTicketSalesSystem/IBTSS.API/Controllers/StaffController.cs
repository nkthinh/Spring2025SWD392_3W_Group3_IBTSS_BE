using IBTSS.Service.Services.TripService;
using IBTSS.Service.Services.TicketService;
using IBTSS.Service.Services.SeatService;
using IBTSS.Service.Services.CustomerService;
using IBTSS.Service.Services.SMSService;
using IBTSS.Service.Services.TransactionService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly ITicketService _ticketService;
        private readonly ISeatService _seatService;
        private readonly ICustomerService _customerService;
        private readonly ITransactionService _transactionService;
        private readonly SmsService _smsService;

        public StaffController(
            ITripService tripService,
            ITicketService ticketService,
            ISeatService seatService,
            ICustomerService customerService,
            ITransactionService transactionService,
            SmsService smsService)
        {
            _tripService = tripService;
            _ticketService = ticketService;
            _seatService = seatService;
            _customerService = customerService;
            _transactionService = transactionService;
            _smsService = smsService;
        }

        // 1. Xem lịch trình các chuyến đi
        //[HttpGet("trips")]
        //public async Task<IActionResult> GetTrips() =>
        //    Ok(await _tripService.GetAllAsync());

        // 2. Xem danh sách vé theo chuyến
        //[HttpGet("tickets/by-trip/{tripId}")]
        //public async Task<IActionResult> GetTicketsByTrip(string tripId)
        //{
        //    var tickets = await _ticketService.GetAllAsync();
        //    var result = tickets.Where(t => t.TripId == tripId);
        //    return Ok(result);
        //}

        // 3. Xác nhận khách đã lên xe
        [HttpPut("ticket/confirm-boarding/{ticketId}")]
        public async Task<IActionResult> ConfirmBoarding(string ticketId)
        {
            var success = await _ticketService.ConfirmBoardingAsync(ticketId);
            return success ? Ok("Boarding confirmed") : NotFound("Ticket not found");
        }


        // 4. Huỷ vé
        [HttpPut("ticket/cancel/{ticketId}")]
        public async Task<IActionResult> CancelTicket(string ticketId)
        {
            var result = await _ticketService.CancelTicketAsync(ticketId);
            if (result == null) return NotFound("Ticket not found");
            return Ok(result);
        }

        // 5. Kiểm tra ghế trống
        //[HttpGet("seat-availability/{tripId}")]
        //public async Task<IActionResult> GetSeatAvailability(string tripId)
        //{
        //    var result = await _seatService.GetSeatAvailabilityByTripIdAsync(tripId);
        //    return Ok(result);
        //}

        // 6. Xác nhận thanh toán
        //[HttpGet("transactions/by-customer/{customerId}")]
        //public async Task<IActionResult> GetTransactions(string customerId)
        //{
        //    var result = await _transactionService.GetByCustomerIdAsync(customerId);
        //    return Ok(result);
        //}

        // 7. Tìm kiếm khách hàng
        //[HttpGet("customer/{id}")]
        //public async Task<IActionResult> GetCustomer(string id)
        //{
        //    var customer = await _customerService.GetByIdAsync(id);
        //    if (customer == null) return NotFound();
        //    return Ok(customer);
        //}

        // 8. Lịch sử giao dịch
        //[HttpGet("transactions")]
        //public async Task<IActionResult> GetAllTransactions()
        //{
        //    var result = await _transactionService.GetAllAsync();
        //    return Ok(result);
        //}

        // 9. Xếp lịch tài xế (cập nhật trip)
        [HttpPut("assign-driver/{tripId}")]
        public async Task<IActionResult> AssignDriver(string tripId, [FromQuery] string driverId)
        {
            var updatedTrip = await _tripService.AssignDriverAsync(tripId, driverId);
            return updatedTrip != null ? Ok(updatedTrip) : NotFound("Trip not found");
        }


        // 10. Gửi tin nhắn đến khách hàng
        //[HttpPost("send-sms")]
        //public IActionResult SendSmsToCustomer([FromQuery] string phone, [FromQuery] string name, [FromQuery] string tripInfo)
        //{
        //    _smsService.SendBookingConfirmation(phone, name, tripInfo);
        //    return Ok("SMS sent");
        //}
    }
}
