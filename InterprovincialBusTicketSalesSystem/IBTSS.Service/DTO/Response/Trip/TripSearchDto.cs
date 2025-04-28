using System;
using System.Collections.Generic;

namespace IBTSS.Service.DTO.Response.Trip
{
    public class TripSearchDto
    {
        public string TripId { get; set; } = string.Empty;
        public string BusId { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty;

        public string RouteName { get; set; } = string.Empty;

        // Sử dụng DateTime cho DepartureTime và Date
        public string DepartureTime { get; set; } = string.Empty; // ✅ chuyển TimeOnly -> string
        public string Date { get; set; } = string.Empty;

        public decimal Price { get; set; }

        // Stops sẽ được khởi tạo dưới dạng danh sách rỗng
        public List<LocationStopDto> Stops { get; set; } = new List<LocationStopDto>();

        // Phương thức để trả về chuỗi đã định dạng cho DepartureTime và Date
        //public string GetFormattedDepartureTime()
        //{
        //    return DepartureTime.ToString("HH:mm");  // Ví dụ định dạng: 00:00
        //}

        //public string GetFormattedDate()
        //{
        //    return Date.ToString("dd-MM-yyyy");  // Ví dụ định dạng: 25-04-2025
        //}
    }
}
