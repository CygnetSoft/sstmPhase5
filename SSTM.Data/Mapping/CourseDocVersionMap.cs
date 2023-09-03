using SSTM.Core.CourseDocVersion;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseDocVersionMap : EntityTypeConfiguration<CourseDocVersion>
    {
        public CourseDocVersionMap()
        {
            ToTable("CourseDocVersion");
            HasKey(a => a.Id);
        }
    }
}