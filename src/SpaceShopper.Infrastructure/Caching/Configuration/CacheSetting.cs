using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShopper.Infrastructure.Caching.Configuration
{
    public class CacheSetting
    {
        public const string SectionName = "Redis";

        public string ConnectionString { get; set; } = string.Empty;
        public int DatabaseId { get; set; } = 0;
        public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan AbsoluteExpiration { get; set; } = TimeSpan.FromHours(6);
    }
}
