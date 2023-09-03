using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.Course
{
  public  class DashbordCourseCountModel
    {
        public int TotalCourse { get; set; }
        public int ReleaseCourse { get; set; }
        public int PendingDeveloperCourse { get; set; }
        public int PendingSMECourse { get; set; }
        public int PendinSharingCourse { get; set; }
        public int PendinRenewalCourse { get; set; }
        public int PendingNewCourse { get; set; }
    }
}
