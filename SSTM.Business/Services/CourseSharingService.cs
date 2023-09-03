using SSTM.Business.Interfaces;
using SSTM.Core.CourseSharing;
using SSTM.Data.Infrastructure;
using SSTM.Models.CourseSharing;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class CourseSharingService : RepositoryBase<CourseSharing>, ICourseSharingService
    {
        public CourseSharingService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public CourseSharing GetRecordByCourseAndDocIds(long courseId, long docId)
        {
            return Table.Where(a => a.CourseId == courseId && a.DocId == docId).FirstOrDefault();
        }

        public IEnumerable<SharedCourseListModel> GetListOfSharedCourses(bool? MasterCourse,long? MasterCourseId)
        {
            //return DataContext.Database.SqlQuery<SharedCourseListModel>("EXEC sstmo.GetListOfSharedCourses", new object[0]);
            return DataContext.Database.SqlQuery<SharedCourseListModel>("EXEC sstmo.GetListOfSharedCourses @p0,@p1", MasterCourse, MasterCourseId);
        }
        public IEnumerable<SharedCourseListModel> Get_Today_Course_Doc(bool? MasterCourse, long? MasterCourseId)
        {
            //return DataContext.Database.SqlQuery<SharedCourseListModel>("EXEC sstmo.GetListOfSharedCourses", new object[0]);
            return DataContext.Database.SqlQuery<SharedCourseListModel>("EXEC sstmo.Today_Class_Doc @p0,@p1", MasterCourse, MasterCourseId);
        }

        public IEnumerable<SharedCourseListModel> Today_central_Class_Doc()
        {  
            return DataContext.Database.SqlQuery<SharedCourseListModel>("EXEC sstmo.Today_central_Class_Doc");
        }

        public IEnumerable<SharedCourseListModel> GetListofSharedCourseDocs(long courseId)
        {
            return DataContext.SqlQuery<SharedCourseListModel>("EXEC sstmo.GetListofSharedCourseDocs @p0", courseId);
        }
        public IEnumerable<SharedCourseListModel> GetListofTodayCourseDocs(long courseId ,int isCentral)
        {
            return DataContext.SqlQuery<SharedCourseListModel>("EXEC sstmo.GetListof_Today_doc_CourseDocs @p0,@p1", courseId, isCentral);
        }

        public IEnumerable<SharedCourseListModel> GetCentralListofTodayCourseDocs(long courseId)
        {
            return DataContext.SqlQuery<SharedCourseListModel>("EXEC sstmo.GetCentralListof_Today_doc_CourseDocs @p0", courseId);
        }

        public IEnumerable<SharedCourseListModel> GetListofRASWPDocs(long airlineCourseId)
        {
            return DataContext.SqlQuery<SharedCourseListModel>("EXEC sstmo.Get_RA_SWP_Docs_Only_By_AirLine_CourseId @p0", airlineCourseId);
        }

        public void SaveRecord(CourseSharing entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }

        public void DeleteRecordByDocId(long docId)
        {
            var entity = Table.Where(a => a.DocId == docId).FirstOrDefault();

            if (entity != null)
                Delete(entity);
        }
    }
}