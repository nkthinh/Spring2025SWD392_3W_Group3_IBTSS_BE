using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Customer
{
    public class CustomerRequest
    {

        public string PhoneNumber { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
