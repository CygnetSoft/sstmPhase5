using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.CourseDocument
{
    public class CourseDocumentModel
    {
        [Key]
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
        public bool isOpened { get; set; }

        public long UserId { get; set; }
        public string isOldDocument { get; set; }
    }
}