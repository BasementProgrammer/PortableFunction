using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support
{
    public class UniversalRecord
    {
        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public string ObjectSize { get; set; }
        public string EventTime { get; set; }
        public string EventName { get; set; }
        public string SourceIPAddress { get; set; }
    }
}
