using SSTM.Core.Centralized_Course;
using SSTM.Models.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICentralizedHistoryService
    {
        Centralized_History GetRecordById(long Id);
        List<Centralized_History> GetAllRecord();
        Centralized_History GetRecordNewcourseById(long Id);
        long SaveRecord(Centralized_History entity);
        void DeleteRecord(long Id);
        List<Centralized_History> GetAllbyCenterRecord(long Central_Master_Id);
        List<Centralized_HistoryWithDataModel> GetHistory_data();
        void DeleteAllRecord(long Id);
    }
}
