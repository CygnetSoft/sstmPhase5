using System.Data.Entity.ModelConfiguration;
using SSTM.Core.IntroPage;

namespace Data.Mapping
{
    public class StudentQpMap : EntityTypeConfiguration<StudentQP>
    {
        public StudentQpMap()
        {
            ToTable("StudentQP");
            HasKey(a => a.StudentQpId);
        }
    }
}
