using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Trip
{
    public class QueryParameters
    {
        public string? Keyword { get; set; } // tìm theo RouteName, DriverId,...
        public string? SortBy { get; set; } = "date_desc"; // date_asc, price_asc, price_desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
