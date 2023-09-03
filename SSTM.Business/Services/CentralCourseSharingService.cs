using SSTM.Business.Interfaces;
using SSTM.Core.Central_CourseSharing;
using SSTM.Data.Infrastructure;
using SSTM.Models.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class CentralCourseSharingService : RepositoryBase<Central_CourseSharing>, ICentralCourseSharingService
    {
        public CentralCourseSharingService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public Central_CourseSharing GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }
        public List<Central_CourseSharing> GetAllRecord()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.Id).ToList();
        }

        public List<Central_CourseSharing> GetAssignRecord(int CourseId)
        {
            return Table.AsNoTracking().Where(a => a.CourseId== CourseId).ToList();
        }
        public Central_CourseSharing GetRecordByCentraAndDocIds(int master_id,int docid)
        {
            return Table.AsNoTracking().Where(a => a.CourseId == master_id && a.DocId== docid).FirstOrDefault();
        }
        
        public long SaveRecord(Central_CourseSharing entity)
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

        public IEnumerable<CentralDocumentsListModel> GetListofSharedCentralCourseDocs(int Central_master_Id)
        {
            return DataContext.Database.SqlQuery<CentralDocumentsListModel>("EXEC sstmo.GetListOfCentralDocuments @p0", @Central_master_Id);
        }

    }
}
