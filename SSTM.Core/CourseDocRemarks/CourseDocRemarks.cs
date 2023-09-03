using System;

namespace SSTM.Core.CourseDocRemarks
{
    public class CourseDocRemarks
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public long DocId { get; set; }

        public string Remarks { get; set; }
        public string Suggestion { get; set; }
        public string ReferenceFile { get; set; }

        public bool isCompleted { get; set; }
        public bool isDeleted { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}