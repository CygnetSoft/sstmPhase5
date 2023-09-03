using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.Centralized_Course
{
   public class Centralized_List_MasterModel
    {
        public int? id { get; set; }
        public string CentralDocumentName { get; set; }
        public string document_type { get; set; }
        public string choose_type { get; set; }
        public string language { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public long? Statusid { get; set; }
        public string Developer { get; set; }
        public string SME { get; set; }
        public string CourseStatus { get; set; }
        public string comment { get; set; }
        public string sme_comment { get; set; }
        public string developer_sme_comment_reply { get; set; }
        public long? DirectorId { get; set; }
        public string FolderNameInput { get; set; }
        public string FolderNameOutput { get; set; }
        public long? AirLineCourseId { get; set; }
    }
}
