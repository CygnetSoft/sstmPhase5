using System.Data.Entity.ModelConfiguration;
using SSTM.Core.IntroPage;

namespace Data.Mapping
{
    public class StudentIntroPageMap : EntityTypeConfiguration<StudentIntroPage>
    {
        public StudentIntroPageMap()
        {
            ToTable("StudentIntroPage");
            HasKey(a => a.StudentIntroPageId);
        }
    }
}
