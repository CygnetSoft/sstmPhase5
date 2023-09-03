using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Core
{
    public class QPRequest
    {
        public long Id { get; set; }
        public string CourseName { get; set; }

        public string CourseCode { get; set; }
        public string EnterFooter { get; set; }
        public string DeveloperName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
