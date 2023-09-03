using SSTM.Business.Interfaces;
using SSTM.Core.CourseStatus;
using SSTM.Data.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class CourseStatusService : RepositoryBase<CourseStatus>, ICourseStatusService
    {
        public CourseStatusService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public CourseStatus GetRecordById(string Id)
        {
            return Table.Where(a => !a.isDeleted).FirstOrDefault();
        }

        public long GetRecordIdByName(string status)
        {
            return Table.Where(a => !a.isDeleted && a.Status == status).FirstOrDefault().Id;
        }

        public IEnumerable<CourseStatus> GetList()
        {
            return Table.Where(a => !a.isDeleted);
        }
    }
}