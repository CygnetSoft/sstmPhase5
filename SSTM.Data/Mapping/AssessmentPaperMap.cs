using SSTM.Core.Assessment_Paper;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
  public  class AssessmentPaperMap : EntityTypeConfiguration<AssessmentPaper>
    {
        public AssessmentPaperMap()
        {
            ToTable("Assessment_Paper");
            HasKey(a => a.id);
        }
    }
}
