using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.TrainerQPUpload
{
   public class TrainerQPUploadDataModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public bool isShared { get; set; }
        public long TrainerId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public long? SMEId { get; set; }
    }
}
