using SSTM.Core.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{

    public class TrainerQP_Level_ApprovalMap : EntityTypeConfiguration<TrainerQP_Level_Approval>
    {
        public TrainerQP_Level_ApprovalMap()
        {
            ToTable("TrainerQP_Level_Approval");
            HasKey(a => a.id);
        }
    }

}
