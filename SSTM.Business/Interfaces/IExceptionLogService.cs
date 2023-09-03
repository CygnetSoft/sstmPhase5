using System;
using System.Web;

namespace SSTM.Business.Interfaces
{
    public interface IExceptionLogService
    {
        void SaveRecord(Exception ex, string controller, string action, string url, long userId);
    }
}