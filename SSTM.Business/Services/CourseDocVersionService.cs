using SSTM.Business.Interfaces;
using SSTM.Core.CourseDocVersion;
using SSTM.Data.Infrastructure;
using SSTM.Models.CourseDocVersion;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class CourseDocVersionService : RepositoryBase<CourseDocVersion>, ICourseDocVersionService
    {
        public CourseDocVersionService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public CourseDocVersion GetRecordById(long Id)
        {
            return Table.Where(a => a.Id == Id).FirstOrDefault();
        }

        public CourseDocVersion GetLatestRecordByDocId(long docId)
        {
            return Table.Where(a => !a.isDeleted && a.isActive && a.DocId == docId).OrderByDescending(a => a.Id).FirstOrDefault();
        }

        public IEnumerable<CourseDocVersionsListModel> GetListByDocId(long docId)
        {
            return DataContext.Database.SqlQuery<CourseDocVersionsListModel>("EXEC sstmo.GetListOfCourseDocVersions @p0", docId);
        }

        public IEnumerable<CourseDocVersionsListModel> GetRecentDocumentList(long days)
        {
            return DataContext.Database.SqlQuery<CourseDocVersionsListModel>("EXEC sstmo.GetRecentDocumentList @p0", days);
        }


        public void SaveRecord(CourseDocVersion entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id).FirstOrDefault();

            if (entity != null)
                Delete(entity);
        }

        public void DeleteRecordsByDocId(long docId)
        {
            var entityList = Table.Where(a => a.DocId == docId).ToList();

            if (entityList.Count > 0)
            {
                foreach (var entity in entityList)
                    Delete(entity);
            }
        }

        public void UpdateDocVersionStatus(long docVersionId, bool isActive, long userId)
        {
            DataContext.Database.ExecuteSqlCommand("EXEC sstmo.UpdateDocVersionStatus @p0, @p1, @p2", docVersionId, isActive, userId);
        }
    }
}