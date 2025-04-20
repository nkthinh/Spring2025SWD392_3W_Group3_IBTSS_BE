using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.LocationRoute
{
    public class LocationRouteRequest
    {
        public string LocationId { get; set; } = string.Empty; // ID của location
        public int StopOrder { get; set; } // Thứ tự điểm dừng
        public int StopDurationMinutes { get; set; } // Thời gian nghỉ tại điểm dừng (phút)
    }
}
