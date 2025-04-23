using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Customer
    {
        [Key]
        public string CustomerId { get; set; } = string.Empty;

        public int Score { get; set; }

        [ForeignKey("Membership")]
        public string? MembershipId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public int? DiscountQuotaLeft { get; set; } // số lượng vé còn được giảm

        public virtual Membership? Membership { get; set; }
        public virtual ICollection<Book>? Books { get; set; }
    }
}
