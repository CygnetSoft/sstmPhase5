using System;

namespace SSTM.Models.Course
{
    public class CourseListModel
    {
        public long Id { get; set; }

        public string CourseName { get; set; }
        public long? NewCourseId { get; set; }
        public bool? MasterCourse { get; set; }
        public long? MasterCoursId { get; set; }
        public int? NumberOfDocs { get; set; }

        public bool isActive { get; set; }
        public bool isDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }

        public long? Statusid { get; set; }

        public string Developer { get; set; }
        public string SME { get; set; }
        public string CourseType { get; set; }
        public string CourseStatus { get; set; }
        public long? DeveloperId { get; set; }
        public string renewal_reminder { get; set; }
        public int? reminder_days { get; set; }
        public DateTime? renew_date { get; set; }
        public DateTime? reminder_created_date { get; set; }
    }
}