using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Trip
    {
        [Key]
        public string TripId { get; set; } = string.Empty;

        [ForeignKey("Route")]
        public string RouteId { get; set; } = string.Empty;

        [ForeignKey("Bus")]
        public string BusId { get; set; } = string.Empty;

        [ForeignKey("Driver")]
        public string DriverId { get; set; } = string.Empty;

        public TimeOnly DepartureTime { get; set; }


        public string Date { get; set; } = string.Empty;

        public string Direction { get; set; } = string.Empty;

        public bool IsDelete { get; set; }

        public int Price { get; set; }
        public string Status { get; set; }

        public virtual Route? Route { get; set; }
        public virtual Bus? Bus { get; set; }
        public virtual User? Driver { get; set; }
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
