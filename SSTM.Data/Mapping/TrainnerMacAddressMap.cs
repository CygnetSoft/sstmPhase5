using SSTM.Core.TrainnerMacAddress;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
   
    public class TrainnerMacAddressMap : EntityTypeConfiguration<TrainnerMacAddress>
    {
        public TrainnerMacAddressMap()
        {
            ToTable("TrainnerMacAddress");
            HasKey(a => a.Id);
        }
    }
}
