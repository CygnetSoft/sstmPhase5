using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Core.Course_Reminder
{
    public class CourseRenewal
    {
        public long Id { get; set; }
        public long? Course_id { get; set; }
        public DateTime? Renew_date { get; set; }
        public string Duration { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
