using SSTM.Core.Course_Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICourseRenewalService
    {
        long SaveRecord(CourseRenewal entity);
        void DeleteRecord(long Id);
        CourseRenewal GetRecordById(long Id);
        IEnumerable<CourseRenewal> GetAllRecord();
    }
}
