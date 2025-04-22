using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Route
{
    public class RouteRequest
    {

        public string RouteName { get; set; } = string.Empty;

        public int Distance { get; set; }

        public int EstimatedDuration { get; set; }

    }
}
