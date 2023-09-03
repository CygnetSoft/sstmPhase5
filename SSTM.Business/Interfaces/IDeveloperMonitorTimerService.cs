using SSTM.Core.Developer_Monitor_Timer;
using SSTM.Models.DeveloperMonitorTimer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface IDeveloperMonitorTimerService
    {
        DeveloperMonitorTimer GetRecordById(long Id);
        List<DeveloperMonitorTimer> GetAllRecord();
        long SaveRecord(DeveloperMonitorTimer entity);
        void DeleteRecord(long Id);
        IEnumerable<MonitorListModel> GetDetailsList(string start_date, string end_date, long user_id);
        IEnumerable<MonitorListModel> GetDetails_all_List(string start_date, string end_date, long user_id);
        DeveloperMonitorTimer GetUserLastRecord(Int64 userid);
    }
}
