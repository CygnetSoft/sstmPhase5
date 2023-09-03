using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSTM.Core.IntroPage;

namespace Data.Mapping
{
   public class StudentQP_WrittenMap : EntityTypeConfiguration<StudentQP_Written>
    {
        public StudentQP_WrittenMap()
        {
            ToTable("StudentQP_Written");
            HasKey(a => a.StudentQpWrittenId);
        }
    }
}
