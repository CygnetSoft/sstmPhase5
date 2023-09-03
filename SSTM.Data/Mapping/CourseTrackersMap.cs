using SSTM.Core.CourseTrackers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class CourseTrackersMap : EntityTypeConfiguration<CourseTrackers>
    {
        public CourseTrackersMap()
        {
            ToTable("CourseTrackers");
            HasKey(a => a.Id);
        }
    }
}
