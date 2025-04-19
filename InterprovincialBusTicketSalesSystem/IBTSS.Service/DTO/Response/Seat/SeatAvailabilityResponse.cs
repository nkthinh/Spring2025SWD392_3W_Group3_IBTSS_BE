using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Seat
{
    public class SeatAvailabilityResponse
    {
        public string SeatId { get; set; } = string.Empty;
        public bool IsBooked { get; set; }
    }
}
