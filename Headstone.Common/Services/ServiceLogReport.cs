using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Services
{
    public class ServiceLogRecord
    {
        public string Type { get; set; }

        public string Body { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Process { get; set; }

        public string ThreadId { get; set; }
    }
}
