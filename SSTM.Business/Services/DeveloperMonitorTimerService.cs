using SSTM.Business.Interfaces;
using SSTM.Core.Developer_Monitor_Timer;
using SSTM.Data.Infrastructure;
using SSTM.Models.DeveloperMonitorTimer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class DeveloperMonitorTimerService : RepositoryBase<DeveloperMonitorTimer>, IDeveloperMonitorTimerService
    {
        public DeveloperMonitorTimerService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<MonitorListModel> GetDetailsList(string start_date, string end_date, long user_id)
        {
            return DataContext.Database.SqlQuery<MonitorListModel>("EXEC sstmo.DeveloperLogin_Monitor @p0, @p1,@p2",start_date, end_date, user_id);
        }
        public IEnumerable<MonitorListModel> GetDetails_all_List(string start_date, string end_date, long user_id)
        {
            return DataContext.Database.SqlQuery<MonitorListModel>("EXEC sstmo.DeveloperLogin_Monitor_AllData @p0, @p1,@p2", start_date, end_date, user_id);
        }

        public DeveloperMonitorTimer GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }
        public List<DeveloperMonitorTimer> GetAllRecord()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.id).ToList();
        }
        public DeveloperMonitorTimer GetUserLastRecord(Int64 userid)
        {
            return Table.AsNoTracking().Where(a=>a.user_id== userid).OrderByDescending(a => a.id).FirstOrDefault();
        }



        public long SaveRecord(DeveloperMonitorTimer entity)
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
