using SSTM.Core.TrainerQPUpload;
using SSTM.Models.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ITrainerQPSharedStudentService
    {
        IEnumerable<TrainerQP_Shared_Student> GetRecordList(long id);
        void SaveRecord(TrainerQP_Shared_Student entity);
        void DeleteRecord(long Id);
        List<TrainerStudentApiModel> GetQpToStundetList(string courseid, string batchid);
    }
}
