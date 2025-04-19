using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Trip
{
    public class TripResponse
    {
        public string TripId { get; set; } = string.Empty;


        public string RouteId { get; set; } = string.Empty;


        public string BusId { get; set; } = string.Empty;


        public string DriverId { get; set; } = string.Empty;

        public TimeOnly DepartureTime { get; set; }

        public string Date { get; set; } = string.Empty;

        public string Direction { get; set; } = string.Empty;

        public bool IsDelete { get; set; }

        public int Price { get; set; }
        public string Status { get; set; }
    }
}
