using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Trip
{
    public class LocationStopDto
    {
        public string LocationName { get; set; } = string.Empty;
        public int StopOrder { get; set; }
        public TimeSpan? StopDuration { get; set; }
    }
}
