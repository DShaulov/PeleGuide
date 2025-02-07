using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeleGuide
{
    public class SearchResult
    {
        public string FilePath { get; set; }
        public int PageNumber { get; set; }
        public string MatchedText { get; set; }
        public string Context { get; set; }
    }
}
