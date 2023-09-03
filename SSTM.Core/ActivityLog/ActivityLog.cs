using System;

namespace SSTM.Core.ActivityLog
{
    public class ActivityLog
    {
        public long Id { get; set; }

        public string URL { get; set; }
        public string Duration { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? UserId{ get; set; }

        public string IPAddress { get; set; }
    }
}