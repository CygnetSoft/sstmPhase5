using SSTM.Core.MainCourse;
using System.Data.Entity.ModelConfiguration;


namespace SSTM.Data.Mapping
{ 
    public class MainCourseMap : EntityTypeConfiguration<MainCourse>
    {
        public MainCourseMap()
        {
            ToTable("MainCourse");
            HasKey(a => a.Id);
        }
    }
}
