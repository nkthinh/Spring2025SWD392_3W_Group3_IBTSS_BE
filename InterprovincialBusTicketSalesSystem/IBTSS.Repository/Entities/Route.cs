using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Route
    {
        [Key]
        public string RouteId { get; set; } = string.Empty;

        public string RouteName { get; set; } = string.Empty;

        public int Distance { get; set; }

        public int EstimatedDuration { get; set; }

        public bool IsDelete { get; set; }

        public virtual ICollection<LocationRoute>? LocationRoutes { get; set; }
        public virtual ICollection<Trip>? Trips { get; set; }
    }
}
