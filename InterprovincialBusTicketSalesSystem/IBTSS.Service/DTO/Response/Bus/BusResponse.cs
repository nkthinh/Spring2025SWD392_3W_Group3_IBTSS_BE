using IBTSS.Service.DTO.Response.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Bus
{
    public class BusResponse
    {
        public string BusId { get; set; } = string.Empty;
        public int SeatCount { get; set; }
        public string BusType { get; set; } = string.Empty;
        public List<SeatAvailabilityResponse> Seats { get; set; } = new();
    }

}
