using SSTM.Business.Interfaces;
using SSTM.Core.Centralized_Course;
using SSTM.Data.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{

    public class CentralizedDocumentFilesService : RepositoryBase<Centralized_Document_files>, ICentralizedDocumentFilesService
    {
        public CentralizedDocumentFilesService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public Centralized_Document_files GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }
        public List<Centralized_Document_files> GetAllRecord()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.id).ToList();
        }

        public List<Centralized_Document_files> GetAllbyCenterRecord(long Central_Master_Id)
        {
            return Table.AsNoTracking().Where(a => a.Central_Master_Id== Central_Master_Id).ToList();
        }
        public Centralized_Document_files GetRecordNewcourseById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }
        public Centralized_Document_files GetRecordNewcourseByTypewithmasterid(long master_id,string type)
        {
            return Table.AsNoTracking().Where(a => a.Central_Master_Id == master_id && a.Document_Type_Name==type).FirstOrDefault();
        }

        public long SaveRecord(Centralized_Document_files entity)
        {
            if (entity.id > 0)
                Update(entity);
            else
                Add(entity);

            return entity.id;
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }

    }
}
