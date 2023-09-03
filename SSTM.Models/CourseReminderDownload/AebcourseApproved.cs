using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.CourseReminderDownload
{
    public class AebcourseApproved
    {
        public int? AEBID { get; set; }
        public int? courseid { get; set; }
        public string CourseName { get; set; }
        public string courseshortname { get; set; }
        public string DateS { get; set; }
    }
}
