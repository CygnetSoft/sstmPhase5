using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.CourseStatus
{
    public class CourseStatusModel
    {
        [Key]
        public long Id { get; set; }

        public string Status { get; set; }

        public bool isDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }
    }
}