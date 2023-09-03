using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Core.Assessment_Paper
{
    public class AssessmentPaper
    {
        public int id { get; set; }
        public int? courseid { get; set; }
        public string student_fin { get; set; }
        public decimal? batchid { get; set; }
        public int? qty { get; set; }
        public string li_filename { get; set; }
        public string course_name { get; set; }
        public string batch_name { get; set; }
        public string filename { get; set; }
        public string trainer_id { get; set; }
        public string fin_number { get; set; }
        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
