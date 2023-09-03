using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class CourseDownloadController : Controller
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IUserService _IUserService;
        private readonly ICourseDownloadUserService _ICourseDownloadUserService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion
        #region Class Properties Definitions
        public CourseDownloadController(IExceptionLogService exceptionLogService ,IUserService userService, 
            ICourseDownloadUserService CourseDownloadUserService)
        {
            _IExceptionLogService = exceptionLogService;
            _IUserService = userService;
            _ICourseDownloadUserService = CourseDownloadUserService;
        }
        #endregion

        // GET: CourseDownload
        public ActionResult Index(bool? MasterCourse, long? MasterCourseId, string Coursename)
        {
            if (MasterCourse == null)
                MasterCourse = true;
            if (MasterCourseId == null)
                MasterCourseId = 0;

            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.Coursename = Coursename;

            return View(CurrentSession);
        }
        #region Course List

        [HttpGet]
        public ActionResult GetDownloadCoursesList(bool MasterCourse, long MasterCourseId)
        {
            try
            {
                var sBuilder = new StringBuilder();
                
                var list = _ICourseDownloadUserService.GetDownloadList(CurrentSession.UserId, MasterCourse, MasterCourseId).ToList();
                foreach (var item in list)
                {
                    sBuilder.Append(
                        "<tr id='" + item.CourseId + "'>" +
                            "<td>" + item.CourseName + "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Sub Course' class='btn btn-warning btn-sm btnSubCourse'>" +
                                    "Sub Course </button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Documents' class='btn btn-info btn-sm btnCourseDocs'>" +
                                    "<i class='fa fa-files-o'></i></button>" +
                            "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDownload", "GetDownloadCoursesList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}