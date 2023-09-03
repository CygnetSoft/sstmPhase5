using SSTM.Core.SubCourse;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class SubCourseMap : EntityTypeConfiguration<SubCourse>
    {
        public SubCourseMap()
        {
            ToTable("SubCourse");
            HasKey(a => a.Id);
        }
    }
}
