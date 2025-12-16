using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShopper.Application.Dtos.Dev
{
    public class ExternalProductResponse
    {
        public List<CloneProductDto> data { get; set; } = new();
        public PaginateDto paginate { get; set; } = default!;
    }
    public class CloneProductDto
    {
        public string _id { get; set; } = default!;
        public long id { get; set; }
        public int categories { get; set; }
        public string name { get; set; } = default!;
        public decimal price { get; set; }
        public decimal real_price { get; set; }
        public string slug { get; set; } = default!;
        public string description { get; set; } = default!;
    }
    public class PaginateDto
    {
        public int currentPage { get; set; }
        public int totalPage { get; set; }
        public int count { get; set; }
        public int perPage { get; set; }
        public int nextPage { get; set; }
    }
}
