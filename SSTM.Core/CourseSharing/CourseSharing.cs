using System;

namespace SSTM.Core.CourseSharing
{
    public class CourseSharing
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public long DocId { get; set; }

        public bool isTraining { get; set; }
        public bool isPrinting { get; set; }
        public bool isDeleted { get; set; }
        public bool isDeveloper { get; set; }
        public long? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}