using System;

namespace SSTM.Models.CourseDocument
{
    public class CourseDocumentsListModel
    {
        public long Id { get; set; }
        public long? CourseId { get; set; }

        public string DocName { get; set; }
        public string Filename { get; set; }

        public bool isCompleted { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }

        public bool isApproved { get; set; }
        public bool isReassigned { get; set; }
        public bool isTraining { get; set; }
        public bool isPrinting { get; set; }
        public bool isDeveloper { get; set; }

        public string Remarks { get; set; }
        public string Suggestion { get; set; }
        public string ReferenceFile { get; set; }
        public string Version { get; set; }
        public string VersionDate { get; set; }

        public bool isRemarksCompleted { get; set; }
    }
}