using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.TrainerQPUpload
{
   public class TrainerStudentApiModel
    {
        public string course_id { get; set; }
        public string batch_id { get; set; }
        public string course_name { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }
    }
}
