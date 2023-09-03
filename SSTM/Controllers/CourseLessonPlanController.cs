using SSTM.Business.Interfaces;
using SSTM.Helpers.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    public class CourseLessonPlanController : Controller
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IActivityLogService _IActivityLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        private readonly IRoleService _IRoleService;

        private readonly ICourseStatusService _ICourseStatusService;
        private readonly ICourseService _ICourseService;
        private readonly ICourseAssignmentService _ICourseAssignmentService;

        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ICourseDocVersionService _ICourseDocVersionService;
        private readonly ICourseDocRemarksService _ICourseDocRemarksService;
        private readonly ICourseSharingService _ICourseSharingService;
        private readonly IMainCourseService _IMainCourseService;
        private readonly ISubCourseService _ISubCourseService;
        private readonly ICourseTrackersService _ICourseTrackersService;
        private readonly ICourseDownloadUserService _ICourseDownloadUserService;
        private readonly IAssessmentPaperService _IAssessmentPaperService;
        private readonly IDeveloperMonitorTimerService _IDeveloperMonitorTimerService;
        private static string documentPath = "";
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public CourseLessonPlanController
            (IExceptionLogService exceptionLogService, IActivityLogService activityLogService, IConfigService configService,
            IUserService userService, IRoleService roleService, ICourseStatusService courseStatusService, ICourseService courseService,
            ICourseAssignmentService courseAssignmentService, ICourseDocumentService courseDocumentService,
            ICourseDocVersionService courseDocVersionService, ICourseDocRemarksService courseDocRemarksService,
            ICourseSharingService courseSharingService, IMainCourseService MainCourseService,
            ISubCourseService SubCourseService, ICourseTrackersService CourseTrackersService,
             ICourseDownloadUserService CourseDownloadUserService, IAssessmentPaperService IAssessmentPaperService,
             IDeveloperMonitorTimerService DeveloperMonitorTimerService)
        {

            _IDeveloperMonitorTimerService = DeveloperMonitorTimerService;
            _IExceptionLogService = exceptionLogService;
            _IActivityLogService = activityLogService;
            _IConfigService = configService;
            _IUserService = userService;
            _IRoleService = roleService;

            _ICourseStatusService = courseStatusService;
            _ICourseService = courseService;
            _ICourseAssignmentService = courseAssignmentService;

            _ICourseDocumentService = courseDocumentService;
            _ICourseDocVersionService = courseDocVersionService;
            _ICourseDocRemarksService = courseDocRemarksService;
            _ICourseSharingService = courseSharingService;
            _IMainCourseService = MainCourseService;
            _ISubCourseService = SubCourseService;
            _ICourseTrackersService = CourseTrackersService;
            _ICourseDownloadUserService = CourseDownloadUserService;
            _IAssessmentPaperService = IAssessmentPaperService;
        }
        #endregion
        // GET: AssessmentPaper
        // GET: CourseLessonPlan
        public ActionResult Index()
        {
            return View();
        }
    }
}