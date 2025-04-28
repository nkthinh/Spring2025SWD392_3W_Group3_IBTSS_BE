using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Payment
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; } = "other";
        public double Amount { get; set; }
        public string OrderDescription { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
    }
}
