using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServices.Models
{
    public class WorkserSettings
    {
        public string? LocalIp { get; set; }
        public int LocalPort { get; set; }
        public string? RemoteIp { get; set; }
        public int RemotePort { get; set; }
    }
}
