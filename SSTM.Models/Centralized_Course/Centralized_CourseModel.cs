using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.Centralized_Course
{
    public class Centralized_CourseModel
    {
        [Key]
        public string id { get; set; }
        public int? center_master_id { get; set; }
        public string master_text { get; set; }
        public string replace_text { get; set; }
        public string type { get; set; }
        public string textimage { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
