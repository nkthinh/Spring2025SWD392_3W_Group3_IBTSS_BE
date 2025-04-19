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
        public string CustomerId { get; set; } = string.Empty;
        public string SeatId { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsCancelled { get; set; }
    }
}
