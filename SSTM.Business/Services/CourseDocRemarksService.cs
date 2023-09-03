using SSTM.Business.Interfaces;
using SSTM.Core.CourseDocRemarks;
using SSTM.Data.Infrastructure;
using SSTM.Models.CourseDocRemarks;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class CourseDocRemarksService : RepositoryBase<CourseDocRemarks>, ICourseDocRemarksService
    {
        public CourseDocRemarksService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<CourseDocRemarksGridModel> GetListByCourseId(long courseId)
        {
            return DataContext.SqlQuery<CourseDocRemarksGridModel>("EXEC sstmo.GetListOfCourseDocRemarks @p0", courseId);
        }

        public CourseDocRemarks GetRecordById(long Id)
        {
            return Table.Where(a => a.Id == Id).FirstOrDefault();
        }
        public List<CourseDocRemarks> GetCourseremarkByList(long courseid)
        {
            return Table.Where(a => a.CourseId == courseid).ToList();
        }

        public CourseDocRemarks GetRecordByDocId(long docId)
        {
            return Table.Where(a => a.DocId == docId).FirstOrDefault();
        }

        public void SaveRecord(CourseDocRemarks entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }

        public void DeleteRecord(long docId)
        {
            var entity = Table.Where(a => a.DocId == docId).FirstOrDefault();

            if (entity != null)
                Delete(entity);
        }
    }
}