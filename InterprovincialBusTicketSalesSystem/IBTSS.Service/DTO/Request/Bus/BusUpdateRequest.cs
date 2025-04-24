using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Bus
{
    public class BusUpdateRequest
    {
        public int SeatCount { get; set; }
        public string BusType { get; set; } = string.Empty;
        public string? Model { get; set; }
        public int? ModelYear { get; set; }
        public string? Color { get; set; }
        public List<string>? SeatIdsToRemove { get; set; }
    }

}
