using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Ticket
    {
        [Key]
        public string TicketId { get; set; } = string.Empty;

        [ForeignKey("Book")]
        public string BookId { get; set; } = string.Empty;

        [ForeignKey("Trip")]
        public string? TripId { get; set; }

        [ForeignKey("Seat")]
        public string? SeatId { get; set; }

        public bool isCancelled { get; set; }
        public bool IsDelete { get; set; }

        public int Price { get; set; }
        public DateTime CreatedAt { get; set; } // ✅ thêm
        public string Status { get; set; } = string.Empty; // ✅ thêm
        public virtual Book? Book { get; set; }
        public virtual Seat? Seat { get; set; }
        public virtual Trip? Trip { get; set; }
    }
}
