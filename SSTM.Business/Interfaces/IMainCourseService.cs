using SSTM.Core.MainCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface IMainCourseService
    {
        IEnumerable<MainCourse> GetMainCourseList(int isActive);
        IEnumerable<MainCourse> GetMainStaffCourseList(int isActive);
        MainCourse GetRecordById(long Id);
        long SaveRecord(MainCourse entity);
        void DeleteRecord(long Id);

        bool isCourseNameExists(long Id, string courseName, string CourseType);
        
    }
}
