using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Ticket
{
    public class RouteBookingStatisticResponse
    {
        public string RouterName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
