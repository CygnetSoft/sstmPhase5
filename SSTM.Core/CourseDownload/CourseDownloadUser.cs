using System;

namespace SSTM.Core.CourseDownload
{
    public class CourseDownloadUser
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public long User_id { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
