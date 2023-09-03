using SSTM.Business.Interfaces;
using SSTM.Core.Centralized_Course;
using SSTM.Data.Infrastructure;
using SSTM.Models.Centralized_Course;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{

    public class CentralizedHistoryService : RepositoryBase<Centralized_History>, ICentralizedHistoryService
    {
        public CentralizedHistoryService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public List<Centralized_HistoryWithDataModel> GetHistory_data()
        {
            return DataContext.Database.SqlQuery<Centralized_HistoryWithDataModel>("EXEC sstmo.Get_centralize_history_data").ToList();
        }

        public Centralized_History GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }
        public List<Centralized_History> GetAllRecord()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.id).ToList();
        }
        public List<Centralized_History> GetAllbyCenterRecord(long Central_Master_Id)
        {
            return Table.AsNoTracking().OrderByDescending(a => a.center_master_id== Central_Master_Id).ToList();
        }
        public Centralized_History GetRecordNewcourseById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }

        public long SaveRecord(Centralized_History entity)
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
        public void DeleteAllRecord(long Id)
        {
            var entity = Table.Where(a => a.center_master_id == Id).ToList();
            foreach (var item in entity)
            {
                var data = Table.Where(a => a.id == item.id).FirstOrDefault();
                if (data != null)
                    Delete(data);
            }

        }
    }
}
