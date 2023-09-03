using SSTM.Business.Interfaces;
using SSTM.Core.ExceptionLog;
using SSTM.Data.Infrastructure;
using SSTM.Helpers.Common;
using System;
using System.Web;

namespace SSTM.Business.Services
{
    public class ExceptionLogService : RepositoryBase<ExceptionLog>, IExceptionLogService
    {
        public ExceptionLogService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public void SaveRecord(Exception ex, string controller, string action, string url, long userId)
        {
            Add(new ExceptionLog
            {
                Exception = ex.ToString(),
                Message = ex.Message,
                InnerException = ex.InnerException != null ? ex.InnerException.Message : null,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                UserId = userId,
                Controller = controller,
                ActionName = action,
                URL = url,
                IPAddress = UtilityHelper.GetIPAddress(),
                CreatedOn = DateTime.Now
            });
        }
    }
}