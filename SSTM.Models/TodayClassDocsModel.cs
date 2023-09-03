using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models
{
    public class TodayClassDocsModel
    {
        public string dateofbatch { get; set; }
        public int? Courseid { get; set; }
        public string coursename { get; set; }
        public string section1Trainer { get; set; }
        public string trainername { get; set; }
        public string fin { get; set; }
        public string section1SectionName { get; set; }
        public  double? batchid { get; set; }
        public long? trainerID { get; set; }
    }
}
