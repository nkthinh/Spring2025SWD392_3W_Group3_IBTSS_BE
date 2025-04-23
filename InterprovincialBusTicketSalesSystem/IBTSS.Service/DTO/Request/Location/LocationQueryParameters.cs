using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Location
{
    public class LocationQueryParameters
    {
        public string? Keyword { get; set; }
        public string? SortBy { get; set; } = "name_asc"; // name_asc, name_desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
