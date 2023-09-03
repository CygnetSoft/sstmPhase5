using SSTM.Core.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class TrainerQPUploadMap : EntityTypeConfiguration<TrainerQPUpload>
    {
        public TrainerQPUploadMap()
        {
            ToTable("TrainerQPUpload");
            HasKey(a => a.Id);
        }
    }
  
}
