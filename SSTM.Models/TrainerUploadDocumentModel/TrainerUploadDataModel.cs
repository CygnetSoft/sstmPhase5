using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.TrainerUploadDocumentModel
{
   public class TrainerUploadDataModel
    {
        public long Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public int Status { get; set; }
        public long? TrainerId { get; set; }
        public string TrainerName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public bool? MasterDoc { get; set; }
        public long? MasterDocId { get; set; }

        public long? UpdatedBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
