using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.CourseReminderDownload
{
    public class NeedAnalysis
    {
        public int? courseid { get; set; }
        public string ModuleTitle { get; set; }
        public string ModuleSyllabus { get; set; }
        public string Column1 { get; set; }
        public string CourseName { get; set; }
        public string type1 { get; set; }
        public string aplydate { get; set; }
        public string CourseDuration { get; set; }
        public string courselevel { get; set; }
    }
}
