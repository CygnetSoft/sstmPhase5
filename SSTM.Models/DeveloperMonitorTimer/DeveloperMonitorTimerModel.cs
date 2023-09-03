using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.Developer_Monitor_Timer
{
    public class DeveloperMonitorTimerModel
    {
        [Key]
        public long id { get; set; }
        public long? user_id { get; set; }
        public TimeSpan? in_time { get; set; }
        public TimeSpan? out_time { get; set; }
        public DateTime date_time { get; set; }
    }
}
