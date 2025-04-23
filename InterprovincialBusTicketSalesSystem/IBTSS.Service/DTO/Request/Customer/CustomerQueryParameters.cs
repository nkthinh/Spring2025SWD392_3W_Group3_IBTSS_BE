using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Customer
{
    public class CustomerQueryParameters
    {
        public string? Keyword { get; set; } // tìm theo tên hoặc số điện thoại
        public string? SortBy { get; set; } = "name_asc"; // name_asc, name_desc, score_desc,...
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
