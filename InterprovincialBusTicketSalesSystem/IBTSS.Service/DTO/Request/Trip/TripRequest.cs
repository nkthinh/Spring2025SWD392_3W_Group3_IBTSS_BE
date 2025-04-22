using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBTSS.Service.DTO.Request.LocationRoute;

namespace IBTSS.Service.DTO.Request.Trip
{
    public class TripRequest
    {
        public string RouteId { get; set; } = string.Empty;
        public string BusId { get; set; } = string.Empty;
        public string DriverId { get; set; } = string.Empty;
        public string DepartureTime { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public bool IsDelete { get; set; }
        public int Price { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<LocationRouteRequest> LocationRoutes { get; set; } = new(); // Danh sách điểm dừng
    }
}
