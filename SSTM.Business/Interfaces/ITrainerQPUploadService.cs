using SSTM.Core.TrainerQPUpload;
using SSTM.Models.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ITrainerQPUploadService
    {
        TrainerQPUpload GetRecordById(long Id);
        long SaveRecord(TrainerQPUpload entity);
        void DeleteRecord(long Id);
        IEnumerable<TrainerQPUploadDataModel> GetAllQPUploadDocsList(long trainerId,long SmeId);
    }
}
