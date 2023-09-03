using SSTM.Core.CourseSharing;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseSharingMap : EntityTypeConfiguration<CourseSharing>
    {
        public CourseSharingMap()
        {
            ToTable("CourseSharing");
            HasKey(a => a.Id);
        }
    }
}