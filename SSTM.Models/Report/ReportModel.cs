using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.Report
{
    public class ReportModel
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Report name cannot be blank.")]
        public string ReportName { get; set; }

        [Required(ErrorMessage = "Report description cannot be blank.")]
        public string ReportDesc { get; set; }

        [Required(ErrorMessage = "Report filter cannot be blank.")]
        public string ReportFilter { get; set; }      

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public bool IsActive { get; set; }
    }
}