using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Trip
{
    public class TripSearchDto
    {
        public string RouteName { get; set; }
        public List<string> LocationNames { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public string Date { get; set; }
        public decimal Price { get; set; }
    }


}
