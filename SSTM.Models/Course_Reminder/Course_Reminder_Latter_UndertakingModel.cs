using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.Course_Reminder
{
   public class Course_Reminder_Latter_UndertakingModel
    {
        [Key]
        public long Id { get; set; }
        public long? Course_id { get; set; }
        public string latter_content { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
