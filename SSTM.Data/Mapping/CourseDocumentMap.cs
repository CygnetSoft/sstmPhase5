using SSTM.Core.CourseDocument;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseDocumentMap : EntityTypeConfiguration<CourseDocument>
    {
        public CourseDocumentMap()
        {
            ToTable("CourseDocument");
            HasKey(a => a.Id);
        }
    }
}