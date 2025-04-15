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
        public string TransactionId { get; set; } = string.Empty;

        [ForeignKey("Ticket")]
        public string TicketId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public int Amount { get; set; }

        public virtual Ticket? Ticket { get; set; }
    }
}
