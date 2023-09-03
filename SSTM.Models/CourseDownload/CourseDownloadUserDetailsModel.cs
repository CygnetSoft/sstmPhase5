using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.CourseDownload
{
    public class CourseDownloadUserDetailsModel
    {

        public long Id { get; set; }
        public long CourseId { get; set; }
        public long User_id { get; set; }
        public string CourseName { get; set; }
    }
}
