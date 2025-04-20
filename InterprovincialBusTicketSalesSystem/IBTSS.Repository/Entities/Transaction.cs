using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Transaction
    {
        [Key]
        public string? TransactionId { get; set; } = string.Empty;

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public int Amount { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Customer? Customer { get; set; }

        // ✅ 1 transaction có thể có nhiều ticket
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }

}
