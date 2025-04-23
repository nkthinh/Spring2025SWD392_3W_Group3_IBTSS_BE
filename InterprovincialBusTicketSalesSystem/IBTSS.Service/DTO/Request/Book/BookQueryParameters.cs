using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Request.Book
{
    public class BookQueryParameters
    {
        public string? Keyword { get; set; } // tìm theo tên customer
        public string? SortBy { get; set; } = "date_desc"; // date_asc | date_desc | id_asc | id_desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
