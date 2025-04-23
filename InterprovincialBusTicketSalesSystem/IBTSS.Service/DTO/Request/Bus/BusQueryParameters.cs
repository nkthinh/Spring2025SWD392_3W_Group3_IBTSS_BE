using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Bus
{
    public class BusQueryParameters
    {
        public string? Keyword { get; set; } // Tìm theo tên xe, biển số,...
        public string? SortBy { get; set; } = "id_asc"; // hoặc model_asc, model_desc,...
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
