using IBTSS.Service.DTO.Response.LocationRoute;
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
        public string RouteName { get; set; } = string.Empty; // ✅ thêm
        public string BusId { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty; // ✅ thêm
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty; // ✅ thêm tên tài xế
        public string DepartureTime { get; set; } = string.Empty; // ✅ chuyển TimeOnly -> string
        public string Date { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public bool IsDelete { get; set; }
        public int Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<LocationRouteResponse> LocationRoutes { get; set; } = new();
    }

}
