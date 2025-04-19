using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Seat
{
    public class SeatSummaryResponse
    {
        public string BusId { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public List<SeatAvailabilityResponse> Seats { get; set; } = new();
    }
}
