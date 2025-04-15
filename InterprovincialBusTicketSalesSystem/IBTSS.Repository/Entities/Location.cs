using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Location
    {
        [Key]
        public string LocationId { get; set; } = string.Empty;

        public string LocationName { get; set; } = string.Empty;

        public bool IsDelete { get; set; }

        public string RouteId { get; set; } = string.Empty;
    }
}
