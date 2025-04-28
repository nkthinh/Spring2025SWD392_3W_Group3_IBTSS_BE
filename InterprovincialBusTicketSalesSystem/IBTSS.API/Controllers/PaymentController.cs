using IBTSS.Service.DTO.Request.Payment;
using Microsoft.AspNetCore.Mvc;
using IBTSS.Service.Services.Payment;

namespace IBTSS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create-vnpay")]
        public IActionResult CreatePaymentUrlVnpay([FromBody] PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Ok(new { paymentUrl = url });
        }

        [HttpGet("vnpay-callback")]
        public IActionResult PaymentCallbackVnpay([FromQuery] string BookId)
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success)
            {
                // TODO: Cập nhật Transaction + Book thành "Đã Thanh Toán"
            }

            return Ok(response);
        }
    }
}
