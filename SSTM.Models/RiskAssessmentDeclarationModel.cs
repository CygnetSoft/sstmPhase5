using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models
{
    public class RiskAssessmentDeclarationModel
    {
        public long Id { get; set; }
        public int CourseId { get; set; }
        public int BatchId { get; set; }
        public long? StudentId { get; set; }
        public long? TrainerId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
