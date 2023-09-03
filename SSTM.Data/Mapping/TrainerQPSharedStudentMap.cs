using SSTM.Core.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    class TrainerQPSharedStudentMap : EntityTypeConfiguration<TrainerQP_Shared_Student>
    {
        public TrainerQPSharedStudentMap()
        {
            ToTable("TrainerQP_Shared_Student");
            HasKey(a => a.id);
        }
    }
}
