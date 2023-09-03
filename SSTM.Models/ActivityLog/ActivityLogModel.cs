using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.ActivityLog
{
    public class ActivityLogModel
    {
        [Key]
        public long Id { get; set; }

        public string URL { get; set; }
        public string Duration { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long? UserId { get; set; }

        public string IPAddress { get; set; }
    }
}