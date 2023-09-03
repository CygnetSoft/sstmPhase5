using SSTM.Core.TrainerQPUpload;
using SSTM.Models.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ITrainerQP_Level_ApprovalService
    {
        TrainerQP_Level_Approval GetRecordById(long Id);
        TrainerQP_Level_Approval GetRecordByQPId(long Id);
        long SaveRecord(TrainerQP_Level_Approval entity);
        void DeleteRecord(long Id);
        IEnumerable<Trainer_qp_level_data_model> Trainer_QP_Level_Approval_data();
    }
}
