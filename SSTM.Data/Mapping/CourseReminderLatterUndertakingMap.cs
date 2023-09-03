using SSTM.Core.Course_Reminder;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
  
    public class CourseReminderLatterUndertakingMap : EntityTypeConfiguration<Course_Reminder_Latter_Undertaking>
    {
        public CourseReminderLatterUndertakingMap()
        {
            ToTable("Course_Reminder_Latter_Undertaking");
            HasKey(a => a.Id);
        }
    }
}
