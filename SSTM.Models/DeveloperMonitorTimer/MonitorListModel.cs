using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.DeveloperMonitorTimer
{
    public class MonitorListModel
    {
        public long id { get; set; }
        public long? user_id { get; set; }
        public string username { get; set; }
        public TimeSpan? in_time { get; set; }
        public TimeSpan? out_time { get; set; }
        public string totaltime { get; set; }
        public string date { get; set; }
    }
}
