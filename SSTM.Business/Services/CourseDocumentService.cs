using SSTM.Business.Interfaces;
using SSTM.Core.CourseDocument;
using SSTM.Data.Infrastructure;
using SSTM.Models.CourseDocument;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{
    public class CourseDocumentService : RepositoryBase<CourseDocument>, ICourseDocumentService
    {
        public CourseDocumentService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<CourseDocumentsListModel> GetListByCourseId(long courseId,bool MasterCourse)
        {
            return DataContext.SqlQuery<CourseDocumentsListModel>("EXEC sstmo.GetListOfCourseDocuments @p0,@p1", courseId, MasterCourse);
        }
        public IEnumerable<CourseDocumentsListModel> GetListByOldCourse()
        {
            return DataContext.SqlQuery<CourseDocumentsListModel>("EXEC  sstmo.GetListOfOldCourseDocuments");
        }
        public IEnumerable<CourseDocumentsListModel> GetListVersionByCourseId(long courseId)
        {
            return DataContext.SqlQuery<CourseDocumentsListModel>("EXEC sstmo.GetListOfCourseDocumentsversion @p0", courseId);
        }

        public CourseDocument GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }

        public IEnumerable<CourseDocument> UpdateDocumentStatusByUserRecordById(long Userid)
        {
            return Table.AsNoTracking().Where(a => a.UserId == Userid);
        }

        public long SaveRecord(CourseDocument entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);

            return entity.Id;
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id).FirstOrDefault();

            if (entity != null)
                Delete(entity);
        }

        public bool isExistsDocNameForCourse(string docName, long courseId, long docId)
        {
            if (Table.Where(a => a.CourseId == courseId && a.Id != docId && a.DocName == docName).Count() > 0)
                return true;
            else
                return false;
        }
    }
}