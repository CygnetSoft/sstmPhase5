using SSTM.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class QPRequestMap : EntityTypeConfiguration<QPRequest>
    {
        public QPRequestMap()
        {
            ToTable("QPRequest");
            HasKey(a => a.Id);
        }
    }
}
