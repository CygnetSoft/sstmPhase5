using SSTM.Core.CourseSharing;
using SSTM.Models.CourseSharing;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface ICourseSharingService
    {
        CourseSharing GetRecordByCourseAndDocIds(long courseId, long docId);

        //IEnumerable<SharedCourseListModel> GetListOfSharedCourses();

        IEnumerable<SharedCourseListModel> GetListOfSharedCourses(bool? MasterCourse, long? MasterCourseId);
        IEnumerable<SharedCourseListModel> GetListofSharedCourseDocs(long courseId);

        void SaveRecord(CourseSharing entity);

        void DeleteRecordByDocId(long docId);
        IEnumerable<SharedCourseListModel> Get_Today_Course_Doc(bool? MasterCourse, long? MasterCourseId);
       // IEnumerable<SharedCourseListModel> GetListofTodayCourseDocs(long courseId);
        IEnumerable<SharedCourseListModel> GetListofRASWPDocs(long airlineCourseId);

        IEnumerable<SharedCourseListModel> Today_central_Class_Doc();
        IEnumerable<SharedCourseListModel> GetCentralListofTodayCourseDocs(long courseId);

        IEnumerable<SharedCourseListModel> GetListofTodayCourseDocs(long courseId, int isCentral);
    }
}