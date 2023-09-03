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
    public class TodayClassDocsController : BaseController
    {
        DateFormat dtformat;

        private readonly ICourseService _ICourseService;
        private readonly ICourseSharingService _ICourseSharingService;
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IUserService _IUserService;

        public TodayClassDocsController(ICourseService courseService, ICourseSharingService courseSharingService,
            IExceptionLogService exceptionLogService, IUserService userService)
        {
            _ICourseService = courseService;
            _ICourseSharingService = courseSharingService;
            _IExceptionLogService = exceptionLogService;
            _IUserService = userService;
            dtformat = new DateFormat();
        }
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        // GET: TodayClassDocs
        public ActionResult Index(bool? MasterCourse, long? MasterCourseId, string Coursename)
        {
            var AppendList = new List<TodayClassDocsWithSSTMModel>();
            try
            {
                var list = new List<SharedCourseListModel>();
                if (MasterCourse == null)
                    MasterCourse = true;
                if (MasterCourseId == null)
                    MasterCourseId = 0;

                ViewBag.MasterCourse = MasterCourse.ToString();
                ViewBag.MasterCourseId = MasterCourseId;
                ViewBag.Coursename = Coursename;

                if (CurrentSession.Trainer_AirLine_id != 0 || CurrentSession.Trainer_AirLine_id.ToString() != "")
                {
                    CourseService.SSTM service = new CourseService.SSTM();
                    List<TodayClassDocsModel> TodayClass_data = new List<TodayClassDocsModel>();
                    string dt = dtformat.GetSingaporeTime().ToString("yyyy-MM-dd");
                    string data = service.TodayCoursesWithSectionTainerid(dt, Convert.ToInt32(CurrentSession.Trainer_AirLine_id));
                    List<TodayClassDocsModel> model = (new JavaScriptSerializer()).Deserialize<List<TodayClassDocsModel>>(data);
                    list = _ICourseSharingService.Get_Today_Course_Doc(MasterCourse, MasterCourseId).ToList();
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
                                classModal.LiCourseid = model[i].Courseid;
                                classModal.fin = model[i].fin;
                                classModal.isCentral = list[j].isCentral;
                                AppendList.Add(classModal);
                            }
                            //else
                            //{
                            //    TodayClassDocsWithSSTMModel classModal = new TodayClassDocsWithSSTMModel();
                            //    classModal.coursename = list[j].CourseName;
                            //    classModal.sstmCourseName = list[j].CourseName;
                            //    classModal.CourseId = list[j].CourseId;
                            //    classModal.section1SectionName = "";
                            //    classModal.trainerID = model[i].trainerID;
                            //    classModal.batchid =0;
                            //    classModal.fin = "";
                            //    AppendList.Add(classModal);
                            //}
                        }
                    }
                    ViewBag.Error = "";
                }
                else
                {
                    ViewBag.Error = "Note: your Li id not register in system so, please contact to administrator and add Airline system so, you can show today's Document.";

                }

            }
            catch (Exception)
            {
                ViewBag.Error = "Not Get data from Li system data Please try again and refresh ....!";
            }
           var UserDetail = _IUserService.GetRecordById(CurrentSession.UserId);
           
            ViewBag.loginuserid = CurrentSession.UserId;
            // TodayClass_data = model.Where(a => a.trainername == CurrentSession.UserName).ToList();
            return View(AppendList);
        }

        [HttpPost]
        public ActionResult ViewTodayCourseDocs(long courseId, string batchId,int isCentral)
        {
            try
            {
                var list = new List<SharedCourseListModel>();
                list = _ICourseSharingService.GetListofTodayCourseDocs(courseId, isCentral).Where(a => a.isTraining).ToList();
                ViewBag.batchid = batchId;
                return PartialView("_ViewCourseDocsTodayCourseModel", list);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TodayClassDocsController", "_ViewCourseDocsTodayCourseModel.cshtml", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        public ActionResult GetLessonPlanDetails(int courseId,long batchId,string topic)
        {
            GetLessonPlanDetails_final GetLessonPlanDetails_model = new GetLessonPlanDetails_final();
            string topic_data = "";
            //courseId = 347;
            //batchId = 2409;
            try
            {
                CurrentSession.CurrentCourseId = courseId;
                CurrentSession.CurrentBatchId = batchId;

                UtilityHelper.CurrentCourseId = courseId;
                UtilityHelper.CurrentbatchId = batchId;

                CourseService.SSTM service = new CourseService.SSTM();
                //string dt = dtformat.GetSingaporeTime().ToString("dd-MM-yyyy h:mm:ss tt");
                string data = service.GetLessonPlanDetails(courseId, batchId);
                GetLessonPlanDetails_model = (new JavaScriptSerializer()).Deserialize<GetLessonPlanDetails_final>(data);
                ViewBag.Rem_Time = DateTime.Now.AddMinutes(2).ToString("dd-MM-yyyy h:mm:ss tt");
                int i = 1;
                if (GetLessonPlanDetails_model.data != null)
                {
                    foreach (var item in GetLessonPlanDetails_model.data)
                    {
                        //if (item.topic == "Instant Performance Reflection")
                        //{
                        //    item.starttime = "12:00";
                        //    item.endtime = "11:35";
                        //    item.startmeridian = "AM";
                        //    item.endmeridian = "PM";
                        //}
                        i++;
                        DateTime time = DateTime.Today;
                        //item.starttime = item.starttime;
                        //item.endtime = item.endtime;
                        #region Convert 12 hr to 24hrs                       
                        string starttime = Convert.ToDateTime(item.starttime+ " "+item.startmeridian).ToString("HH:mm");
                        string endtime = Convert.ToDateTime(item.endtime + " " + item.endmeridian).ToString("HH:mm");
                        #endregion

                       // DateTime currentdt = new DateTime(2023, 08, 30,10, 14, 20);
                        DateTime currentdt = dtformat.GetSingaporeTime();
                        DateTime hr1 = new DateTime(currentdt.Year, currentdt.Month, currentdt.Day, Convert.ToInt32(starttime.Split(':')[0]), Convert.ToInt32(starttime.Split(':')[1]), 0);
                        DateTime hr2 = new DateTime(currentdt.Year, currentdt.Month, currentdt.Day, Convert.ToInt32(endtime.Split(':')[0]), Convert.ToInt32(endtime.Split(':')[1]), 0);

                        
                        if (currentdt > hr1 && currentdt < hr2)
                        {
                            DateTime a = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(starttime.Split(':')[0]), Convert.ToInt32(starttime.Split(':')[1]), 00);
                            DateTime b = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(endtime.Split(':')[0]), Convert.ToInt32(endtime.Split(':')[1]), 00);
                           


                            DateTime c = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, currentdt.Hour, currentdt.Minute, 00);

                            string Final_remining_time = c.Subtract(a).Hours.ToString().PadLeft(2, '0') + ":" + c.Subtract(a).Minutes.ToString().PadLeft(2, '0') + ":" + c.Subtract(a).Seconds.ToString().PadLeft(2, '0');
                            DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(Final_remining_time.Split(':')[0]), Convert.ToInt32(Final_remining_time.Split(':')[1]), 00);

                            string remining_time = b.Subtract(a).Hours.ToString().PadLeft(2, '0') + ":" + b.Subtract(a).Minutes.ToString().PadLeft(2, '0') + ":" + b.Subtract(a).Seconds.ToString().PadLeft(2, '0');
                            DateTime dtremining = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(remining_time.Split(':')[0]), Convert.ToInt32(remining_time.Split(':')[1]), 00);

                            string ffFinal_remining_time = dtremining.Subtract(d).Hours.ToString().PadLeft(2, '0') + ":" + dtremining.Subtract(d).Minutes.ToString().PadLeft(2, '0') + ":" + d.Subtract(d).Seconds.ToString().PadLeft(2, '0');


                            ViewBag.Rem_Time = ffFinal_remining_time;

                            ViewBag.topic = item.topic;
                            ViewBag.start_time = starttime.Split(':')[0] + ":" + starttime.Split(':')[1]+ ":" + +DateTime.Now.Second;
                            ViewBag.current_time = currentdt.Hour + ":" + currentdt.Minute + ":" + currentdt.Second;
                            ViewBag.endtime = endtime.Split(':')[0] + ":" + endtime.Split(':')[1] + ":" + +DateTime.Now.Second;
                            topic_data = item.topic;

                            break;
                        }
                        else
                        {
                            ViewBag.Rem_Time = "0:00:00";

                            ViewBag.topic = "No topic found";
                            topic_data = "No topic found";

                            ViewBag.start_time = "0:00:00";
                            ViewBag.current_time = "0:00:00";
                            ViewBag.endtime = "0:00:00";
                        }
                    }
                }
                else
                {
                    ViewBag.Rem_Time = "0:00:00";

                    ViewBag.topic = "No topic found";
                    topic_data = "No topic found";

                    ViewBag.start_time = "0:00:00";
                    ViewBag.current_time = "0:00:00";
                    ViewBag.endtime = "0:00:00";

                }

            }
            catch (Exception)
            {
                ViewBag.Error = "";
            }
            // TodayClass_data = model.Where(a => a.trainername == CurrentSession.UserName).ToList();
            return Json(new { data = GetLessonPlanDetails_model,topic= topic_data ,remainig_time = ViewBag.Rem_Time , start_time = ViewBag.start_time , current_time = ViewBag.current_time, endtime= ViewBag.endtime });
        }

        public ActionResult BlockUser()
        {
            //var data = _IUserService.GetRecordById(CurrentSession.UserId);
           
            //_IUserService.SaveRecord(data);

            return Json(new { data = "Successfully Block user"});
        }
    }
}
public class GetLessonPlanDetails
{
    public string topic { get; set; }
    public string starttime { get; set; }
    public string startmeridian { get; set; }
    public string endtime { get; set; }
    public string endmeridian { get; set; }
    public string slidefrom { get; set; }
    public string slideto { get; set; }
}
public  class GetLessonPlanDetails_final
{
    public string status { get; set; }
    public List<GetLessonPlanDetails> data { get; set; }

}