using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Bus
    {
        [Key]
        public string BusId { get; set; } = string.Empty;

        public int SeatCount { get; set; }

        public string BusType { get; set; } = string.Empty;
        public string? Model { get; set; } // ✅ Dòng xe
        public int? ModelYear { get; set; } // ✅ Đời xe (năm sản xuất)
        public string? Color { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<Trip>? Trips { get; set; }
        public virtual ICollection<Seat>? Seats { get; set; }
    }
}
