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

        [ForeignKey("Trip")]
        public string TripId { get; set; } = string.Empty;

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        [ForeignKey("Seat")]
        public string SeatId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsCancelled { get; set; }

        public bool IsDelete { get; set; }

        public int Price { get; set; }
        public string Status { get; set; }

        public virtual Trip? Trip { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Seat? Seat { get; set; }
        public virtual ICollection<Transaction>? Transactions { get; set; }
    }
}
