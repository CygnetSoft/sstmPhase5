using SSTM.Core.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICentralizedCourseService
    {
        Centralized_Course GetRecordById(long Id);
        List<Centralized_Course> GetAllRecord();
        Centralized_Course GetRecordNewcourseById(long Id);
        long SaveRecord(Centralized_Course entity);
        void DeleteRecord(long Id);
        List<Centralized_Course> GetAllbyCenterRecord(long Central_Master_Id);
        void DeleteAllRecord(long Id);
        List<Centralized_Course> GetCourseReplacedataWithType(string type, int center_master_id);
    }
}
