using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Transaction
{
    public class TransactionResponse
    {
        public string TransactionId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}
