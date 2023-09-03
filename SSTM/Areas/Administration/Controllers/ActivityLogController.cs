using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.Common;
using System;
using System.Text;
using System.Web.Mvc;

namespace SSTM.Areas.Administration.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class ActivityLogController : Controller
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IActivityLogService _IActivityLogService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public ActivityLogController(IExceptionLogService exceptionLogService, IActivityLogService activityLogService)
        {
            _IExceptionLogService = exceptionLogService;
            _IActivityLogService = activityLogService;
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetActivityLogsList(string dateFrom, string dateTo)
        {
            try
            {
                if (dateFrom != "" && dateTo != "")
                {
                    DateTime datefrom = UtilityHelper.ConvertToSGTDate(dateFrom);
                    DateTime dateto = UtilityHelper.ConvertToSGTDate(dateTo);

                    var list = _IActivityLogService.GetListByDates(datefrom, dateto);

                    var sBuilder = new StringBuilder();

                    foreach (var item in list)
                    {
                        sBuilder.Append(
                            "<tr>" +
                                "<td>" + item.Date + "</td>" +
                                "<td>" + item.User + "</td>" +
                                "<td>" + item.Duration + "</td>" +
                            "</tr>");
                    }

                    return Json(new { result = true, content = sBuilder.ToString() });
                }
                else
                    return Json(new { result = false, message = "Please select valid date range to continue." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "ActivityLog", "GetActivityLogsList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteSelectedRecords(string dateFrom, string dateTo)
        {
            try
            {
                if (dateFrom != "" && dateTo != "")
                {
                    DateTime datefrom = UtilityHelper.ConvertToSGTDate(dateFrom);
                    DateTime dateto = UtilityHelper.ConvertToSGTDate(dateTo);

                    _IActivityLogService.DeleteRecordsByDates(datefrom, dateto);

                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "Please select valid date range to continue." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "ActivityLog", "GetActivityLogsList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}