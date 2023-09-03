using SSTM.Core.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class CentralizedHistoryMap : EntityTypeConfiguration<Centralized_History>
    {
        public CentralizedHistoryMap()
        {
            ToTable("Centralized_History");
            HasKey(a => a.id);
        }
    }
}
