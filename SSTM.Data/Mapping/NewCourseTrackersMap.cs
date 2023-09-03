using SSTM.Core.CourseTrackers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{

    public class NewCourseTrackersMap : EntityTypeConfiguration<NewCourseTrackingData>
    {
        public NewCourseTrackersMap()
        {
            ToTable("Course_Reminder_Tracker");
            HasKey(a => a.Id);
        }
    }

}
