using SSTM.Core.CourseDocRemarks;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseDocRemarksMap : EntityTypeConfiguration<CourseDocRemarks>
    {
        public CourseDocRemarksMap()
        {
            ToTable("CourseDocRemarks");
            HasKey(a => a.Id);
        }
    }
}