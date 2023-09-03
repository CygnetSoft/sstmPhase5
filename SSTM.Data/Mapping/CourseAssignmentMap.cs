using SSTM.Core.CourseAssignment;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseAssignmentMap : EntityTypeConfiguration<CourseAssignment>
    {
        public CourseAssignmentMap()
        {
            ToTable("CourseAssignment");
            HasKey(a => a.Id);
        }
    }
}