using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.LocationRoute
{
  public class LocationRouteResponse
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty; // Tên của Location
    public int StopOrder { get; set; } // Thứ tự điểm dừng
    public TimeSpan? StopDuration { get; set; } // Thời gian nghỉ tại điểm dừng
}
}
