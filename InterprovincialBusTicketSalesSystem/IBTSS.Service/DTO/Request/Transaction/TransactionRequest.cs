using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Transaction
{
    public class TransactionRequest
    {
        public string TicketId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}
