using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.StudentNotification
{
    public class StudentNotificationModel
    {      
        public Guid NotificationId { get; set; }
        public long StudentId { get; set; }
        public string NotificationType { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsSend { get; set; }
        public bool? IsRecieved { get; set; }
        public TimeSpan? SessionStartTime { get; set; }
        public TimeSpan? SessionExpiryTime { get; set; }
        public string Link { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
