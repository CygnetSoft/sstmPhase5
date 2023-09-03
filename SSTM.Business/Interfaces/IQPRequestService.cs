using SSTM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
   public interface IQPRequestService
    {
        QPRequest GetFirstRecord();
        List<QPRequest> GetAllRecord();
        QPRequest GetRecordById(long Id);
        void SaveRecord(QPRequest entity);
        void DeleteRecord(long Id);
    }
}
