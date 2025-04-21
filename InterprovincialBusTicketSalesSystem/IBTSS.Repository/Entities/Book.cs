using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Book
    {
        [Key]
        public string BookId { get; set; } = string.Empty;

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        [ForeignKey("Transaction")]
        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; }
        public int TicketCount { get; set; }
        public int TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;

        public virtual Customer? Customer { get; set; }
        public virtual Transaction? Transaction { get; set; }

        // Quan hệ 1-N với Ticket
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }


}
