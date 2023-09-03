using SSTM.Business.Interfaces;
using SSTM.Core.CourseAssignment;
using SSTM.Core.CourseTrackers;
using SSTM.Core.User;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models;
using SSTM.Models.ActivityLog;
using SSTM.Models.AWS;
using SSTM.Models.Course;
using SSTM.Models.CourseAssignment;
using SSTM.Models.CourseDocRemarks;
using SSTM.Models.CourseDocument;
using SSTM.Models.CourseDownload;
using SSTM.Models.CourseSharing;
using SSTM.Models.CourseTrackers;
using SSTM.Models.MainCourseModel;
using SSTM.Models.SubCourse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebGrease.Css.Extensions;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class ReportController : BaseController
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IActivityLogService _IActivityLogService;
        private readonly IReportService _IReportService;        

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public ReportController
            (IExceptionLogService exceptionLogService, IActivityLogService activityLogService, IReportService reportService)
        {
           
            _IExceptionLogService = exceptionLogService;
            _IActivityLogService = activityLogService;
            _IReportService = reportService;
           
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }       

    }
}
