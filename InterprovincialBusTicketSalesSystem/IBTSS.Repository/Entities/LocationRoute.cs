using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class LocationRoute
    {
        [Key]
        public string LocationRouteId { get; set; } = string.Empty;

        [ForeignKey("Location")]
        public string LocationId { get; set; } = string.Empty;

        [ForeignKey("Route")]
        public string RouteId { get; set; } = string.Empty;

        public virtual Location? Location { get; set; }
        public virtual Route? Route { get; set; }
    }
}
