using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class Membership
    {
        [Key]
        public string MembershipId { get; set; } = string.Empty;

        public string RankName { get; set; } = string.Empty;

        public int MinTicketsRequired { get; set; }

        public float DiscountRate { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public virtual ICollection<Customer>? Customers { get; set; }
    }
}
