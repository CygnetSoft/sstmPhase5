using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.QPRequest
{
  public  class QPRequestModel
    {
        [Key]
        public long Id { get; set; }
        [Required(ErrorMessage = "Course name cannot be blank.")]
        public string CourseName { get; set; }
        [Required(ErrorMessage = "Course Code cannot be blank.")]
        public string CourseCode { get; set; }
        [Required(ErrorMessage = "Footer cannot be blank.")]
        public string EnterFooter { get; set; }
        [Required(ErrorMessage = "Developer cannot be blank.")]
        public string DeveloperName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }
      
    }
}
