using SSTM.Core.Centralized_Course;
using SSTM.Models.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICentralizedMasterService
    {
        Centralized_Master GetRecordById(int Id);
        List<Centralized_Master> GetAllRecord();
        Centralized_Master GetRecordNewcourseById(int Id);
        long SaveRecord(Centralized_Master entity);
        void DeleteRecord(int Id);
        IEnumerable<Centralized_List_MasterModel> GetAllStatusRecord(long? status_id);

        IEnumerable<Centralized_List_MasterModel> GetAllSMERecord(long? smeid);
        IEnumerable<Centralized_List_MasterModel> GetAllTrainerStatusRecord(long? status_id);
    }
}
