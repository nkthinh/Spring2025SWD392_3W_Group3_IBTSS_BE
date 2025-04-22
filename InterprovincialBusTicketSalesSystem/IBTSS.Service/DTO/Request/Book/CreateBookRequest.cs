using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Book
{
    public class CreateBookRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public string TripId { get; set; } = string.Empty;
        public List<string> Seats { get; set; } = new();
    }

}
