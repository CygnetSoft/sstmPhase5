using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.TrainerQPUpload
{
   public class TrainerQP_Shared_StudentModel
    {
        [Key]
        public long id { get; set; }
        public string course_id { get; set; }
        public string batch_id { get; set; }
        public long QP_id { get; set; }
        public string course_name { get; set; }
        public string batch_name { get; set; }
    }
}
