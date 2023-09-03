using SSTM.Business.Interfaces;
using SSTM.Core.CourseAssignment;
using SSTM.Data.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class CourseAssignmentService : RepositoryBase<CourseAssignment>, ICourseAssignmentService
    {
        public CourseAssignmentService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public CourseAssignment GetRecordByCourseId(long courseId)
        {
            return Table.Where(a => a.CourseId == courseId).FirstOrDefault();
        }
        public List<CourseAssignment> GetRecordByCourseId_list()
        {
            return Table.Where(a => a.SMEId !=0).ToList();
        }

        public void SaveRecord(CourseAssignment entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }

        public void DeleteRecordByCourseId(long courseId)
        {
            var entity = Table.Where(a => a.CourseId == courseId).FirstOrDefault();

            if (entity != null)
                Delete(entity);
        }
    }
}