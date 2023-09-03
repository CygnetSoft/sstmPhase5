using SSTM.Core.CourseDownload;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class CourseDownloadUserMap : EntityTypeConfiguration<CourseDownloadUser>
    {
        public CourseDownloadUserMap()
        {
            ToTable("CourseDownloadUser");
            HasKey(a => a.Id);
        }
    }
}
