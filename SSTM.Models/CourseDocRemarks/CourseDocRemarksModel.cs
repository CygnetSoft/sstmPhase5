using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Models.CourseDocRemarks
{
    public class CourseDocRemarksModel
    {
        [Key]
        public long Id { get; set; }
        public long CourseId { get; set; }
        public long DocId { get; set; }

        [AllowHtml, Required(ErrorMessage = "Remarks cannot be blank. If there are no remarks to give then type N/A.")]
        public string Remarks { get; set; }

        [AllowHtml, Required(ErrorMessage = "Suggetion cannot be blank. If there are no suggestion to give then type N/A.")]
        public string Suggestion { get; set; }
        public string ReferenceFile { get; set; }

        public bool isCompleted { get; set; }
        public bool isDeleted { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public HttpPostedFileBase ReferenceDoc { get; set; }

        public string DocName { get; set; }
    }
}