using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Trip
{
    public class TripSearchDto
    {
        public string RouteName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string Date { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public List<LocationStopDto> Stops { get; set; } = new();
    }


}
