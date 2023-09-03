using SSTM.Core.ActivityLog;
using SSTM.Models.ActivityLog;
using System;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface IActivityLogService
    {
        IEnumerable<ActivityLogListModel> GetListByDates(DateTime fromDate, DateTime toDate);

        void SaveRecord(ActivityLog entity);

        void DeleteRecordsByDates(DateTime fromDate, DateTime toDate);
    }
}