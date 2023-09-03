using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.Centralized_Course
{
  public  class Centralized_HistoryWithDataModel
    {
        public int id { get; set; }
        public int? center_master_id { get; set; }
        public string master_text { get; set; }
        public string replace_text { get; set; }
        public string UserName { get; set; }
        public string type { get; set; }
        public string textimage { get; set; }
        public int? is_active { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public long? Version { get; set; }
        public DateTime? VersionDate { get; set; }
    }
}
