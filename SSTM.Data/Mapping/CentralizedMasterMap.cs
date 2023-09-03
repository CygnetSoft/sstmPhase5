using SSTM.Core.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class CentralizedMasterMap : EntityTypeConfiguration<Centralized_Master>
    {
        public CentralizedMasterMap()
        {
            ToTable("Centralized_Master");
            HasKey(a => a.id);
        }
    }
  
}
