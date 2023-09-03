using SSTM.Business.Interfaces;
using SSTM.Core.Course_Reminder;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class CourseRenewalService : RepositoryBase<CourseRenewal>, ICourseRenewalService
    {
        public CourseRenewalService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        public IEnumerable<CourseRenewal> GetAllRecord()
        {
            return Table.OrderBy(a => a.Id).ToList();
        }
        public CourseRenewal GetRecordById(long Id)
        {
            return Table.Where(a => a.Id == Id).FirstOrDefault();
        }
        public long SaveRecord(CourseRenewal entity)
        {
            if (entity.Id > 0)
                Update(entity);
            else
                Add(entity);

            return entity.Id;
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }
      
    }
}
