using SSTM.Core.IntroPage;
using System.Data.Entity.ModelConfiguration;

namespace Data.Mapping
{
    public class StudentNotificationMap : EntityTypeConfiguration<StudentNotification>
    {
        public StudentNotificationMap()
        {
            ToTable("StudentNotification");
            HasKey(a => a.NotificationId);
        }
    }
}
