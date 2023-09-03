using SSTM.Business.Interfaces;
using SSTM.Core.Centralized_Course;
using SSTM.Data.Infrastructure;
using SSTM.Models.Centralized_Course;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{

    public class CentralizedMasterService : RepositoryBase<Centralized_Master>, ICentralizedMasterService
    {
        public CentralizedMasterService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public Centralized_Master GetRecordById(int Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }
        public List<Centralized_Master> GetAllRecord()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.id).ToList();
        }
        public IEnumerable<Centralized_List_MasterModel> GetAllStatusRecord(long?  status_id)
        {
            return DataContext.Database.SqlQuery<Centralized_List_MasterModel>("EXEC sstmo.Central_Course_List @p0", status_id);
            
        }

        public IEnumerable<Centralized_List_MasterModel> GetAllTrainerStatusRecord(long? status_id)
        {
            return DataContext.Database.SqlQuery<Centralized_List_MasterModel>("EXEC sstmo.Central_triner_Course_List @p0", status_id);

        }

        public IEnumerable<Centralized_List_MasterModel> GetAllSMERecord(long? smeid)
        {
            return DataContext.Database.SqlQuery<Centralized_List_MasterModel>("EXEC sstmo.Central_SME_Course_List @p0", smeid);

        }
        public Centralized_Master GetRecordNewcourseById(int Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }

        public long SaveRecord(Centralized_Master entity)
        {
            if (entity.id > 0)
                Update(entity);
            else
                Add(entity);

            return entity.id;
        }

        public void DeleteRecord(int Id)
        {
            var entity = Table.Where(a => a.id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }
    }
}
