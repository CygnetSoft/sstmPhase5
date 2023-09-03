using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.Central_CourseSharing
{
   public class Central_SharedCourseListModel
    {
        public long? CourseId { get; set; }
        public long? DocId { get; set; }

        public string CourseName { get; set; }
        public string DocName { get; set; }

        public bool? isTraining { get; set; }
        public bool? isPrinting { get; set; }
        public bool? isDeveloper { get; set; }
    }
}
