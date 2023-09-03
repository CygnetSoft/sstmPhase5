using SSTM.Core.TrainerUploadDocument;
using SSTM.Models.TrainerUploadDocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ITrainerUploadDocumentService
    {
        TrainerUploadDocument GetRecordById(long Id);
        long SaveRecord(TrainerUploadDocument entity);
        void DeleteRecord(long Id);
        // IEnumerable<TrainerUploadDocument> GetUploadDocsList(int status);
        IEnumerable<TrainerUploadDataModel> GetUploadDocsList(int status);
        IEnumerable<TrainerUploadDocument> GetCommonUploadDocsList(int status, bool? MasterDoc, long? MasterDocId);
    }
}
