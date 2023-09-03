using SSTM.Business.Interfaces;
using SSTM.Core.Course_Reminder;
using SSTM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class DashboardController : Controller
    {
        private readonly ICourseService _ICourseService;
        private readonly ICourseReminderService _ICourseReminderService;
        public DashboardController(ICourseService courseService, ICourseReminderService CourseReminderService)
        {
            _ICourseService = courseService;
            _ICourseReminderService = CourseReminderService;
        }
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
     
        public ActionResult Get_Course_Dashboard_data()
        {
            var courseCount = _ICourseService.Get_Dashboard_data().ToList();
            return Json(courseCount, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Get_New_Course_data(string status)
        {
            List<CourseReminder> NewCourseList = new List<CourseReminder>();
            if(status=="pending")
                NewCourseList =_ICourseReminderService.GetPendingCourseList().ToList();
            else
                NewCourseList = _ICourseReminderService.GetRenewalCourseList().ToList();

            return Json(NewCourseList, JsonRequestBehavior.AllowGet);
        }
    }
}