using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Ticket
{
    public class TicketResponse
    {
        public string TicketId { get; set; }
        public string BookId { get; set; }
        public string? TripId { get; set; }
        public string? SeatId { get; set; }
        public int Price { get; set; }
        public int OriginalPrice { get; set; } // ✅ Thêm giá gốc
        public bool IsCancelled { get; set; }
        public string? CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
