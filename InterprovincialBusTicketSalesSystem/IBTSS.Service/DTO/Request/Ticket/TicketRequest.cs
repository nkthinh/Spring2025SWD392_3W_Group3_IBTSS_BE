using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Ticket
{
    public class TicketRequest
    {
        public string TripId { get; set; } = string.Empty;
        public string SeatId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty; // ✅ thêm
        public string Status { get; set; } = "Pending";    // ✅ thêm nếu cần cập nhật status
    }

}
