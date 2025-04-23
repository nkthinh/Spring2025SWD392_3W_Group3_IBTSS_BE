using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.Customer
{
    public class CustomerResponseByTrip
    {
        public string CustomerId { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? RankName { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
