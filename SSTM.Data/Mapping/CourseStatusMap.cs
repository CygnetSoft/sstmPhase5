using SSTM.Core.CourseStatus;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseStatusMap : EntityTypeConfiguration<CourseStatus>
    {
        public CourseStatusMap()
        {
            ToTable("CourseStatus");
            HasKey(a => a.Id);
        }
    }
}