using SSTM.Business.Interfaces;
using SSTM.Core.ActivityLog;
using SSTM.Data.Infrastructure;
using SSTM.Models.ActivityLog;
using System;
using System.Collections.Generic;

namespace SSTM.Business.Services
{
    public class ActivityLogService : RepositoryBase<ActivityLog>, IActivityLogService
    {
        public ActivityLogService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<ActivityLogListModel> GetListByDates(DateTime fromDate, DateTime toDate)
        {
            return DataContext.SqlQuery<ActivityLogListModel>("EXEC sstmo.GetActivityLogsByDates @p0, @p1", fromDate, toDate);
        }

        public void SaveRecord(ActivityLog entity)
        {
            Add(entity);
        }

        public void DeleteRecordsByDates(DateTime fromDate, DateTime toDate)
        {
            DataContext.SqlQuery<ActivityLogListModel>("EXEC sstmo.DeleteActivityLogsByDates @p0, @p1", fromDate, toDate);
        }
    }
}