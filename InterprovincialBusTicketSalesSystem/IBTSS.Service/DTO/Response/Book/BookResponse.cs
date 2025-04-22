using IBTSS.Service.DTO.Response.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Book
{
    public class BookResponse
    {
        public string BookId { get; set; }
        public string CustomerId { get; set; }
        public int TicketCount { get; set; }
        public int TotalPrice { get; set; }
        public List<string> SeatIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }

        public List<TicketResponse>? Tickets { get; set; } // ✅ thêm nếu muốn
    }


}
