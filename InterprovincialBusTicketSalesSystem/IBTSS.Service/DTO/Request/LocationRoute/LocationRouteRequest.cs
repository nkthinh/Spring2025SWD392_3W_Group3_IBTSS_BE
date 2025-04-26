using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.LocationRoute
{
    public class LocationRouteRequest
    {
        public string LocationId { get; set; } // ID của location

        public int StopDurationMinutes { get; set; } // Thời gian nghỉ tại điểm dừng (phút)
    }
}
