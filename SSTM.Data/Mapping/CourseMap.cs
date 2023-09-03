using SSTM.Core.Course;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseMap : EntityTypeConfiguration<Course>
    {
        public CourseMap()
        {
            ToTable("Course");
            HasKey(a => a.Id);
        }
    }
}