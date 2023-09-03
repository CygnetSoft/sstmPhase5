using System;

namespace SSTM.Core.Config
{
    public class Report
    {       
        public long Id { get; set; }        
        public string ReportName { get; set; }        
        public string ReportDesc { get; set; }        
        public string ReportFilter { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}