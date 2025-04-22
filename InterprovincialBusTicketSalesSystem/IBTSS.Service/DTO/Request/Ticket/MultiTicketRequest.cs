using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Ticket
{
    public class MultiTicketRequest
    {
        public string BookId { get; set; } = string.Empty;
        public string TripId { get; set; } = string.Empty;
        public List<string> SeatIds { get; set; } = new();
        public int Price { get; set; }
    }
}
