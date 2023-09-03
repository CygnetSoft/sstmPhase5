using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.SubCourse
{
    public class SubCourseModel
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Sub Course name cannot be blank.")]
        public string SubCourseName { get; set; }
        public long MainCourseId { get; set; }
        public string CourseType { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
