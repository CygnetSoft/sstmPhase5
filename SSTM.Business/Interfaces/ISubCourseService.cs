using SSTM.Core.SubCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ISubCourseService
    {
        IEnumerable<SubCourse> GetSubCourseList(int isActive,long MainCourseId);
        IEnumerable<SubCourse> GetSubStaffCourseList(int isActive, long MainCourseId);
        SubCourse GetRecordById(long Id);
        IEnumerable<SubCourse> GetMainCourseList(string CourseType);
        long SaveRecord(SubCourse entity);
        void DeleteRecord(long Id);
        bool isCourseNameExists(long Id, string SubcourseName, string CourseType);
        IEnumerable<SubCourse> GetSubCourseList(string CourseType, long MainCourseId);
    }
}
