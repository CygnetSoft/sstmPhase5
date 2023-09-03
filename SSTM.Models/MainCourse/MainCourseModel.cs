using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.MainCourseModel
{
   public class MainCourseModel
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Sub Course name cannot be blank.")]
        public string CourseName { get; set; }
        public string CourseType { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
