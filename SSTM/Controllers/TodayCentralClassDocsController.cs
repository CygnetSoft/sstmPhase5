using SSTM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SSTM.CourseService;
using SSTM.sg.com.eversafe.li;
using SSTM.Helpers.App;
using SSTM.Business.Interfaces;
using SSTM.Models.CourseSharing;
using SSTM.Filters;
using SSTM.Helpers.Common;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class TodayCentralClassDocsController : BaseController
    {
        DateFormat dtformat;

        private readonly ICourseService _ICourseService;
        private readonly ICourseSharingService _ICourseSharingService;
        private readonly IExceptionLogService _IExceptionLogService;

        public TodayCentralClassDocsController(ICourseService courseService, ICourseSharingService courseSharingService,
            IExceptionLogService exceptionLogService)
        {
            _ICourseService = courseService;
            _ICourseSharingService = courseSharingService;
            _IExceptionLogService = exceptionLogService;

            dtformat = new DateFormat();
        }
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }

        // GET: TodayCentralClassDocs
        public ActionResult Index()
        {
            var AppendList = new List<TodayClassDocsWithSSTMModel>();
            try
            {
                var list = new List<SharedCourseListModel>();

                if (CurrentSession.Trainer_AirLine_id != 0 || CurrentSession.Trainer_AirLine_id.ToString() != "")
                {
                    CourseService.SSTM service = new CourseService.SSTM();
                    List<TodayClassDocsModel> TodayClass_data = new List<TodayClassDocsModel>();
                    string dt = dtformat.GetSingaporeTime().ToString("yyyy-MM-dd");
                    string data = service.TodayCoursesWithSectionTainerid(dt, Convert.ToInt32(CurrentSession.Trainer_AirLine_id));
                    List<TodayClassDocsModel> model = (new JavaScriptSerializer()).Deserialize<List<TodayClassDocsModel>>(data);
                    list = _ICourseSharingService.Today_central_Class_Doc().ToList();
                    for (int i = 0; i < model.Count(); i++)
                    {
                        for (int j = 0; j < list.Count(); j++)
                        {
                            if (list[j].AirLineCourseId == model[i].Courseid)
                            {
                                TodayClassDocsWithSSTMModel classModal = new TodayClassDocsWithSSTMModel();
                                classModal.coursename = model[i].coursename;
                                classModal.sstmCourseName = list[j].CourseName;
                                classModal.CourseId = list[j].CourseId;
                                classModal.section1SectionName = model[i].section1SectionName;
                                classModal.trainerID = model[i].trainerID;
                                classModal.batchid = model[i].batchid;
                                classModal.fin = model[i].fin;
                                AppendList.Add(classModal);
                            }
                          
                        }
                    }
                    ViewBag.Error = "";
                }
                else
                {
                    ViewBag.Error = "Note: your Li id not register in system so, please contact to administrator and add Airline system so, you can show today's Document.";

                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Not Get data from Li system data Please try again and refresh ....!";
            }
            return View(AppendList);
        }

        [HttpPost]
        public ActionResult ViewTodayCourseDocs(long courseId, string batchId)
        {
            try
            {
                var list = new List<SharedCourseListModel>();
                list = _ICourseSharingService.GetCentralListofTodayCourseDocs(courseId).Where(a => a.isTraining).ToList();
                ViewBag.batchid = batchId;
                return PartialView("_ViewCentralCourseDocsTodayCourseModel", list);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TodayClassDocsController", "_ViewCourseDocsTodayCourseModel.cshtml", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }
    }
}