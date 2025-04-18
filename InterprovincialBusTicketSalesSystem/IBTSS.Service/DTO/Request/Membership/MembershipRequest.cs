using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Membership
{
    public class MembershipRequest
    {
        public string RankName { get; set; }
        public int MinTicketsRequired { get; set; }
        public float DiscountRate { get; set; }
        public string Description { get; set; }
    }
}
