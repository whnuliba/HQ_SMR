using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class HttpRequestEntity
    {
        public string? Uri { get; set; }
        public string? Method { get; set; }
        public Dictionary<string,string>? Header { get; set; }
        public string? Content { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public int Timeout { get; set; } = 6000;
        public bool Enable { get; set; } = false;
    }
}
