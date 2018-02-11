using System.Collections.Generic;
using Aiplugs.Functions.Core;

namespace Sample.Models
{
    public class SampleViewModel
    {
        public IJob Current { get; set; }
        public IEnumerable<IJob> History { get; set; }
    }
}