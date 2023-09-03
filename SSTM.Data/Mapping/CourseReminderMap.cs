using SSTM.Core.Course_Reminder;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    class CourseReminderMap: EntityTypeConfiguration<CourseReminder>
    {
        public CourseReminderMap()
        {
            ToTable("Course_Reminder");
            HasKey(a => a.Id);
        }
    }
}
