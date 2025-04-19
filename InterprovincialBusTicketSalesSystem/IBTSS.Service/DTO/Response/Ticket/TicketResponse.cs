using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Ticket
{
    public class TicketResponse
    {
        public string TicketId { get; set; } = string.Empty;
        public string TripId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string SeatId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsCancelled { get; set; }
        public int Price { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
