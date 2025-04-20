using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Seat
    {
        [Key]
        public string SeatId { get; set; } = string.Empty;

        [ForeignKey("Bus")]
        public string BusId { get; set; } = string.Empty;

        public bool IsDelete { get; set; }
        public bool IsBooked { get; set; }

        public virtual Bus? Bus { get; set; }
        public virtual Ticket? Ticket { get; set; }


    }
}
