using SSTM.Business.Interfaces;
using SSTM.Core.Course_Reminder;
using SSTM.Core.CourseTrackers;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.AWS;
using SSTM.Models.Course;
using SSTM.Models.Course_Reminder;
using SSTM.Models.CourseTrackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class CourseReminderController : BaseController
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
        private readonly ICourseReminderService _ICourseReminderService;
        private readonly ICourseRenewalService _ICourseRenewalService;
        private readonly ICourseReminderLatterUndertakingService _ICourseReminderLatterUndertakingService;
        private readonly ICourseReminderTrackerService _ICourseReminderTrackerService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion
        #region Class Properties Definitions
        public CourseReminderController
            (IExceptionLogService exceptionLogService, IActivityLogService activityLogService, IConfigService configService,
            IUserService userService, IRoleService roleService, ICourseStatusService courseStatusService, ICourseService courseService,
            ICourseAssignmentService courseAssignmentService, ICourseDocumentService courseDocumentService,
            ICourseDocVersionService courseDocVersionService, ICourseDocRemarksService courseDocRemarksService,
            ICourseSharingService courseSharingService, IMainCourseService MainCourseService,
            ISubCourseService SubCourseService, ICourseTrackersService CourseTrackersService,
             ICourseDownloadUserService CourseDownloadUserService, ICourseReminderService ICourseReminderService,
             ICourseRenewalService ICourseRenewalService, ICourseReminderLatterUndertakingService ICourseReminderLatterUndertakingService,
             ICourseReminderTrackerService ICourseReminderTrackerService)
        {
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
            _ICourseReminderService = ICourseReminderService;

            _ICourseRenewalService = ICourseRenewalService;
            _ICourseReminderLatterUndertakingService = ICourseReminderLatterUndertakingService;
            _ICourseReminderTrackerService = ICourseReminderTrackerService;
        }
        #endregion

        // GET: CourseReminder
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
        #region New Course List

        [HttpGet]
        public ActionResult GetNewCoursesList(bool MasterCourse, long MasterCourseId)
        {
            try
            {
                var sBuilder = new StringBuilder();
                List<CourseReminder> list = new List<CourseReminder>();
                list = CurrentSession.UserRole == "Developer" || CurrentSession.UserRole == "Aassociate Developer" ?
                            _ICourseReminderService.GetAllRecordwithDeveloper(MasterCourse, MasterCourseId, CurrentSession.UserId).ToList() :
                            _ICourseReminderService.GetAllRecord(MasterCourse, MasterCourseId).ToList();

                foreach (var item in list)
                {
                    string renewdate = "", refdoc = "", action = "",
                        renewalform = "", latter = "", Fixdeveloper = "";

                    var course = _ICourseService.GetRecordNewcourseById(item.Id);
                    var SharedCourseList = _ICourseSharingService.GetListofSharedCourseDocs(course.Id);
                    try
                    {
                        renewdate = item.renew_date.Value.ToString("dd/MM/yyyy");
                    }
                    catch (Exception)
                    {
                        renewdate = "";
                    }

                    string courseNameTrack = "";
                    var courseNameTrack1 = course.Statusid;
                    string sharedcourse = "";
                    if (item.steps == 1)
                    {
                        courseNameTrack = "New Course";
                    }
                    else if (item.steps == 2)
                    {
                        courseNameTrack = "Course Proposal";
                    }
                    else if (item.steps == 3)
                    {
                        courseNameTrack = "AEB meeting";
                    }
                    else if (item.steps == 4)
                    {
                        courseNameTrack = "Latter";
                        if (item.director_latter_signature != null)
                        {
                            if (courseNameTrack1 == 1)
                                courseNameTrack = "Draft (By Developer)";
                            else if (courseNameTrack1 == 2)
                                courseNameTrack = "Submitted (By Developer)";
                            else if (courseNameTrack1 == 3)
                                courseNameTrack = "SME Comments";
                            else if (courseNameTrack1 == 4)
                                courseNameTrack = "Improvement (By Developer)";

                            if (SharedCourseList.Count() != 0)
                                courseNameTrack = "Shared";
                        }
                    }

                    string view = "ViewCourseDoc('" + item.doc_file + "')";
                    string delete = "DeleteNewCourseDoc('" + item.Id + "')";
                    string edit = "OpenAddOrUpdateCourseModal('" + item.Id + "')";
                    string fixcourseId = "FixCourseDeveloperModal('" + item.Id + "')";
                    string renew = "OpenAddOrUpdateReminderCourseModal('" + item.Id + "')";
                    string lattertaiking = "OpenLatterCourseModal('" + item.Id + "')";
                    string NewCourse = "NewCourseShowCourse('" + item.Id + "','" + item.course_name.ToString().Replace(" ", "") + "')";
                    if (!string.IsNullOrEmpty(item.doc_file))
                        refdoc = "<a href='javascript:void(0)' style='color: #ff8707; ' class='longnameellipsis' title ='view document' onclick=" + view + ">" + item.doc_file + "</a>";
                    else
                        refdoc = "-";

                    //if ((item.renew_date.Value - item.reminder_created_date.Value).Days <= 0)


                    string subcourse = "<button type='button' title='Sub Course' class='btn btn-warning btn-sm btnSubCourse'>" +
                                    "New Sub Course </button>";

                    if (CurrentSession.UserRole == "Administration" || CurrentSession.UserRole == "Director")
                    {
                        if (item.steps == 2)
                            Fixdeveloper = "<a href='javascript:void(0)' title='Fixing of course developer ' onclick=" + fixcourseId + ">Confirm</a>";
                    }
                    //if (CurrentSession.UserRole == "Administration" || CurrentSession.UserRole == "Director")
                    //{
                    if (CurrentSession.UserRole == "Administration" || CurrentSession.UserRole == "Director")
                    {
                        latter = "<a href='javascript:void(0)' title='latter document' onclick=" + lattertaiking + ">Signature</a>";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.latter_signature))
                            latter = "<a href='javascript:void(0)' title='latter document' onclick=" + lattertaiking + ">Signature</a>";
                        else
                            latter = "<a href='javascript:void(0)' title='latter document' onclick=" + lattertaiking + ">Signatured</a>";
                    }
                    // }
                    if (CurrentSession.UserRole == "Developer" || CurrentSession.UserRole == "Aassociate Developer")
                    {
                        if (string.IsNullOrEmpty(item.latter_signature))
                            latter = "<a href='javascript:void(0)' title='latter document' onclick=" + lattertaiking + "> Signature</a>";
                    }

                    if (CurrentSession.UserRole != "AEB")
                        action = "<a class='btn btn-sm  btn-danger' href='javascript:void(0)' title='Delete document' onclick=" + delete + "><i class='fa fa-trash'></i></a> &nbsp" +
                           "<a  class='btn btn-sm  btn-primary' href = 'javascript:void(0)' title = 'Edit document' onclick = " + edit + " ><i class='fa fa-edit'></i></a>";
                    else
                        action = "";

                    if (item.steps == 4)
                    {
                        action += "<a  class='btn btn-sm  btn-primary' style='margin-left:5px;' href = 'javascript:void(0)' title = 'All Course Process' onclick = " + NewCourse + " >Course Process >></a>";
                    }
                    double reminingDays = Convert.ToDateTime(item.renew_date).Subtract(DateTime.Today).TotalDays;
                    string style = "";
                    if (reminingDays <= 0)
                    {
                        reminingDays = 0;
                        style = "style='background: #ff1744; color: white;'";
                        if (CurrentSession.UserRole != "AEB")
                            action = "<a class='btn btn-sm btn-default' href='javascript:void(0)' title='Delete document' onclick=" + delete + "><i class='fa fa-trash'></i></a>";
                        else
                            action = "";
                        latter = "";
                        Fixdeveloper = "";
                        renewalform = "<a class='btn btn-sm  btn-warning' href='javascript:void(0)' title='Renewal Form' onclick=" + renew + ">Renewal Reminder</a>";
                    }
                    if (item.steps != 4)
                    {
                        style = "style='background:rgba(30, 11, 48, 0.88); color: white;'";
                        if (reminingDays <= 0)
                        {
                            style = "style='background: #ff1744; color: white;'";
                        }
                    }
                    sBuilder.Append(
                        "<tr id='" + item.Id + "' " + style + ">" +
                            "<td>" + item.course_name + "</td>" +
                            "<td>" + item.course_type_name + "</td>" +
                            "<td>" + item.course_level_name + "</td>" +
                            "<td>" + item.CreatedOn.Value.ToString("dd/MM/yyyy") + "</td>" +
                            "<td>" + item.renewal_reminder + "<br> Days ( " + (item.reminder_days) + " )</td>" +
                            //"<td>" + item.course_duration + "</td>" +
                            "<td>" + reminingDays + "</td>" +
                            "<td>" + item.remark + "</td>" +
                            "<td>" + subcourse + "</td>" +
                            "<td>" + latter + "</td>" +
                            "<td>" + Fixdeveloper + "</td>" +
                            "<td>" + renewdate + " " + renewalform + "</td>" +
                            "<td >" +
                                "<a  title='Traking' onclick='btnTraking' class='btn btn3d btn-warning btn-sm btnTraking'>" + courseNameTrack + "</a>" +
                            "</td>" +
                            "<td> " + refdoc + "</td>" +
                            "<td class='text-center'>" + action + "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseReminder", "GetNewCoursesList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        [HttpPost]
        public ActionResult GetNewCourseById(int Id)
        {
            try
            {
                var model = _ICourseReminderService.GetRecordById(Id).ToModel();
                if (model == null)
                {
                    model = new CourseReminderModel();
                }
                #region LI System to get Course level List

                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "0", Text = "--- Select Level ----" });
                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.AllCourseLevel();
                    List<CourseLevelModel> CourseLevelModel = (new JavaScriptSerializer()).Deserialize<List<CourseLevelModel>>(data);

                    CourseLevelModel.ForEach(a =>
                    {
                        if (model.course_level_name == a.courselevel)
                            list.Add(new SelectListItem { Value = Convert.ToString(a.courselevel), Text = a.courselevel, Selected = true });
                        else
                            list.Add(new SelectListItem { Value = Convert.ToString(a.courselevel), Text = a.courselevel });
                    });
                    TempData["courselevel"] = new SelectList(list, "Value", "Text");

                }
                catch (Exception)
                {
                    var list = new List<SelectListItem>();
                    TempData["courselevel"] = new SelectList(list, "Value", "Text");
                }
                #endregion

                #region Li system to get Course Type
                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "0", Text = "--- Select Type  ----" });
                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.AllCourseType();
                    List<CourseTypeModel> CourseLevelModel = (new JavaScriptSerializer()).Deserialize<List<CourseTypeModel>>(data);

                    CourseLevelModel.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.type), Text = a.type });
                    });
                    TempData["coursetype"] = new SelectList(list, "Value", "Text");

                }
                catch (Exception)
                {
                    var list = new List<SelectListItem>();
                    TempData["coursetype"] = new SelectList(list, "Value", "Text");
                }
                #endregion

                #region LI system course list
                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "0", Text = "--- Select Course Name  ----" });

                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.CourseProposal();

                    List<Course_Li_Reminder_Model> Course_Li_Reminder = (new JavaScriptSerializer()).Deserialize<List<Course_Li_Reminder_Model>>(data);
                    Course_Li_Reminder.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.courseid), Text = a.CourseName });
                    });
                    TempData["CourseName"] = new SelectList(list, "Value", "Text");
                }
                catch (Exception ex)
                {
                    string e = ex.Message;
                    var list = new List<SelectListItem>();
                    TempData["CourseName"] = new SelectList(list, "Value", "Text");
                }
                #endregion


                ViewBag.newcourseid = Id;
                ViewBag.courseid = model.li_course_id;
                ViewBag.DeveloperId = model.DeveloperId;

                ViewBag.reminder = model.reminder_days;
                if (Id != 0)
                {
                    var DeveloeperEntity = _IUserService.GetDefaultList().Where(a => a.Id == model.DeveloperId).FirstOrDefault();
                    var role = _IRoleService.GetRecordById(DeveloeperEntity.RoleId);
                    ViewBag.role = role.Id;
                    ViewBag.rolename = role.RoleName;
                }
                else
                {
                    ViewBag.role = "";
                    ViewBag.rolename = "";
                }
                return PartialView("_RequestCourseAddOrUpdate", model);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Coursereminder", "GetCourseById", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }


        [HttpPost]
        public ActionResult GetFixCourse(int Id)
        {
            try
            {
                var model = _ICourseReminderService.GetRecordById(Id).ToModel();
                if (model == null)
                {
                    model = new CourseReminderModel();
                }
                #region LI System to get Course level List

                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "0", Text = "--- Select Level ----" });
                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.AllCourseLevel();
                    List<CourseLevelModel> CourseLevelModel = (new JavaScriptSerializer()).Deserialize<List<CourseLevelModel>>(data);

                    CourseLevelModel.ForEach(a =>
                    {
                        if (model.course_level_name == a.courselevel)
                            list.Add(new SelectListItem { Value = Convert.ToString(a.courselevel), Text = a.courselevel, Selected = true });
                        else
                            list.Add(new SelectListItem { Value = Convert.ToString(a.courselevel), Text = a.courselevel });
                    });
                    TempData["courselevel"] = new SelectList(list, "Value", "Text");

                }
                catch (Exception)
                {
                    var list = new List<SelectListItem>();
                    TempData["courselevel"] = new SelectList(list, "Value", "Text");
                }
                #endregion

                #region Li system to get Course Type
                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "0", Text = "--- Select Type  ----" });
                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.AllCourseType();
                    List<CourseTypeModel> CourseLevelModel = (new JavaScriptSerializer()).Deserialize<List<CourseTypeModel>>(data);

                    CourseLevelModel.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.type), Text = a.type });
                    });
                    TempData["coursetype"] = new SelectList(list, "Value", "Text");

                }
                catch (Exception)
                {
                    var list = new List<SelectListItem>();
                    TempData["coursetype"] = new SelectList(list, "Value", "Text");
                }
                #endregion

                #region New course list
                try
                {
                    var entity = _ICourseReminderService.GetAllRecord();

                    var list = new List<SelectListItem>();
                    //list.Add(new SelectListItem { Value = "0", Text = "--- Select Course Name  ----" });

                    //CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    //string data = service.CourseProposal();

                    //List<Course_Li_Reminder_Model> Course_Li_Reminder = (new JavaScriptSerializer()).Deserialize<List<Course_Li_Reminder_Model>>(data);
                    entity.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.Id), Text = a.course_name });
                    });
                    TempData["CourseName"] = new SelectList(list, "Value", "Text");
                }
                catch (Exception ex)
                {
                    string e = ex.Message;
                    var list = new List<SelectListItem>();
                    TempData["CourseName"] = new SelectList(list, "Value", "Text");
                }
                #endregion


                ViewBag.newcourseid = Id;
                ViewBag.courseid = model.li_course_id;
                ViewBag.DeveloperId = model.DeveloperId;

                ViewBag.reminder = model.reminder_days;
                if (Id != 0)
                {
                    var DeveloeperEntity = _IUserService.GetDefaultList().Where(a => a.Id == model.DeveloperId).FirstOrDefault();
                    var role = _IRoleService.GetRecordById(DeveloeperEntity.RoleId);
                    ViewBag.role = role.Id;
                    ViewBag.rolename = role.RoleName;
                }
                else
                {
                    ViewBag.role = "";
                    ViewBag.rolename = "";
                }
                return PartialView("_FixCourseDeveloper", model);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Coursereminder", "GetFixCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        #region Fix Developer for New Course
        [HttpPost]
        public ActionResult FixDeveloperNewCourse(string data)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            try
            {
                CourseReminderModel model = new JavaScriptSerializer().Deserialize<CourseReminderModel>(data);
                int step = (int)model.steps;
                #region New Course Save
                var entity = _ICourseReminderService.GetRecordById(model.Id);
                if (step == 2)
                {
                    entity.DeveloperId = model.DeveloperId;
                    entity.steps = Convert.ToInt32(coursestep.AEBfixDeveloper);
                    var courseId = _ICourseReminderService.SaveRecord(entity);
                    #endregion

                    #region Sent mail Fixing of course developer  to Letter of undertaking Link
                    string callbackurl = Request.Url.Host != "localhost"
        ? Request.Url.Host : Request.Url.Authority;
                    var uri = new Uri(Request.Url.AbsoluteUri);
                    string url = uri.Scheme + "://" + callbackurl + "/" + "QPAPI/Letterofundertaking/" + entity.Id + "@d55lmrVlrJPg4iuy4PJ0A==mrVlrJPg4iuy";
                    if (configEntity != null)
                    {
                        var developerEntity = _IUserService.GetDefaultList().Where(a => a.Id == entity.DeveloperId).ToList(); //developer

                        foreach (var item in developerEntity)
                        {
                            var emailBody = UtilityHelper.GetEmailTemplate("Letterofundertaking.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                                .Replace("@link@", url);

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(item.Email),
                                Subject = "SSTM Letter of undertaking  ",
                                Message = emailBody,
                                SMTPHost = configEntity.Host,
                                SMTPPort = configEntity.Port,
                                SMTPEmail = configEntity.Email,
                                SMTPPassword = configEntity.Pass,
                                EnableSsl = configEntity.EnableSsl
                            });
                        }
                    }
                    #endregion


                    #region Sent mail Fixing of course Director  to Letter of undertaking Link

                    string callbackurl1 = Request.Url.Host != "localhost" ? Request.Url.Host : Request.Url.Authority;
                    var uri1 = new Uri(Request.Url.AbsoluteUri);
                    string url1 = uri.Scheme + "://" + callbackurl + "/" + "QPAPI/directorLetterofundertaking/" + entity.Id + "@d55lmrVlrJPg4iuy4PJ0A==mrVlrJPg4iuy";

                    if (configEntity != null)
                    {
                        var DirectorEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == 4).ToList(); //director list

                        foreach (var item in DirectorEntity)
                        {
                            var emailBody = UtilityHelper.GetEmailTemplate("Letterofundertaking.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                                .Replace("@link@", url1);

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(item.Email),
                                Subject = "SSTM Letter of undertaking  ",
                                Message = emailBody,
                                SMTPHost = configEntity.Host,
                                SMTPPort = configEntity.Port,
                                SMTPEmail = configEntity.Email,
                                SMTPPassword = configEntity.Pass,
                                EnableSsl = configEntity.EnableSsl
                            });
                        }
                    }
                    #endregion

                    #region New course Track Entry

                    var NewcourseTrackentity = _ICourseReminderTrackerService.GetDocument(model.Id);
                    if (NewcourseTrackentity == null)
                    {
                        NewcourseTrackentity = new NewCourseTrackingData();
                        NewcourseTrackentity.AEBMeetingUserid = CurrentSession.UserId;
                        NewcourseTrackentity.NewCourseId = model.Id;
                        NewcourseTrackentity.AEBMeetingDate = DateTime.Now;
                        _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);
                    }
                    else
                    {
                        NewcourseTrackentity.AEBMeetingUserid = CurrentSession.UserId;
                        NewcourseTrackentity.AEBMeetingDate = DateTime.Now;
                        _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);
                    }
                    #endregion
                }
                else
                {
                    if (step == 1)
                        return Json(new { result = false, message = "This couse is course analysis required and current course status is pending course first course analysis approve after move process ...!" });
                    else
                        return Json(new { result = false, message = "This couse is course analysis required and current course status is reject course first course analysis approve after move process ...!" });
                }
                return Json(new { result = true, message = "Sucessfully fix Developer New course" });

            }
            catch (System.Exception ex)
            {
                string e = ex.Message;
                // _IExceptionLogService.SaveRecord(ex, "CourseReminder", "SaveNewCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        #endregion
        public ActionResult Li_new_courseList(string id)
        {
            CourseService.SSTM service = new SSTM.CourseService.SSTM();
            string data = service.CourseProposal();

            List<Course_Li_Reminder_Model> Course_Li_Reminder = (new JavaScriptSerializer()).Deserialize<List<Course_Li_Reminder_Model>>(data);
            var res = Course_Li_Reminder.Where(c => c.courseid == Convert.ToInt64(id)).FirstOrDefault();
            return Json(new { data = res }, JsonRequestBehavior.AllowGet);
        }
        #region Save New Course
        [HttpPost]
        public ActionResult SaveNewCourse(string data)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            try
            {
                CourseReminderModel model = new JavaScriptSerializer().Deserialize<CourseReminderModel>(data);
                long masterids = 0;
                string NewcourseInLIId = "0";
                if (_ICourseReminderService.isCourseNameExists(model.Id, model.course_name))
                    return Json(new { result = false, message = "Course is already exists with the same name." });

                #region Image Upload to AWS Server
                string fileName = null;
                if (Request.Files.Count > 0)
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase file = files[0];

                    var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();


                    if (fileExtension == ".pdf" || fileExtension.ToLower() == ".doc" || fileExtension.ToLower() == ".docx" || fileExtension.ToLower() == ".pptx" || fileExtension.ToLower() == ".ppt" ||
                    fileExtension.ToLower() == ".xlsx" || fileExtension.ToLower() == ".xls")
                    {
                        #region Save file on cloud storage
                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fileName = testfiles[testfiles.Length - 1];
                        }
                        else
                            fileName = file.FileName;

                        Random generator = new Random();

                        fileName = model.course_name.Replace(" ", "") + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + System.IO.Path.GetExtension(fileName);

                        #region one table multle menu document Upload with folder in s3 bucket
                        string TrainerDocumentdir = "NewCourseReminder";


                        AWSModel awsModel = new AWSModel()
                        {
                            AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                            SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                            BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                            BucketDirectory = TrainerDocumentdir,
                            FileName = fileName,
                            LocalFileStream = file.InputStream
                        };

                        AWSHelper.UploadFile(awsModel);
                        #endregion
                        #endregion
                    }
                    else
                        return Json(new { result = false, message = "Rerence Docuement File Extention (doc,ppt or xlxs,PDF files)" });
                }
                #endregion

                #region New Course Save
                var entity = _ICourseReminderService.GetRecordById(model.Id);
                if (entity != null)
                {
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                }
                else
                {

                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    model.li_course_id = Convert.ToInt64(service.AddNewcourseInLI(model.course_name, 4)); //https://li.eversafe.com.sg/services/XamrinService.asmx/ShowCourseDepartment "idDepartment":4,"Departmentname":"Other"

                    entity = new CourseReminderModel().ToEntity();
                    entity.CreatedBy = CurrentSession.UserId;
                    entity.CreatedOn = DateTime.Now;
                    entity.reminder_created_date = DateTime.Now;
                    entity.is_renew_required = false;
                    entity.latter_undertaking = false;

                    entity.MasterCourse = model.MasterCourse;
                    entity.MasterCoursId = model.MasterCoursId;

                    entity.course_proposal_link = Courseproposal.Pending.ToString();
                    entity.need_analysis_link = Courseproposal.Pending.ToString();

                    entity.steps = Convert.ToInt32(coursestep.mailtodeveloper);
                }


                entity.course_type = model.course_type;
                entity.course_level = model.course_level;
                entity.course_type_name = model.course_type_name;
                entity.course_level_name = model.course_level_name;
                entity.renewal_reminder = model.renewal_reminder;
                entity.course_name = model.course_name;
                entity.li_course_id = model.li_course_id;
                entity.reminder_days = model.reminder_days;
                entity.course_duration = model.course_duration;
                entity.DeveloperId = model.DeveloperId;

                entity.remark = model.remark;
                entity.doc_file = fileName == null ? model.doc_file : fileName;
                entity.renew_date = DateTime.Today.AddDays(Convert.ToInt32(model.reminder_days));
                entity.total_renew_counter = string.IsNullOrEmpty(model.total_renew_counter.ToString()) ? 0 : model.total_renew_counter;
                var courseId = _ICourseReminderService.SaveRecord(entity);
                #endregion

                #region Insert or Update Course Entity

                var CourseEntity = _ICourseService.GetRecordNewcourseById(model.Id);
                CourseModel Course = new CourseModel();
                if (CourseEntity != null)
                    Course.Id = CourseEntity.Id;
                else
                    Course.Id = 0;

                Course.MasterCourse = true;
                Course.MasterCoursId = 0;
                Course.CourseName = model.course_name;
                Course.isActive = true;
                Course.NewCourseId = courseId;
                Course.AirLineCourseId = 0;
                Course.isDeleted = false;
                Course.CourseType = "NewCourse";

                var controller = DependencyResolver.Current.GetService<CourseController>();
                controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);
                controller.SaveCourse(Course);
                #endregion

                #region Email to developer to fill up course proposal
                if (model.Id == 0)
                {
                    if (configEntity != null)
                    {
                        var developerEntity = _IUserService.GetDefaultList().Where(a => a.Id == model.DeveloperId).ToList();

                        foreach (var item in developerEntity)
                        {
                            string path = UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/" + fileName;
                            var emailBody = UtilityHelper.GetEmailTemplate("CourseProposalToDeveloper.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                                .Replace("@CourseName@", model.course_name)
                                .Replace("@Remark@", model.remark)
                                .Replace("@duration@", model.course_duration)
                                .Replace("@document@", fileName == "" ? "" : path)
                                .Replace("@courseproposal@", "https://li.eversafe.com.sg")
                                .Replace("@needanalysis@", "https://li.eversafe.com.sg");

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(item.Email),
                                Subject = "SSTM fill up course proposal (" + model.course_name + ")",
                                Message = emailBody,
                                SMTPHost = configEntity.Host,
                                SMTPPort = configEntity.Port,
                                SMTPEmail = configEntity.Email,
                                SMTPPassword = configEntity.Pass,
                                EnableSsl = configEntity.EnableSsl
                            });
                        }
                        model.steps = 1;
                    }
                }
                #endregion

                if (model.Id == 0)
                {
                    #region New course Track Entry

                    var NewcourseTrackentity = new NewCourseTrackingDataModel().ToEntity();
                    NewcourseTrackentity.NewCourseId = courseId;
                    NewcourseTrackentity.NewCourseSubmitUserid = CurrentSession.UserId;
                    NewcourseTrackentity.NewCourseSubmitDate = DateTime.Now;
                    _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);

                    #endregion
                    return Json(new { result = true, message = "Sucessfully Save New Course" });
                }
                else
                    return Json(new { result = true, message = "Sucessfully Updated New Course" });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseReminder", "SaveNewCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        #endregion

        #region Delete new course

        public ActionResult DeleteNewCourse(int id)
        {
            try
            {
                var CourseEntity = _ICourseReminderService.GetRecordById(id);
                _ICourseReminderService.DeleteRecord(id);

                var configEntity = _IConfigService.GetFirstRecord();

                #region delete old file
                if (id != 0)
                {
                    try
                    {
                        AWSModel awsModel1 = new AWSModel()
                        {
                            AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                            SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                            BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                            BucketDirectory = "NewCourseReminder",
                            FileName = CourseEntity.doc_file
                        };
                        AWSHelper.DeleteFile(awsModel1);
                    }
                    catch (Exception)
                    {
                    }
                }
                #endregion

                return Json(new { result = true, message = "Successfully deleted" });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCourseById", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        #endregion

        public ActionResult NewCourseIndex(bool? MasterCourse, long? MasterCourseId, string Coursename, long? NewCourseId)
        {

            GetDocsStatusList();
            if (MasterCourse == null)
                MasterCourse = true;
            if (MasterCourseId == null)
                MasterCourseId = 0;

            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.Coursename = Coursename;
            ViewBag.NewCourseId = NewCourseId;
            return View(CurrentSession);
        }

        #region User Defined functions
        public void GetDocsStatusList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "All" });
            var statusList = _ICourseStatusService.GetList().ToList();
            if (CurrentSession.UserRole == "Staff")
            {
                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Pending").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Pending").FirstOrDefault().Status.ToString()
                });

                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Submitted").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Submitted").FirstOrDefault().Status.ToString()
                });

            }
            else if (CurrentSession.UserRole == "SME")
            {
                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Under Review").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Under Review").FirstOrDefault().Status.ToString()
                });

                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Under Improvement").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Under Improvement").FirstOrDefault().Status.ToString()
                });

                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Released").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Released").FirstOrDefault().Status.ToString()
                });
            }
            else
            {
                statusList.ForEach(a =>
                {
                    list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.Status });
                });
            }

            TempData["CourseStatusList"] = new SelectList(list, "Value", "Text");
        }
        #endregion

        [HttpPost]
        public ActionResult GetReminderCourseForm(Int64 Id, bool MasterCourse, long MasterCourseId)
        {
            try
            {
                var model = _ICourseReminderService.GetRecordById(Convert.ToInt64(Id));
                var data = _ICourseReminderService.GetAllRecord(MasterCourse, MasterCourseId).ToList();
                var list = new List<SelectListItem>();
                data.ForEach(a =>
                {
                    list.Add(new SelectListItem { Value = Convert.ToString(a.Id), Text = a.course_name });
                });
                TempData["courseName"] = new SelectList(list, "Value", "Text");
                ViewBag.newcourseid = Id;
                ViewBag.courseid = model.li_course_id;
                ViewBag.DeveloperId = model.DeveloperId;

                ViewBag.reminder = model.reminder_days;
                var DeveloeperEntity = _IUserService.GetDefaultList().Where(a => a.Id == model.DeveloperId).FirstOrDefault();
                var role = _IRoleService.GetRecordById(DeveloeperEntity.RoleId);
                ViewBag.role = role.Id;
                ViewBag.rolename = role.RoleName;

                return PartialView("_Renewal_course", data);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Coursereminder", "GetReminderCourseForm", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }


        #region Save Reminder Course
        [HttpPost]
        public ActionResult SaveReminderCourse(string courseid, string renewremiderday, string courseremindertext, long developerid)
        {
            try
            {
                #region Renew Course And Update Course Remainder Details

                var entity = _ICourseReminderService.GetRecordById(Convert.ToInt64(courseid));
                entity.DeveloperId = developerid;
                entity.renewal_reminder = courseremindertext;
                entity.reminder_days = Convert.ToInt32(renewremiderday);
                entity.renew_date = DateTime.Today.AddDays(Convert.ToInt32(Convert.ToInt32(renewremiderday)));
                entity.total_renew_counter = entity.total_renew_counter + 1;
                entity.director_latter_signature = "";
                entity.latter_signature = "";
                entity.is_renew_required = false;
                entity.latter_undertaking = false;
                entity.need_analysis_link = Courseproposal.Pending.ToString();
                entity.course_proposal_link = Courseproposal.Pending.ToString();
                entity.steps = 1;
                var courseId = _ICourseReminderService.SaveRecord(entity);

                #endregion
                try
                {
                    #region Renewal Last Entry
                    try
                    {
                        var Renewalentity = new CourseRenewalModel().ToEntity();
                        Renewalentity.CreatedBy = CurrentSession.UserId;
                        Renewalentity.CreatedOn = DateTime.Now;
                        Renewalentity.Course_id = Convert.ToInt64(courseid);
                        Renewalentity.Renew_date = DateTime.Now;
                        Renewalentity.Duration = courseremindertext;
                        _ICourseRenewalService.SaveRecord(Renewalentity);
                    }
                    catch (Exception)
                    {
                    }
                    #endregion

                    #region Remove entry in Course Reminder Tracker
                    try
                    {
                        var courseTrack = _ICourseReminderTrackerService.GetDocument(entity.Id);
                        courseTrack.LetterofundertakingDate = null;
                        courseTrack.LetterofundertakingUserId = null;
                        courseTrack.NeedanalysisDate = null;
                        courseTrack.NeedanalysisUserid = null;
                        courseTrack.NewCourseSubmitDate = DateTime.Now;
                        courseTrack.NewCourseSubmitUserid = CurrentSession.UserId;
                        courseTrack.AEBMeetingDate = null;
                        courseTrack.AEBMeetingUserid = null;
                        courseTrack.CourseProposalDate = null;
                        courseTrack.CourseProposalUserid = null;
                        courseTrack.RenewReminderUser = null;
                        courseTrack.RenewReminderDate = null;

                        _ICourseReminderTrackerService.SaveRecord(courseTrack);
                    }
                    catch (Exception)
                    {
                    }
                    #endregion

                    #region Email to developer to fill up course proposal
                    var configEntity = _IConfigService.GetFirstRecord();
                    if (entity.Id != 0)
                    {
                        if (configEntity != null)
                        {
                            var developerEntity = _IUserService.GetDefaultList().Where(a => a.Id == entity.DeveloperId || a.RoleId == 4).ToList();

                            foreach (var item in developerEntity)
                            {
                                string path = "";
                                if (!string.IsNullOrEmpty(entity.doc_file))
                                    path = UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/" + entity.doc_file;


                                var emailBody = UtilityHelper.GetEmailTemplate("NewCourseRemindermail.html").ToString();
                                emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                                    .Replace("@CourseName@", entity.course_name)
                                    .Replace("@courseproposal@", "https://li.eversafe.com.sg")
                                    .Replace("@needanalysis@", "https://li.eversafe.com.sg");

                                EmailHelper.SendMail(new Models.EmailModel.EmailModel
                                {
                                    From = configEntity.Email,
                                    To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(item.Email),
                                    Subject = "SSTM Reminder mail (" + entity.course_name + ")",
                                    Message = emailBody,
                                    SMTPHost = configEntity.Host,
                                    SMTPPort = configEntity.Port,
                                    SMTPEmail = configEntity.Email,
                                    SMTPPassword = configEntity.Pass,
                                    EnableSsl = configEntity.EnableSsl
                                });
                            }
                        }
                    }
                    #endregion
                }
                catch (System.Exception ex)
                {
                    return Json(new { result = true, message = "Successfully Set Renew Reminder but mail sending issue " + ex.Message });
                }
                return Json(new { result = true, message = "Successfully Set Renew Reminder." });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseReminder", "SaveNewCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        #endregion

        public ActionResult latter_of_undertaking()
        {
            var entity = _ICourseReminderLatterUndertakingService.GetFirstRecord();
            if (entity == null)
            {
                entity = new Course_Reminder_Latter_UndertakingModel().ToEntity();
                entity.CreatedBy = CurrentSession.UserId;
                entity.CreatedOn = DateTime.Now;
            }
            return View(entity);
        }
        #region Save latter
        [HttpPost]
        public ActionResult savelatter(string data)
        {
            var entity = _ICourseReminderLatterUndertakingService.GetFirstRecord();
            if (entity != null)
            {
                entity.UpdatedBy = CurrentSession.UserId;
                entity.UpdatedOn = DateTime.Now;
                entity.Course_id = 0;
            }
            else
            {
                entity = new Course_Reminder_Latter_UndertakingModel().ToEntity();
                entity.CreatedBy = CurrentSession.UserId;
                entity.CreatedOn = DateTime.Now;
                entity.Course_id = 0;
            }
            entity.latter_content = data;
            _ICourseReminderLatterUndertakingService.SaveRecord(entity);
            return Json(new { result = true, message = "Successfully save." });
        }
        #endregion


        [HttpPost]
        public ActionResult CourseReminder_Latter(string id, bool isdeveloper, long developerid, HttpPostedFileBase file)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            var entity = _ICourseReminderService.GetRecordById(Convert.ToInt64(id));
            string fileName = null;
            if (Request.Files.Count > 0)
            {
                //  Get all files from Request object  
                //HttpFileCollectionBase files = Request.Files;
                //HttpPostedFileBase file = files[0];

                var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();


                if (fileExtension == ".pdf" || fileExtension.ToLower() == ".png" || fileExtension.ToLower() == ".jpeg" || fileExtension.ToLower() == ".jpg")
                {
                    #region Save file on cloud storage
                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fileName = testfiles[testfiles.Length - 1];
                    }
                    else
                        fileName = file.FileName;

                    Random generator = new Random();

                    fileName = entity.course_name.Replace(" ", "") + "_Letter_of_undertaking_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + System.IO.Path.GetExtension(fileName);

                    #region one table multle menu document Upload with folder in s3 bucket
                    string TrainerDocumentdir = "NewCourseReminder/Latter";


                    AWSModel awsModel = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = TrainerDocumentdir,
                        FileName = fileName,
                        LocalFileStream = file.InputStream
                    };

                    AWSHelper.UploadFile(awsModel);
                    #endregion
                    #endregion
                }
                else
                    return Json(new { result = false, message = "Rerence Docuement File Extention (PDF files,jpeg,png Or jpg)" });
            }
            if (CurrentSession.UserRole == "Director" || CurrentSession.UserRole == "AEB")
            {
                if (isdeveloper == true)
                    entity.latter_signature = fileName;
                else
                    entity.director_latter_signature = fileName;
            }
            else
            {
                entity.latter_signature = fileName;
            }


            entity.latter_undertaking = !string.IsNullOrEmpty(entity.director_latter_signature) || !string.IsNullOrEmpty(entity.latter_signature) ? true : false;
            entity.steps = Convert.ToInt32(coursestep.latter);
            _ICourseReminderService.SaveRecord(entity);

            if (CurrentSession.UserRole == "Director" || CurrentSession.UserRole == "AEB")
            {
                #region New course Track Entry

                var NewcourseTrackentity = _ICourseReminderTrackerService.GetDocument(entity.Id);

                NewcourseTrackentity.LetterofundertakingUserId = CurrentSession.UserId;
                NewcourseTrackentity.LetterofundertakingDate = DateTime.Now;
                _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);

                #endregion
            }

            #region Email to admin and director to developer sign added for approval.
            if (CurrentSession.UserRole == "Developer")
            {
                if (configEntity != null)
                {
                    var dirAdminEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == 4).ToList();
                    var developerEntity = _IUserService.GetRecordById(developerid);
                    foreach (var item in dirAdminEntity)
                    {
                        var emailBody = UtilityHelper.GetEmailTemplate("NewCourseAssociateDeveloper.html").ToString();
                        emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                            .Replace("@courseName@", entity.course_name)
                            .Replace("@developername@", developerEntity.FirstName + " " + developerEntity.LastName);

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(item.Email),
                            Subject = "SSTM user for associate developer for Course (" + entity.course_name + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                    }
                }
            }
            #endregion

            return Json(new { result = true, message = "Successfully save." });

        }
        [HttpPost]
        public ActionResult GetlatterCourseForm(Int64 Id)
        {
            try
            {
                var configEntity = _IConfigService.GetFirstRecord();
                var model = _ICourseReminderService.GetRecordById(Id);

                model.latter_signature = !string.IsNullOrEmpty(model.latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.latter_signature : "";
                model.director_latter_signature = !string.IsNullOrEmpty(model.director_latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.director_latter_signature : "";
                var entity = _ICourseReminderLatterUndertakingService.GetFirstRecord();
                var userEntity = _IUserService.GetRecordById(model.DeveloperId);
                //StringBuilder latter_data =new StringBuilder();
                var latter_data = entity.latter_content.ToString();
                if (userEntity != null)
                {
                    try
                    {
                        var Li_trainer = Li_trainer_detail(userEntity.Trainer_AirLine_id);
                        latter_data = latter_data.Replace("@@name@@", userEntity.FirstName + " " + userEntity.LastName).Replace("@@icno@@", Li_trainer.fin);
                    }
                    catch (Exception)
                    {
                        latter_data = latter_data.Replace("@@name@@", "Developer not assing Li Id").Replace("@@icno@@", "0");
                    }
                }
                ViewBag.newcourseid = Id;
                ViewBag.latter = latter_data;
                return PartialView("_developer_latter_undertaking", model);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Coursereminder", "GetReminderCourseForm", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        public LITrainerModel Li_trainer_detail(long? trainerid)
        {
            LITrainerModel LitrinModel = new LITrainerModel();
            CourseService.SSTM service = new SSTM.CourseService.SSTM();
            string data = service.AllTrainer();

            List<LITrainerModel> Li_trainer_list = (new JavaScriptSerializer()).Deserialize<List<LITrainerModel>>(data);
            LitrinModel = Li_trainer_list.Where(a => a.TrainerID == trainerid).FirstOrDefault();

            return LitrinModel;
        }

        public ActionResult developerList(string rolename)
        {
            var roleId = _IRoleService.GetRecordIdByName(rolename);
            var DeveloeperEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == roleId).ToList();
            return Json(new { result = DeveloeperEntity, message = "Successfully save." });
        }


        #region New course Tracker

        public ActionResult NewCourseStatus(long Courseid, bool MasterCourse, long MasterCourseId, string courseName, int flag)
        {

            NewCourseTrackingModel data = _ICourseReminderTrackerService.GetNewCoursesTrackWithData(Courseid, flag);
            if (data == null)
            {
                data = new NewCourseTrackingModel();
            }
            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.oldDocnotrack = "Old Document So, can't track";
            ViewBag.courseName = courseName;
            return View(data);
        }

        #endregion
    }
}
public class CourseLevelModel
{
    public string courselevel { get; set; }
}
public class CourseTypeModel
{
    public string type { get; set; }
}

public class LITrainerModel
{
    public long? TrainerID { get; set; }
    public string TrainerName { get; set; }
    public string fin { get; set; }
}