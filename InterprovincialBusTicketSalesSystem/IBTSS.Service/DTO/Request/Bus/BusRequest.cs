using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Bus
{
    public class BusRequest
    {
        public string BusId { get; set; } = string.Empty;
        public int SeatCount { get; set; }
        public string BusType { get; set; } = string.Empty;
    }

}
