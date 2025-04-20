using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Route
{
    public class RouteResponse
    {
        public string RouteId { get; set; } = string.Empty;

        public string RouteName { get; set; } = string.Empty;

        public int Distance { get; set; }

        public int EstimatedDuration { get; set; }

        public bool IsDelete { get; set; }
    }
}
