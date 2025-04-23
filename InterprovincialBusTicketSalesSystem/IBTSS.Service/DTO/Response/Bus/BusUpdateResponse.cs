using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Bus
{
    public class BusUpdateResponse
    {
        public int SeatCount { get; set; }
        public string BusType { get; set; } = string.Empty;
        public string? Model { get; set; }
        public int? ModelYear { get; set; }
        public string? Color { get; set; }
        public bool IsDelete { get; set; }
    }

}
