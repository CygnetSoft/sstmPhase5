using SSTM.Core.Course;
using SSTM.Models.Course;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface ICourseService
    {

        IEnumerable<CourseListModel> GetList(int isActive, long statusId, bool MasterCourse, long MainCourseId);
        IEnumerable<CourseListModel> GetStaffSubCoursesList(int isActive, long statusId, long CurrentuserLogin, bool MasterCourse, long MainCourseId);
        IEnumerable<CourseListModel> GetStaffList(int isActive, long statusId, long CurrentuserLogin);
        IEnumerable<CourseListModel> GetCoursesWithStatus(int isActive, long statusId);
        IEnumerable<CourseListModel> GetCoursesWithotStatus(int isActive, long statusId);
        IEnumerable<CourseListModel> GetComonCoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId);
        IEnumerable<CourseListModel> GetComonCoursesList_All(int isActive, bool MasterCourse, long MainCourseId);
        IEnumerable<CourseListModel> GetList_All(int isActive, bool MasterCourse, long MainCourseId);
        Course GetRecordById(long Id);

        long SaveRecord(Course entity);

        void DeleteRecord(long Id);

        bool isCourseNameExists(long Id, string courseName, string CourseType);
        //IEnumerable<Course> GetMainCourseToCourseList(long MainCourseId);
        IEnumerable<Course> GetMainCourseToCourseList(string CourseType, long MainCourseId);
        //long SubFolderCount(long? MasterCoursId, string CourseType);

        IEnumerable<CourseListModel> Get_CourseAndSubCourse(string CourseType, bool MasterCourse, long MainCourseId, long selectdSubid);
        IEnumerable<DashbordCourseCountModel> Get_Dashboard_data();
        IEnumerable<CourseListModel> Get_Iso_Edu_CoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId);
        IEnumerable<CourseListModel> Get_Iso_Edu_ComonCoursesList_All(int isActive, bool MasterCourse, long MainCourseId);
        IEnumerable<CourseListModel> Get_Iso_Edu_ComonCoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId);

        IEnumerable<CourseListModel> GetComonNewCoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId, long NewCourseId);
        IEnumerable<CourseListModel> GetNewList(int isActive, long statusId, bool MasterCourse, long MainCourseId, long NewCourseId);
        Course GetRecordNewcourseById(long Id);
        List<Course> GetAllRecord();
    }
}