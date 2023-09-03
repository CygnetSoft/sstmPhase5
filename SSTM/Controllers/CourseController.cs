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
    public class CourseController : BaseController
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

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public CourseController
            (IExceptionLogService exceptionLogService, IActivityLogService activityLogService, IConfigService configService,
            IUserService userService, IRoleService roleService, ICourseStatusService courseStatusService, ICourseService courseService,
            ICourseAssignmentService courseAssignmentService, ICourseDocumentService courseDocumentService,
            ICourseDocVersionService courseDocVersionService, ICourseDocRemarksService courseDocRemarksService,
            ICourseSharingService courseSharingService, IMainCourseService MainCourseService,
            ISubCourseService SubCourseService, ICourseTrackersService CourseTrackersService,
             ICourseDownloadUserService CourseDownloadUserService)
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
        }
        #endregion

        public ActionResult Index(bool? MasterCourse, long? MasterCourseId, string Coursename)
        {

            GetDocsStatusList();
            if (MasterCourse == null)
                MasterCourse = true;
            if (MasterCourseId == null)
                MasterCourseId = 0;

            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.Coursename = Coursename;

            return View(CurrentSession);
        }
        public ActionResult StaffIndex(bool? MasterCourse, long? MasterCourseId, string Coursename)
        {
            GetDocsStatusList();
            if (MasterCourse == null)
                MasterCourse = true;
            if (MasterCourseId == null)
                MasterCourseId = 0;

            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.Coursename = Coursename;
            return View(CurrentSession);
        }

        #region ISO & Edu trust
        public ActionResult ISOEdutrust(bool? MasterCourse, long? MasterCourseId, string Coursename)
        {

            GetDocsStatusList();
            if (MasterCourse == null)
                MasterCourse = true;
            if (MasterCourseId == null)
                MasterCourseId = 0;

            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.Coursename = Coursename;

            return View(CurrentSession);
        }
        #endregion

        #region Course List


        [HttpGet]
        public ActionResult GetCoursesList(int isActive, long statusId, string type, bool MasterCourse, long MasterCourseId, long NewcourseId)
        {
            try
            {
                var sBuilder = new StringBuilder();

                if (CurrentSession.UserRole == "HR")
                    statusId = _ICourseStatusService.GetRecordIdByName("Approved");
                long Userid = 0;
                if (CurrentSession.UserRole == "Staff")
                {
                    Userid = CurrentSession.UserId;
                }
                if (type == null)
                {
                    type = "other";
                }
                List<CourseListModel> list = new List<CourseListModel>();
                if (type== "isoedu")
                {
                     list = CurrentSession.UserRole == "HR" || CurrentSession.UserRole == "Trainer" ||
                           CurrentSession.UserRole == "Print Incharge" || CurrentSession.UserRole == "SME"
                               ? _ICourseService.Get_Iso_Edu_ComonCoursesList(isActive, statusId, MasterCourse, MasterCourseId).ToList() :
                               _ICourseService.Get_Iso_Edu_CoursesList(isActive, statusId, MasterCourse, MasterCourseId).ToList();
                }
                else if(string.IsNullOrEmpty(NewcourseId.ToString()) || NewcourseId!=0 )
                {
                    list = CurrentSession.UserRole == "HR" || CurrentSession.UserRole == "Trainer" ||
                          CurrentSession.UserRole == "Print Incharge" || CurrentSession.UserRole == "SME"
                              ? _ICourseService.GetComonNewCoursesList(isActive, statusId, MasterCourse, MasterCourseId, NewcourseId).ToList() :
                              _ICourseService.GetNewList(isActive, statusId, MasterCourse, MasterCourseId, NewcourseId).ToList();
                             
                }
                else
                {
                     list = CurrentSession.UserRole == "HR" || CurrentSession.UserRole == "Trainer" ||
                           CurrentSession.UserRole == "Print Incharge" || CurrentSession.UserRole == "SME"
                               ? _ICourseService.GetComonCoursesList(isActive, statusId, MasterCourse, MasterCourseId).ToList() :
                               (type == "other" || type == "") ?
                               _ICourseService.GetList(isActive, statusId, MasterCourse, MasterCourseId).ToList() :
                               _ICourseService.GetStaffSubCoursesList(isActive, statusId, Userid, MasterCourse, MasterCourseId).ToList();
                }
               

                if (CurrentSession.UserRole == "SME")
                {
                    foreach (var item in list.ToList())
                    {
                        var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(item.Id);
                        if (courseAssignmentEntity != null && courseAssignmentEntity.SMEId != CurrentSession.UserId)
                            list.Remove(item);
                    }
                }
                if (CurrentSession.UserRole == "HR")
                {
                    foreach (var item in list.ToList())
                    {
                        if (item.CourseType == "staff")
                        {
                            var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(item.Id);
                            if (courseAssignmentEntity != null && courseAssignmentEntity.HRId != CurrentSession.UserId)
                                list.Remove(item);
                        }
                    }
                }

                foreach (var item in list)
                {

                    var SharedCourseList = _ICourseSharingService.GetListofSharedCourseDocs(item.Id);

                    var coursecount = type == "isoedu" ? _ICourseService.Get_Iso_Edu_ComonCoursesList_All(1, false, item.Id):
                                                _ICourseService.GetComonCoursesList_All(1, false, item.Id);
                    var status = item.isActive ?
                        "<label class='badge badge-success'>Active</label>" : "<label class='badge badge-warning'>Inactive</label>";

                    var actions =
                        "<button type='button' title='Edit' class='btn btn-primary btn-sm btnEditCourse'><i class='fa fa-pen'></i></button>&nbsp;" +
                        "<button type='button' title='Delete' class='btn btn-danger btn-sm btnDeleteCourse'><i class='fa fa-trash'></i></button>";
                    var courseNameTrack = item.CourseStatus;
                    if (courseNameTrack == "Pending")
                        courseNameTrack = "Draft (By Developer)";
                    if (courseNameTrack == "Submitted")
                        courseNameTrack = "Submitted (By Developer)";
                    if (courseNameTrack == "Under Review")
                        courseNameTrack = "SME Comments";
                    if (courseNameTrack == "Under Improvement")
                        courseNameTrack = "Improvement (By Developer)";
                    string sharedcourse = "", subcourse = "";
                    if (SharedCourseList.Count() != 0)
                        courseNameTrack = "Shared";

                    if (courseNameTrack == "Released" || courseNameTrack == "Shared")
                        sharedcourse = "<button type='button' title='Share course documents' class='btn btn-success btn-sm btnShareCourse'>" +
                            "<i class='fas fa-share-alt'></i></button>";
                    if (string.IsNullOrEmpty(NewcourseId.ToString()) || NewcourseId != 0)
                    {
                        subcourse = "";
                    }
                    else
                    {
                        subcourse = "<button type='button' title='Sub Course' class='btn btn-warning btn-sm btnSubCourse'>" +
                                    "Sub Course (" + coursecount.Count() + ") </button>";
                    }

                    //reminder process start
                    string reminder = "";
                    if (!string.IsNullOrEmpty(item.reminder_created_date.ToString()))
                    {
                        reminder = "<div class='btn-group'>" +
                                 "<button type = 'button' class='btn btn-danger dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'> Details</button>" +
                                   "<div class='dropdown-menu'>" +
                                    "<a class='dropdown-item'>Created Date :" + item.reminder_created_date.Value.ToString("dd/MM/yyyy") + "</a>" +
                                     "<div class='dropdown-divider'></div>" +
                                   "<a class='dropdown-item' >Reminder :" + item.renewal_reminder + "</a>" +
                                    "<div class='dropdown-divider'></div>" +
                                   "<a class='dropdown-item'>Days:" + item.reminder_days + " </a>" +
                                    "<div class='dropdown-divider'></div>" +
                                   "<a class='dropdown-item' >Expired On :" + item.renew_date.Value.ToString("dd/MM/yyyy") + "</a>" +
                                  
                                   "</div></div>Created: " + item.reminder_created_date.Value.ToString("dd/MM/yyyy") + "<br>Expired On: "+ item.renew_date.Value.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        reminder = "-";
                    }
                    string style = "";
                    double reminingDays = Convert.ToDateTime(item.renew_date).Subtract(DateTime.Today).TotalDays;
                    if (reminingDays <= 0)
                    {
                        if (!string.IsNullOrEmpty(item.renew_date.ToString()))
                            style = "style='background: #ff1744; color: white;'";
                        else
                            style = "";
                    }
                    else
                        style = "";
                    //End reminder process
                    sBuilder.Append(
                        "<tr id='" + item.Id + "' " + style + ">" +
                            "<td>" + item.CourseName + "</td>" +
                            "<td>" + item.Developer + "</td>" +
                            "<td>" + item.SME + "</td>" +
                             "<td>"+ reminder + "</td>" +
                            "<td class='text-center'>" + status + "</td>" +
                            "<td class='text-center'>" +
                               subcourse +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Documents' class='btn btn-info btn-sm btnCourseDocs'>" +
                                    "<i class='fa fa-files-o'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Remarks' class='btn btn-primary btn-sm btnCourseDocsRemarks'>" +
                                    "<i class='fa fa-comments'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Assign' class='btn btn-warning btn-sm btnAssignCourse'>" +
                                    "<i class='fa fa-user-cog'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Approve course documents' class='btn btn-primary btn-sm btnApproveCourse'>" +
                                    "<i class='fas fa-clipboard-check'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Release' class='btn btn-success btn-sm btnReleaseCourse'>" +
                                    "<i class='fa fa-broadcast-tower'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                    sharedcourse +
                            "</td>" +
                            "<td class='text-center'>" + actions + "</td>" +
                             "<td class='text-center'>" +
                                "<button type='button' title='SKIP ALL' class='btn btn3d btn-info btn-sm btnSkipAll'>" +
                                    "SKIP ALL </button>" +
                            "</td>" +

                             "<td class='text-center'>" +
                                "<button type='button' title='Traking' class='btn btn3d btn-warning btn-sm btnTraking'>" + courseNameTrack + "</button>" +
                            "</td>" +

                            "<td class='text-center'>" +
                                "<button type='button' title='Assing Download Users' class='btn btn-default btn-sm btnAssingDownloadUsers'>Assign User</button>" +
                            "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCoursesList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Main Category Data

        public ActionResult MainCourse()
        {
            //GetDocsStatusList();
            return View(CurrentSession);
        }

        public ActionResult MainStaffCourse(bool? MasterCourse, long? MasterCourseId, string Coursename)
        {
            //GetDocsStatusList();
            if (MasterCourse == null)
                MasterCourse = true;
            if (MasterCourseId == null)
                MasterCourseId = 0;

            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.Coursename = Coursename;

            return View(CurrentSession);
        }

        [HttpGet]
        public ActionResult GetMainCoursesList(string type, int isActive)
        {
            try
            {
                StringBuilder sBuilder = new StringBuilder();
                var list = type == "staff" ? _IMainCourseService.GetMainStaffCourseList(isActive).ToList() : _IMainCourseService.GetMainCourseList(isActive).ToList();
                foreach (var item in list)
                {
                    var status = item.isActive ?
                        "<label class='badge badge-success'>Active</label>" : "<label class='badge badge-warning'>Inactive</label>";

                    var actions =
                        "<button type='button' title='Edit' class='btn btn-primary btn-sm btnMainEditCourse'><i class='fa fa-pen'></i></button>&nbsp;";
                    //"<button type='button' title='Delete' class='btn btn-danger btn-sm btnSubDeleteCourse'><i class='fa fa-trash'></i></button>&nbsp;";

                    var ViewButton = "<button type='button' title='Delete' class='btn btn-warning btn-sm btnMainSubCourse'>Sub Course</button>";

                    sBuilder.Append(
                        "<tr id='" + item.Id + "'>" +
                            "<td>" + item.CourseName + "</td>" +
                            "<td class='text-center'>" + status + "</td>" +
                             "<td class='text-center'>" + ViewButton + "</td>" +
                            "<td class='text-center'>" + actions + "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCoursesList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetMainCourseById(long Id, string type)
        {
            try
            {
                var model = _IMainCourseService.GetRecordById(Id).ToModel();
                if (model == null)
                {
                    model = new MainCourseModel();
                    model.isActive = true;
                    model.CourseType = type;
                }
                return PartialView("_AddOrUpdateMainCategory", model);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCourseById", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveMainCourse(MainCourseModel model)
        {
            try
            {
                if (_IMainCourseService.isCourseNameExists(model.Id, model.CourseName, model.CourseType))
                    return Json(new { result = false, message = "Course is already exists with the same name." });

                var entity = _IMainCourseService.GetRecordById(model.Id);
                if (entity != null)
                {
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                }
                else
                {
                    entity = new MainCourseModel().ToEntity();
                    entity.CreatedBy = CurrentSession.UserId;
                    entity.CreatedOn = DateTime.Now;
                }
                entity.CourseName = model.CourseName;
                entity.isActive = model.isActive;
                entity.CourseType = model.CourseType;
                var MaincourseId = _IMainCourseService.SaveRecord(entity);

                //#region Create AWS cloud directory
                //if (model.Id == 0)
                //{
                //    var configEntity = _IConfigService.GetFirstRecord();
                //    AWSHelper.CreateDirectory(new AWSModel()
                //    {
                //        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                //        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                //        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                //        BucketDirectory = MaincourseId.ToString()
                //    });
                //}
                //#endregion


                return Json(new { result = true });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "SaveCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }


        #endregion

        #region Sub Course
        public ActionResult SubCourse(long MainCourse)
        {
            ViewBag.MainCourseId = MainCourse;
            return View(CurrentSession);
        }

        public ActionResult SubStaffCourse(long MainCourse)
        {
            //GetDocsStatusList();
            ViewBag.MainCourseId = MainCourse;
            return View(CurrentSession);
        }
        [HttpGet]
        public ActionResult GetSubCoursesList(string type, int isActive, long MainCourseId)
        {
            try
            {
                StringBuilder sBuilder = new StringBuilder();
                var list = type == "staff" ? _ISubCourseService.GetSubStaffCourseList(isActive, MainCourseId).ToList() : _ISubCourseService.GetSubCourseList(isActive, MainCourseId).ToList();
                foreach (var item in list)
                {
                    var status = item.isActive ?
                        "<label class='badge badge-success'>Active</label>" : "<label class='badge badge-warning'>Inactive</label>";

                    var actions =
                        "<button type='button' title='Edit' class='btn btn-primary btn-sm btnSubEditCourse'><i class='fa fa-pen'></i></button>&nbsp;";// +
                                                                                                                                                      //"<button type='button' title='Delete' class='btn btn-danger btn-sm btnSubDeleteCourse'><i class='fa fa-trash'></i></button>&nbsp;";

                    var ViewButton = "<button type='button' title='Delete' class='btn btn-warning btn-sm btnSubCourse'>Sub Course</button>";

                    sBuilder.Append(
                        "<tr id='" + item.Id + "'>" +
                            "<td>" + item.SubCourseName + "</td>" +
                            "<td class='text-center'>" + status + "</td>" +
                             "<td class='text-center'>" + ViewButton + "</td>" +
                            "<td class='text-center'>" + actions + "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCoursesList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetSubCourseById(long Id, string type, long mainCourseId)
        {
            try
            {
                var model = _ISubCourseService.GetRecordById(Id).ToModel();
                if (model == null)
                {
                    model = new SubCourseModel();
                    model.isActive = true;
                    model.CourseType = type;
                    model.MainCourseId = mainCourseId;
                }
                return PartialView("_AddOrUpdateSubCategory", model);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCourseById", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveSubCourse(SubCourseModel model)
        {
            try
            {
                if (_ISubCourseService.isCourseNameExists(model.Id, model.SubCourseName, model.CourseType))
                    return Json(new { result = false, message = "Course is already exists with the same name." });

                var entity = _ISubCourseService.GetRecordById(model.Id);
                if (entity != null)
                {
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                }
                else
                {
                    entity = new SubCourseModel().ToEntity();
                    entity.CreatedBy = CurrentSession.UserId;
                    entity.CreatedOn = DateTime.Now;
                }
                entity.SubCourseName = model.SubCourseName;
                entity.MainCourseId = model.MainCourseId;
                entity.isActive = model.isActive;
                entity.CourseType = model.CourseType;
                var MaincourseId = _ISubCourseService.SaveRecord(entity);

                //#region Create AWS cloud directory
                //if (model.Id == 0)
                //{
                //    var configEntity = _IConfigService.GetFirstRecord();
                //    AWSHelper.CreateDirectory(new AWSModel()
                //    {
                //        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                //        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                //        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                //        BucketDirectory = MaincourseId.ToString()
                //    });
                //}
                //#endregion


                return Json(new { result = true });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "SaveCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        #endregion

        [HttpPost]
        public ActionResult GetCourseById(long Id, string type, bool MasterCourse, long MasterCourseId)
        {
            try
            {
                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "0", Text = "--- Select  ----" });
                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.AllCourse();
                    List<AirlineCourseModel> AirlineCourseModel = (new JavaScriptSerializer()).Deserialize<List<AirlineCourseModel>>(data);

                    AirlineCourseModel.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.CourseId), Text = a.CourseName });
                    });
                    TempData["AirlineCourse"] = new SelectList(list, "Value", "Text");

                }
                catch (Exception)
                {
                    var list = new List<SelectListItem>();
                    TempData["AirlineCourse"] = new SelectList(list, "Value", "Text");
                }



                var model = _ICourseService.GetRecordById(Id).ToModel();

                if (model == null)
                {
                    model = new CourseModel();
                    model.isActive = true;
                }
                else
                {
                    if (type == "staff")
                    {
                        var getStaffId = _ICourseAssignmentService.GetRecordByCourseId(model.Id);
                        model.StaffId = getStaffId.StaffId;
                        model.HRId = getStaffId.HRId;
                    }
                    //GetMainCouseList(model.CourseType);
                    Get_CourseAndSubCourse(model.CourseType, true, 0);
                }
                model.MasterCourse = MasterCourse;
                model.MasterCoursId = MasterCourseId;
                GetStaffList(8);
                GetHRList(5);

                return PartialView("_AddOrUpdate", model);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCourseById", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        #region Save Course
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveCourse(CourseModel model)
        {
            try
            {
                long masterids = 0;

                if (_ICourseService.isCourseNameExists(model.Id, model.CourseName, model.CourseType))
                    return Json(new { result = false, message = "Course is already exists with the same name." });

                var entity = _ICourseService.GetRecordById(model.Id);
                if (entity != null)
                {
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                    if (model.MasterCoursId != 0)
                    {
                        entity.MasterCoursId = model.MasterCoursId;
                    }
                }
                else
                {
                    entity = new CourseModel().ToEntity();
                    entity.CreatedBy = CurrentSession.UserId;
                    entity.CreatedOn = DateTime.Now;
                    entity.MasterCoursId = model.MasterCoursId;
                    entity.MasterCourse = model.MasterCourse;
                    entity.NewCourseId = model.NewCourseId;
                    entity.Statusid = _ICourseStatusService.GetRecordIdByName("Pending");

                }
                if (model.CourseType == "staff")
                {
                    bool assign = model.Reassign;
                    if (assign != true)
                    {

                        if (model.HRId != 0)
                            entity.Statusid = _ICourseStatusService.GetRecordIdByName("Released");
                        else
                            entity.Statusid = _ICourseStatusService.GetRecordIdByName("Pending");
                    }
                    else
                        entity.Statusid = _ICourseStatusService.GetRecordIdByName("Pending");
                }

                entity.CourseName = model.CourseName;
                entity.AirLineCourseId = model.AirLineCourseId;
                entity.isActive = model.isActive;
                entity.CourseType = model.CourseType;
                var courseId = _ICourseService.SaveRecord(entity);

                #region Track Entry

                var Trackerentity = new CourseTrackersModel().ToEntity();
                Trackerentity.CreateDated = DateTime.Now;
                Trackerentity.Courseid = courseId;
                _ICourseTrackersService.SaveRecord(Trackerentity);

                #endregion
                #region Create AWS cloud directory
                if (model.Id == 0)
                {
                    var configEntity = _IConfigService.GetFirstRecord();
                    AWSHelper.CreateDirectory(new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = courseId.ToString()
                    });
                }
                #endregion

                #region Save record into course assignment table if not exists
                if (string.IsNullOrEmpty(model.StaffId))
                    model.StaffId = "0";
                var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(entity.Id);
                if (courseAssignmentEntity == null)
                {
                    _ICourseAssignmentService.SaveRecord(new CourseAssignmentModel()
                    {
                        CourseId = entity.Id,
                        DirectorId = CurrentSession.UserRole == "Director" ? CurrentSession.UserId : 0,
                        DeveloperId = 0,
                        StaffId = model.StaffId,
                        HRId = model.HRId,
                        SMEId = 0,
                        CreatedBy = CurrentSession.UserId,
                        CreatedOn = DateTime.Now
                    }.ToEntity());
                }
                else
                {
                    courseAssignmentEntity.CourseId = entity.Id;
                    courseAssignmentEntity.DirectorId = CurrentSession.UserRole == "Director" ? CurrentSession.UserId : 0;
                    courseAssignmentEntity.DeveloperId = 0;
                    courseAssignmentEntity.StaffId = model.StaffId;
                    courseAssignmentEntity.SMEId = 0;
                    courseAssignmentEntity.CreatedBy = CurrentSession.UserId;
                    courseAssignmentEntity.CreatedOn = DateTime.Now;

                    _ICourseAssignmentService.SaveRecord(courseAssignmentEntity);
                }
                #endregion
                return Json(new { result = true });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "SaveCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        #endregion


        [HttpPost]
        public ActionResult DeleteCourse(long Id, bool MasterCourse)
        {
            try
            {
                var configEntity = _IConfigService.GetFirstRecord();
                AWSHelper.DeleteDirectory(new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = Id.ToString()
                });

                _ICourseService.DeleteRecord(Id);
                _ICourseAssignmentService.DeleteRecordByCourseId(Id);

                #region Delete records force course documents
                var courseDocsList = _ICourseDocumentService.GetListByCourseId(Id, MasterCourse);
                foreach (var item in courseDocsList)
                {
                    _ICourseDocumentService.DeleteRecord(item.Id);
                    _ICourseDocVersionService.DeleteRecordsByDocId(item.Id);
                    _ICourseDocRemarksService.DeleteRecord(item.Id);
                    _ICourseSharingService.DeleteRecordByDocId(item.Id);
                }
                #endregion

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "DeleteCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult OpenSMEList()
        {
            try
            {
                GetSMEList();

                return PartialView("_SMEListModal");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "OpenSMEList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }

        [HttpPost]
        public ActionResult OpenDownloadUserList()
        {
            try
            {
                GetDownloadUserList();

                return PartialView("_DownloadUser");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "OpenDownloadList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }

        [HttpPost]
        public ActionResult AssignCourseToSME(long courseId, long SMEId, bool MasterCourse)
        {
            try
            {
                var courseEntity = _ICourseService.GetRecordById(courseId);
                var smeEntity = _IUserService.GetRecordById(SMEId);

                if (courseEntity != null && smeEntity != null)
                {
                    long? statusId = _ICourseStatusService.GetRecordIdByName("Under Review");

                    if (statusId > 0)
                    {
                        #region Update course status in Db
                        courseEntity.Statusid = statusId;
                        courseEntity.UpdatedBy = CurrentSession.UserId;
                        courseEntity.UpdatedOn = DateTime.Now;

                        _ICourseService.SaveRecord(courseEntity);
                        #endregion

                        #region Trackers 
                        var Trackerentity = _ICourseTrackersService.GetDocument(courseId);
                        Trackerentity.UpdateDated = DateTime.Now;
                        Trackerentity.SMEAssignUserId = (int)CurrentSession.UserId;
                        Trackerentity.AssignDate = DateTime.Now;
                        _ICourseTrackersService.SaveRecord(Trackerentity);
                        #endregion

                        #region Save record into course assignment table 
                        var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(courseId);
                        if (courseAssignmentEntity != null)
                        {
                            courseAssignmentEntity.SMEId = SMEId;
                            courseAssignmentEntity.UpdatedBy = CurrentSession.UserId;
                            courseAssignmentEntity.UpdatedOn = DateTime.Now;
                            _ICourseAssignmentService.SaveRecord(courseAssignmentEntity);
                        }
                        #endregion

                        #region Send notification to selected SME
                        var configEntity = _IConfigService.GetFirstRecord();
                        if (configEntity != null)
                        {
                            var documents = string.Empty;
                            var courseDocsEntity = _ICourseDocumentService.GetListByCourseId(courseId, MasterCourse).ToList();
                            courseDocsEntity.ForEach(a => { documents += a.DocName != "N/A" ? a.DocName + ", " : ""; });

                            var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                            var developerName = string.Empty;
                            if (developerEntity != null)
                                developerName = developerEntity.FirstName + " " + developerEntity.LastName;

                            var emailBody = UtilityHelper.GetEmailTemplate("NotificationToSMEAndDeveloper.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", smeEntity.FirstName + " " + smeEntity.LastName)
                                .Replace("@CourseName@", courseEntity.CourseName)
                                .Replace("@Documents@", documents.Trim().TrimEnd(','))
                                .Replace("@DeveloperName@", developerName);

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(smeEntity.Email),
                                Subject = "SSTM COURSE ASSIGNMENTS UPDATES (" + courseEntity.CourseName + ")",
                                Message = emailBody,
                                SMTPHost = configEntity.Host,
                                SMTPPort = configEntity.Port,
                                SMTPEmail = configEntity.Email,
                                SMTPPassword = configEntity.Pass,
                                EnableSsl = configEntity.EnableSsl
                            });

                            return Json(new { result = true });
                        }
                        else
                            return Json(new { result = false, message = "Course is assigned successfully to seleted SME. Notification Mail is not sent due to email settings not found." });
                        #endregion


                    }
                    else
                        return Json(new { result = false, message = AppMessages.Exception });
                }
                else
                    return Json(new { result = false, message = AppMessages.Exception });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "AssignCourseToSME", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult AssignCourseToDownload(long courseId, long User_Id)
        {
            try
            {
                var IsexistDowloadUser = _ICourseDownloadUserService.isCourseNameExists(courseId, User_Id);
                if (IsexistDowloadUser == true)
                    return Json(new { result = false, message = "Already assign try another course." });

                // var smeEntity = _ICourseDownloadUserService.SaveRecord();
                var entity = new CourseDownloadUserModel().ToEntity();
                entity.CreatedBy = CurrentSession.UserId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedBy = CurrentSession.UserId;
                entity.UpdatedOn = DateTime.Now;
                entity.CourseId = courseId;
                entity.User_id = User_Id;
                var id = _ICourseDownloadUserService.SaveRecord(entity);
                var courseentity = _ICourseService.GetRecordById(courseId);
                if (id != 0)
                {

                    #region Send notification to selected User
                    var configEntity = _IConfigService.GetFirstRecord();
                    if (configEntity != null)
                    {

                        var userentity = _IUserService.GetRecordById(User_Id);

                        var emailBody = UtilityHelper.GetEmailTemplate("DownloadUserNotification.html").ToString();
                        emailBody = emailBody.Replace("@DearName@", userentity.FirstName + " " + userentity.LastName)
                            .Replace("@CourseName@", courseentity.CourseName)
                            .Replace("@DocumentCurrentStage@", "This Course assign to only Download document");

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(userentity.Email),
                            Subject = "SSTM COURSE Download (" + courseentity.CourseName + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });

                        return Json(new { result = true, message = "Course is assigned successfully to seleted User.Sent Notification Mail." });
                    }
                    else
                        return Json(new { result = false, message = "Course is assigned successfully to seleted User. Notification Mail is not sent due to email settings not found." });
                    #endregion

                }
                else
                    return Json(new { result = false, message = AppMessages.Exception });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "AssignCourseToSME", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult SubmitCourseDocsAssessment(long courseId, bool MasterCourse)
        {
            try
            {
                var usersList = _IUserService.GetList(1).ToList();
                var courseDocsList = _ICourseDocumentService.GetListByCourseId(courseId, MasterCourse).Where(a => !a.isDeleted).ToList();
                var data = _ICourseDocRemarksService.GetCourseremarkByList(courseId);
                bool isAnyDocComment = false;
                var isReassignCount1 = courseDocsList.Where(a => a.isReassigned).Count();
                if (isReassignCount1 == 0)
                {
                    isAnyDocComment = true;
                }
                if (isAnyDocComment == false)
                {
                    var isApprovedCount = courseDocsList.Where(a => a.isApproved).Count();
                    var isReassignCount = courseDocsList.Where(a => a.isReassigned).Count();

                    if (courseDocsList.Count() == (isApprovedCount + isReassignCount))
                    {
                        // var statusString = isReassignCount > 0 ? "Under Improvement" : "Reviewed";//old
                        var statusString = isReassignCount > 0 ? "Under Improvement" : "Released";//new

                        #region Update course status if all the course documents are accepted / remarks are done
                        var courseEntity = _ICourseService.GetRecordById(courseId);
                        courseEntity.Statusid = _ICourseStatusService.GetRecordIdByName(statusString);
                        courseEntity.UpdatedBy = CurrentSession.UserId;
                        courseEntity.UpdatedOn = DateTime.Now;

                        _ICourseService.SaveRecord(courseEntity);
                        #endregion

                        #region Trackers 
                        if (statusString == "Released")
                        {
                            var Trackerentity = _ICourseTrackersService.GetDocument(courseId);
                            Trackerentity.UpdateDated = DateTime.Now;
                            Trackerentity.SMEReviewUserId = (int)CurrentSession.UserId;
                            Trackerentity.SMEReviewDate = DateTime.Now;
                            Trackerentity.SMEAcceptUserId = (int)CurrentSession.UserId;
                            Trackerentity.SMEAcceptDate = DateTime.Now;
                            Trackerentity.ImproveUserId = (int)CurrentSession.UserId;
                            Trackerentity.ImproveDate = DateTime.Now;

                            _ICourseTrackersService.SaveRecord(Trackerentity);
                        }
                        else
                        {
                            var Trackerentity = _ICourseTrackersService.GetDocument(courseId);
                            Trackerentity.UpdateDated = DateTime.Now;
                            Trackerentity.SMEReviewUserId = (int)CurrentSession.UserId;
                            Trackerentity.SMEReviewDate = DateTime.Now;
                            Trackerentity.ImproveUserId = (int)CurrentSession.UserId;
                            Trackerentity.ImproveDate = DateTime.Now;
                            _ICourseTrackersService.SaveRecord(Trackerentity);
                        }
                        #endregion

                        #region Send notification to director and developer for remarks if SME reviewed all the docs
                        var sBuilder = new StringBuilder();

                        var courseDocsRemarksList = _ICourseDocRemarksService.GetListByCourseId(courseId).ToList();
                        foreach (var item in courseDocsRemarksList)
                        {
                            sBuilder.Append(
                                "<tr>" +
                                    "<td>" + item.DocName + "</td>" +
                                    "<td>" + item.Remarks + "</td>" +
                                    "<td>" + item.Suggestion + "</td>" +
                                    "<td>" + item.ReferenceFile + "</td>" +
                                "</tr>");
                        }

                        var configEntity = _IConfigService.GetFirstRecord();
                        if (configEntity != null)
                        {
                            var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(courseEntity.Id);
                            var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                            var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                            if (directorEntity != null)
                            {
                                var emailBody = UtilityHelper.GetEmailTemplate("RemarksSummaryNotificationToDirectorAndDeveloper.html").ToString();

                                if (isReassignCount == 0)
                                    emailBody = emailBody
                                        .Replace("@CourseStatusMessage@", "Kindly notice that the below course is approved by SME and there is no any remarks or suggestion as you can view in below table.<br /><br />");
                                else
                                    emailBody = emailBody
                                        .Replace("@CourseStatusMessage@", "Kindly notice that the below course is under improvement and you can view remarks and / or suggestion in below table. Developer needs to attend following remarks and / or suggestion and re-submitting the updated documents.<br /><br />");

                                emailBody = emailBody
                                    .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                                    .Replace("@SMEName@", CurrentSession.UserName)
                                    .Replace("@CourseName@", courseEntity.CourseName)
                                    .Replace("@DocumentsRemarksSummary@", sBuilder.ToString());

                                EmailHelper.SendMail(new Models.EmailModel.EmailModel
                                {
                                    From = configEntity.Email,
                                    To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(directorEntity.Email) + ";" + UtilityHelper.Decrypt(developerEntity.Email),
                                    Subject = "SSTM COURSE ASSIGNMENTS UPDATES (" + courseEntity.CourseName + ")",
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

                        return Json(new { result = true, message = "Course Documents are submitted successfully. Course status is updated to " + statusString + "." });

                    }
                    else
                        return Json(new { result = false, message = "Please review all the documents for the course. Then try again." });
                }
                else
                {
                    var statusString = "Released";//new

                    #region Update course status if all the course documents are accepted / remarks are done
                    var courseEntity = _ICourseService.GetRecordById(courseId);
                    courseEntity.Statusid = _ICourseStatusService.GetRecordIdByName(statusString);
                    courseEntity.UpdatedBy = CurrentSession.UserId;
                    courseEntity.UpdatedOn = DateTime.Now;

                    _ICourseService.SaveRecord(courseEntity);
                    #endregion

                    #region Trackers 
                    if (statusString == "Released")
                    {
                        var Trackerentity = _ICourseTrackersService.GetDocument(courseId);
                        Trackerentity.UpdateDated = DateTime.Now;
                        Trackerentity.SMEReviewUserId = (int)CurrentSession.UserId;
                        Trackerentity.SMEReviewDate = DateTime.Now;
                        Trackerentity.SMEAcceptUserId = (int)CurrentSession.UserId;
                        Trackerentity.SMEAcceptDate = DateTime.Now;
                        Trackerentity.ImproveUserId = (int)CurrentSession.UserId;
                        Trackerentity.ImproveDate = DateTime.Now;

                        _ICourseTrackersService.SaveRecord(Trackerentity);
                    }
                    else
                    {
                        var Trackerentity = _ICourseTrackersService.GetDocument(courseId);
                        Trackerentity.UpdateDated = DateTime.Now;
                        Trackerentity.SMEReviewUserId = (int)CurrentSession.UserId;
                        Trackerentity.SMEReviewDate = DateTime.Now;
                        Trackerentity.ImproveUserId = (int)CurrentSession.UserId;
                        Trackerentity.ImproveDate = DateTime.Now;
                        _ICourseTrackersService.SaveRecord(Trackerentity);
                    }
                    #endregion

                    #region Send notification to director and developer for remarks if SME reviewed all the docs
                    var sBuilder = new StringBuilder();

                    var courseDocsRemarksList = _ICourseDocRemarksService.GetListByCourseId(courseId).ToList();
                    foreach (var item in courseDocsRemarksList)
                    {
                        sBuilder.Append(
                            "<tr>" +
                                "<td>" + item.DocName + "</td>" +
                                "<td>" + item.Remarks + "</td>" +
                                "<td>" + item.Suggestion + "</td>" +
                                "<td>" + item.ReferenceFile + "</td>" +
                            "</tr>");
                    }

                    var configEntity = _IConfigService.GetFirstRecord();
                    if (configEntity != null)
                    {
                        var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(courseEntity.Id);
                        var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                        var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                        if (directorEntity != null)
                        {
                            var emailBody = UtilityHelper.GetEmailTemplate("RemarksSummaryNotificationToDirectorAndDeveloper.html").ToString();

                            emailBody = emailBody
                                 .Replace("@CourseStatusMessage@", "Kindly notice that the below course is under improvement and you can view remarks and / or suggestion in below table. Developer needs to attend following remarks and / or suggestion and re-submitting the updated documents.<br /><br />");

                            emailBody = emailBody
                                .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                                .Replace("@SMEName@", CurrentSession.UserName)
                                .Replace("@CourseName@", courseEntity.CourseName)
                                .Replace("@DocumentsRemarksSummary@", sBuilder.ToString());

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(directorEntity.Email) + ";" + UtilityHelper.Decrypt(developerEntity.Email),
                                Subject = "SSTM COURSE ASSIGNMENTS UPDATES (" + courseEntity.CourseName + ")",
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

                    return Json(new { result = true, message = "Course Documents are submitted successfully. Course status is updated to " + statusString + "." });
                }
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "SubmitCourseDocsAssessment", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ApproveCourse(long courseId, bool MasterCourse)
        {
            try
            {
                var courseEntity = _ICourseService.GetRecordById(courseId);
                if (courseEntity != null)
                {
                    #region Update status in course table
                    courseEntity.Statusid = _ICourseStatusService.GetRecordIdByName("Approved");
                    courseEntity.UpdatedBy = CurrentSession.UserId;
                    courseEntity.UpdatedOn = DateTime.Now;

                    _ICourseService.SaveRecord(courseEntity);
                    #endregion

                    #region Send notification to HR 
                    var configEntity = _IConfigService.GetFirstRecord();

                    var hrEmails = string.Empty;
                    var hrList = _IUserService.GetList(1).Where(a => a.Role == "HR");
                    hrList.ForEach(a => { hrEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                    var documents = "";
                    var courseDocumentsList = _ICourseDocumentService.GetListByCourseId(courseEntity.Id, MasterCourse).ToList();
                    courseDocumentsList.ForEach(a => { documents += a.DocName != "N/A" ? a.DocName + ", " : ""; });

                    var emailBody = UtilityHelper.GetEmailTemplate("ReleaseNotificationToHR.html").ToString();
                    emailBody = emailBody.Replace("@CourseName@", courseEntity.CourseName)
                        .Replace("@Documents@", documents.Trim().TrimEnd(','));

                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : hrEmails.Trim().TrimEnd(';'),
                        Subject = "SSTM COURSE DOCUMENTS APPROVED (" + courseEntity.CourseName + ")",
                        Message = emailBody,
                        SMTPHost = configEntity.Host,
                        SMTPPort = configEntity.Port,
                        SMTPEmail = configEntity.Email,
                        SMTPPassword = configEntity.Pass,
                        EnableSsl = configEntity.EnableSsl
                    });
                    #endregion

                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "No data found." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "ApproveCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ReleaseCourse(long courseId)
        {
            try
            {
                var courseEntity = _ICourseService.GetRecordById(courseId);
                if (courseEntity != null)
                {
                    #region Update status in course table
                    courseEntity.Statusid = _ICourseStatusService.GetRecordIdByName("Released");
                    courseEntity.UpdatedBy = CurrentSession.UserId;
                    courseEntity.UpdatedOn = DateTime.Now;

                    _ICourseService.SaveRecord(courseEntity);
                    #endregion

                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "No data found." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "ReleaseCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ShareCourse(CourseSharingModel[] paramsList)
        {
            try
            {
                long docid = 0;
                if (paramsList.Length > 0)
                {
                    #region Save records into Course sharing table for training and printing
                    foreach (var item in paramsList)
                    {
                        docid = item.CourseId;
                        var courseSharingEntity = _ICourseSharingService.GetRecordByCourseAndDocIds(item.CourseId, item.DocId);
                        if (courseSharingEntity != null)
                        {
                            courseSharingEntity.UpdatedBy = CurrentSession.UserId;
                            courseSharingEntity.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            courseSharingEntity = new CourseSharingModel().ToEntity();
                            courseSharingEntity.CourseId = item.CourseId;
                            courseSharingEntity.DocId = item.DocId;
                            courseSharingEntity.CreatedBy = CurrentSession.UserId;
                            courseSharingEntity.CreatedOn = DateTime.Now;
                        }

                        courseSharingEntity.isTraining = item.isTraining;
                        courseSharingEntity.isPrinting = item.isPrinting;
                        courseSharingEntity.isDeveloper = item.isDeveloper;

                        _ICourseSharingService.SaveRecord(courseSharingEntity);
                    }

                    #region Trackers 
                    var Trackerentity = _ICourseTrackersService.GetDocument(docid);
                    Trackerentity.UpdateDated = DateTime.Now;
                    Trackerentity.ReleseUserid = (int)CurrentSession.UserId;
                    Trackerentity.ReleaseDate = DateTime.Now;
                    _ICourseTrackersService.SaveRecord(Trackerentity);
                    #endregion

                    #endregion

                    var sharedCourseDocsList = _ICourseSharingService.GetListofSharedCourseDocs(paramsList[0].CourseId).ToList();

                    var configEntity = _IConfigService.GetFirstRecord();

                    #region Send notification to Trainer 
                    var courseDocumentsList = sharedCourseDocsList.Where(a => a.isTraining).ToList();
                    if (courseDocumentsList.Count() > 0)
                    {
                        var documents = "";
                        courseDocumentsList.ForEach(a => { documents += a.DocName != "N/A" ? a.DocName + ", " : ""; });

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToTrainer.html").ToString();
                        emailBody = emailBody.Replace("@CourseName@", sharedCourseDocsList[0].CourseName)
                            .Replace("@Documents@", documents.Trim().TrimEnd(','));

                        var trainersEmails = string.Empty;
                        var trainersList = _IUserService.GetList(1).Where(a => a.Role == "Trainer").ToList();
                        trainersList.ForEach(a => { trainersEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : trainersEmails.Trim().TrimEnd(';'),
                            Subject = "SSTM COURSE DOCUMENTS RELEASED (" + sharedCourseDocsList[0].CourseName + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                    }
                    #endregion

                    #region Send notification to Print Incharge
                    courseDocumentsList = sharedCourseDocsList.Where(a => a.isPrinting).ToList();
                    if (courseDocumentsList.Count() > 0)
                    {
                        var documents = "";
                        courseDocumentsList.ForEach(a => { documents += a.DocName != "N/A" ? a.DocName + ", " : ""; });

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToPrintIncharge.html").ToString();
                        emailBody = emailBody.Replace("@CourseName@", sharedCourseDocsList[0].CourseName)
                            .Replace("@Documents@", documents.Trim().TrimEnd(','));

                        var printInchargesEmails = string.Empty;
                        var printInchargesList = _IUserService.GetList(1).Where(a => a.Role == "Print Incharge").ToList();
                        printInchargesList.ForEach(a => { printInchargesEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : printInchargesEmails.Trim().TrimEnd(';'),
                            Subject = "SSTM COURSE DOCUMENTS RELEASED (" + sharedCourseDocsList[0].CourseName + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                    }
                    #endregion

                    #region Send notification to Developer 
                    var courseDocumentsDeveloperList = sharedCourseDocsList.Where(a => a.isDeveloper).ToList();
                    if (courseDocumentsDeveloperList.Count() > 0)
                    {
                        var documents = "";
                        courseDocumentsDeveloperList.ForEach(a => { documents += a.DocName != "N/A" ? a.DocName + ", " : ""; });

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToTrainer.html").ToString();
                        emailBody = emailBody.Replace("@CourseName@", sharedCourseDocsList[0].CourseName)
                            .Replace("@Documents@", documents.Trim().TrimEnd(','));

                        var developerEmails = string.Empty;
                        var trainersList = _IUserService.GetList(1).Where(a => a.Role == "Developer").ToList();
                        trainersList.ForEach(a => { developerEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : developerEmails.Trim().TrimEnd(';'),
                            Subject = "SSTM COURSE DOCUMENTS RELEASED (" + sharedCourseDocsList[0].CourseName + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                    }
                    #endregion
                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "No data found for sharing." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "ShareCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        public ActionResult SharedCoursesList(bool? MasterCourse, long? MasterCourseId, string Coursename)
        {
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


                if (CurrentSession.UserRole == "Trainer")
                    list = _ICourseSharingService.GetListOfSharedCourses(MasterCourse, MasterCourseId).Where(a => a.isTraining).ToList();
                else if (CurrentSession.UserRole == "Print Incharge")
                    list = _ICourseSharingService.GetListOfSharedCourses(MasterCourse, MasterCourseId).Where(a => a.isPrinting).ToList();
                else if (CurrentSession.UserRole == "Developer")
                    list = _ICourseSharingService.GetListOfSharedCourses(MasterCourse, MasterCourseId).Where(a => a.isDeveloper).ToList();


                return View("SharedCoursesList", list);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "SharedCoursesList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content(AppMessages.Exception);
            }
        }

        #region User Defined functions
        public void GetDocsStatusList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "All" });
            var statusList = _ICourseStatusService.GetList().ToList();
            if (/*CurrentSession.UserRole == "Developer" ||*/ CurrentSession.UserRole == "Staff")
            {
                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Pending").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Pending").FirstOrDefault().Status.ToString()
                });
                //if (CurrentSession.UserRole == "Developer")
                //{
                //    list.Add(new SelectListItem
                //    {
                //        Value = statusList.Where(a => a.Status == "Submitted").FirstOrDefault().Id.ToString(),
                //        Text = statusList.Where(a => a.Status == "Submitted").FirstOrDefault().Status.ToString()
                //    });
                //}
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

                //list.Add(new SelectListItem
                //{
                //    Value = statusList.Where(a => a.Status == "Reviewed").FirstOrDefault().Id.ToString(),
                //    Text = statusList.Where(a => a.Status == "Reviewed").FirstOrDefault().Status.ToString()
                //});
                //list.Add(new SelectListItem
                //{
                //    Value = statusList.Where(a => a.Status == "Approved").FirstOrDefault().Id.ToString(),
                //    Text = statusList.Where(a => a.Status == "Approved").FirstOrDefault().Status.ToString()
                //});
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

        public void Get_CourseAndSubCourse(string CourseType, bool MasterCourse, long MasterCoursId)
        {
            var list = new List<SelectListItem>();

            var statusList = _ICourseService.Get_CourseAndSubCourse(CourseType, MasterCourse, MasterCoursId, 0).ToList();

            statusList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.CourseName });
            });
            ViewBag.MainCourseList = list;

        }

        public JsonResult Get_CourseAndSubCourse_json(string CourseType, bool MasterCourse, long MasterCoursId)
        {
            var list = new List<SelectListItem>();

            var statusList = _ICourseService.Get_CourseAndSubCourse(CourseType, MasterCourse, MasterCoursId, 0).ToList();

            return Json(new { result = true, content = statusList }, JsonRequestBehavior.AllowGet);
        }

        public string GetSubCouseList(string CourseType, long MasterCourseId, long selectedSubCourse)
        {
            var SubCourseList = _ICourseService.Get_CourseAndSubCourse(CourseType, false, MasterCourseId, selectedSubCourse).ToList();
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append("<option value = '' >-- Select Sub Course --</ option >");
            SubCourseList.ForEach(a =>
            {
                sBuilder.Append("<option value = " + a.Id.ToString() + " >" + a.CourseName + "</ option >");
            });
            return sBuilder.ToString();
        }




        public void GetStaffList(int Userid)
        {
            var list = new List<SelectListItem>();

            var statusList = _IUserService.GetRecordByStaff(Userid).ToList();

            statusList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.FirstName });
            });
            //TempData["StaffList"] = new SelectList(list, "Value", "Text");
            ViewBag.StaffList = list;
        }

        public void GetHRList(int Userid)
        {
            var list = new List<SelectListItem>();

            var statusList = _IUserService.GetRecordByStaff(Userid).ToList();
            list.Add(new SelectListItem { Value = "0", Text = "-- Select HR --" });
            statusList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.FirstName });
            });
            TempData["HrList"] = new SelectList(list, "Value", "Text");
            // ViewBag.HrList = list;
        }
        public void GetSMEList()
        {
            var list = _IUserService.GetList(1).Where(a => a.Role.Trim() == "SME").ToList();

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = "0", Text = "Select" });

            list.ForEach(a => { selectList.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.FirstName + " " + a.LastName }); });

            TempData["SMEList"] = new SelectList(selectList, "Value", "Text");
        }


        public void GetDownloadUserList()
        {
            var list = _IUserService.GetList(1).Where(a => a.Role.Trim() == "DownloadLogin").ToList();

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = "0", Text = "Select" });

            list.ForEach(a => { selectList.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.FirstName + " " + a.LastName }); });

            TempData["DownloadLoginList"] = new SelectList(selectList, "Value", "Text");
        }
        public JsonResult AssignSME(long courseId)
        {
            var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId_list().Where(a => a.CourseId == courseId).FirstOrDefault();
            return Json(courseAssignmentEntity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SMESKIPCourse(long courseId, bool MasterCourse)
        {
            // var statusString = isReassignCount > 0 ? "Under Improvement" : "Released";//new
            try
            {
                #region Skip all the review Steps 
                var courseEntity = _ICourseService.GetRecordById(courseId);
                courseEntity.Statusid = _ICourseStatusService.GetRecordIdByName("Released");
                courseEntity.UpdatedBy = CurrentSession.UserId;
                courseEntity.UpdatedOn = DateTime.Now;

                var result = _ICourseService.SaveRecord(courseEntity);
                IEnumerable<CourseDocumentsListModel> listofDoc = _ICourseDocumentService.GetListByCourseId(courseId, MasterCourse).ToList();
                foreach (var data in listofDoc)
                {
                    var courseDocEntity = _ICourseDocumentService.GetRecordById(data.Id);
                    courseDocEntity.isCompleted = false;
                    courseDocEntity.isReassigned = false;
                    courseDocEntity.isApproved = true;
                    courseDocEntity.UpdatedBy = CurrentSession.UserId;
                    courseDocEntity.UpdatedOn = DateTime.Now;
                    var newId = _ICourseDocumentService.SaveRecord(courseDocEntity);
                }

                #region Trackers 
                CourseTrackers Trackerentity = _ICourseTrackersService.GetDocument(courseId);

                if (Trackerentity == null)
                {
                    CourseTrackers data = new CourseTrackers();
                    data.Courseid = courseId;
                    _ICourseTrackersService.SaveRecord(data);
                    Trackerentity = _ICourseTrackersService.GetDocument(courseId);
                }

                Trackerentity.UpdateDated = DateTime.Now;
                //Trackerentity.SMEAcceptUserId = (int)CurrentSession.UserId;
                //Trackerentity.SMEAcceptDate = DateTime.Now;
                if (string.IsNullOrEmpty(Trackerentity.submitedUserId.ToString()))
                {
                    Trackerentity.submitedUserId = (int)CurrentSession.UserId;
                    Trackerentity.submitedDate = DateTime.Now;
                }
                if (string.IsNullOrEmpty(Trackerentity.SMEAssignUserId.ToString()))
                {
                    Trackerentity.SMEAssignUserId = (int)CurrentSession.UserId;
                    Trackerentity.AssignDate = DateTime.Now;
                }
                if (string.IsNullOrEmpty(Trackerentity.SMEReviewUserId.ToString()))
                {
                    Trackerentity.SMEReviewUserId = (int)CurrentSession.UserId;
                    Trackerentity.SMEReviewDate = DateTime.Now;
                }
                if (string.IsNullOrEmpty(Trackerentity.ImproveUserId.ToString()))
                {
                    Trackerentity.ImproveUserId = (int)CurrentSession.UserId;
                    Trackerentity.ImproveDate = DateTime.Now;
                }
                if (string.IsNullOrEmpty(Trackerentity.SMEAcceptUserId.ToString()))
                {
                    Trackerentity.SMEAcceptUserId = (int)CurrentSession.UserId;
                    Trackerentity.SMEAcceptDate = DateTime.Now;
                }
                _ICourseTrackersService.SaveRecord(Trackerentity);
                #endregion

                return Json(true, JsonRequestBehavior.AllowGet);
                #endregion
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public ActionResult CourseStatus(long Courseid, bool MasterCourse, long MasterCourseId, string courseName)
        {

            CourseTrackingDataModel data = _ICourseTrackersService.GetCoursesTrackWithData(Courseid);
            if (data == null)
            {
                CourseTrackers data1 = new CourseTrackers();
                data1.Courseid = Courseid;
                ViewBag.MasterCourse = MasterCourse;
                ViewBag.MasterCourseId = MasterCourseId;
                ViewBag.courseName = courseName;
                ViewBag.oldDocnotrack = "Old Document So, can't track";
                _ICourseTrackersService.SaveRecord(data1);
                CourseTrackingDataModel datanew = _ICourseTrackersService.GetCoursesTrackWithData(Courseid);
                return View(datanew);
            }
            ViewBag.MasterCourse = MasterCourse.ToString();
            ViewBag.MasterCourseId = MasterCourseId;
            ViewBag.oldDocnotrack = "Old Document So, can't track";
            ViewBag.courseName = courseName;
            return View(data);
        }

        public JsonResult CourseMail(string Courseid, string MasterCourse, string MasterCourseId, string courseName, string stage)
        {
            CourseTrackingDataModel data = _ICourseTrackersService.GetCoursesTrackWithData(Convert.ToInt64(Courseid));
            string message = "";
            if (data == null)
            {
                message = "First Submit Document after start course tracking...!";
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            if (stage == "SMEAssign" || stage == "SMEReview")
            {
                if (stage == "SMEAssign")
                {
                    if (data.SMEAssignUser != " ")
                    {
                        message = "SME Assign Step Already Done ...!";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (data.SMEReviewUser != " ")
                    {
                        message = "SME Review Step Already Done ...!";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                }

                SmeAssign(Convert.ToInt64(Courseid), Convert.ToBoolean(MasterCourse), Convert.ToInt64(MasterCourseId), courseName, stage);
                message = stage == "SMEAssign" ? "Successfully sent mail Director to Assign SME" : "Successfully sent mail Director to SME Reviewed";
            }
            if (stage == "Improvement")
            {

                if (data.ImproveUser != " " && data.ImproveDate.ToString() != "")
                {
                    message = "Improve Step Already Done ...!";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                if (data.ImproveUser == " ")
                {
                    UnderImprovemtnMail(Convert.ToInt64(Courseid), Convert.ToBoolean(MasterCourse), Convert.ToInt64(MasterCourseId), courseName, stage);
                    message = "Successfully Sent Mail Developer for Improvement";
                }
                else
                {
                    message = "First Assign SME After Improvement Courses Sent Mail Notification !";
                }
            }
            if (stage == "SMEAccept")
            {
                if (data.SMEAcceptUser != " " && data.SMEAcceptUser.ToString() != "")
                {
                    message = "SME Accept Step Already Done ...!";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                if (data.SMEAcceptUser == " ")
                {
                    SMEAcceptMail(Convert.ToInt64(Courseid), Convert.ToBoolean(MasterCourse), Convert.ToInt64(MasterCourseId), courseName, stage);
                    message = "Successfully Sent Mail to SME and Director";
                }
                else
                {
                    message = "SME Not Assign First SME Assgin After Sent Mail Notification";
                }
            }
            if (stage == "Release")
            {
                if (data.ImproveUser == " ")
                {
                    message = "SME Accept Pending ...!";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }

                if (data.ReleseUser != " " && data.ReleaseDate.ToString() != "")
                {
                    message = "Release Step Already Done ...!";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                if (data.ReleseUser == " ")
                {
                    Releasemail(Convert.ToInt64(Courseid), Convert.ToBoolean(MasterCourse), Convert.ToInt64(MasterCourseId), courseName, stage);
                    message = "Successfully Sent Mail Directore.";
                }
                else
                {
                    message = "SME Not Accept Course Sent Mail Notification";
                }
            }
            if (stage == "Done")
            {
                if (data.ReleseUser == " " && data.ReleaseDate.ToString() == "")
                {
                    message = "Relese Step Pending  ...!";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                DoneRelese(Convert.ToInt64(Courseid), Convert.ToBoolean(MasterCourse), Convert.ToInt64(MasterCourseId), courseName, stage);
                message = "Successfully Release Course Sent to Directore Mail Notification .";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }
        #region SmeAssign
        public void SmeAssign(long Courseid, bool MasterCourse, long MasterCourseId, string courseName, string stage)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            if (configEntity != null)
            {
                CourseTrackingDataModel data = _ICourseTrackersService.GetCoursesTrackWithData(Courseid);

                var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(Courseid);
                var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                if (directorEntity != null)
                {
                    var emailBody = UtilityHelper.GetEmailTemplate("NotificationAssignSMETracking.html").ToString();
                    if (stage == "SMEAssign")
                    {
                        emailBody = emailBody
                            .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                            .Replace("@CourseStatusMessage@", "SME Not Assign This Course : <b>" + courseName + "</b>")
                            .Replace("@DocumentCurrentStage@", "Not Assign SME to this Course SME")
                            .Replace("@CourseName@", courseName);
                    }
                    else
                    {
                        emailBody = emailBody
                           .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                           .Replace("@CourseStatusMessage@", "SME Review This Course : <b>" + courseName + "</b>")
                           .Replace("@DocumentCurrentStage@", "SME Review this Course.")
                           .Replace("@CourseName@", courseName);
                    }

                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(directorEntity.Email),
                        Subject = stage == "SMEAssign" ? "SME Not Assign : (" + courseName + ")" : "SME Review : (" + courseName + ")",
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

        #region Improvement Mail
        public void UnderImprovemtnMail(long Courseid, bool MasterCourse, long MasterCourseId, string courseName, string stage)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            if (configEntity != null)
            {
                CourseTrackingDataModel data = _ICourseTrackersService.GetCoursesTrackWithData(Courseid);

                var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(Courseid);
                var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                if (directorEntity != null)
                {
                    var emailBody = UtilityHelper.GetEmailTemplate("NotificationAssignSMETracking.html").ToString();

                    emailBody = emailBody
                        .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                        .Replace("@CourseStatusMessage@", "Course Improvement Delay : <b>" + courseName + "</b>")
                        .Replace("@DocumentCurrentStage@", "Course Improvement Take A Time. Submit Course to SME for Review.")
                        .Replace("@CourseName@", courseName);
                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(developerEntity.Email),
                        Subject = "Course Improvement Delay : (" + courseName + ")",
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

        #region SME Accept Mail (sent mail SME,Director)
        public void SMEAcceptMail(long Courseid, bool MasterCourse, long MasterCourseId, string courseName, string stage)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            if (configEntity != null)
            {
                CourseTrackers data = _ICourseTrackersService.GetDocument(Courseid);

                var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(Courseid);
                var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);
                User smeuse = new User();
                if (data.SMEReviewUserId != 0)
                {
                    smeuse = _IUserService.GetRecordById((int)data.SMEReviewUserId);
                }

                if (directorEntity != null)
                {
                    var emailBody = UtilityHelper.GetEmailTemplate("NotificationAssignSMETracking.html").ToString();

                    emailBody = emailBody
                        .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                        .Replace("@CourseStatusMessage@", "SME Accept Course : <b>" + courseName + "</b> SME is : " + smeuse.FirstName + " " + smeuse.LastName)
                        .Replace("@DocumentCurrentStage@", "SME Not Accept Course Please Check And Review Course Document after Approve Accept Course.")
                        .Replace("@CourseName@", courseName);

                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : smeuse.Email != "" ? UtilityHelper.Decrypt(smeuse.Email) + ";" : "" + UtilityHelper.Decrypt(directorEntity.Email),
                        Subject = "SME Accept Course Delay : (" + courseName + ")",
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

        #region Release Mail (sent mail Director)
        public void Releasemail(long Courseid, bool MasterCourse, long MasterCourseId, string courseName, string stage)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            if (configEntity != null)
            {
                CourseTrackers data = _ICourseTrackersService.GetDocument(Courseid);

                var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(Courseid);
                var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                if (directorEntity != null)
                {
                    var emailBody = UtilityHelper.GetEmailTemplate("NotificationAssignSMETracking.html").ToString();

                    emailBody = emailBody
                        .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                        .Replace("@CourseStatusMessage@", "Release Course : <b>" + courseName + "</b>")
                        .Replace("@DocumentCurrentStage@", "Pending Course Release after share and printing document.")
                        .Replace("@CourseName@", courseName);

                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(directorEntity.Email),
                        Subject = "Pending Release Course : (" + courseName + ")",
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

        #region After Printing and shre course Mail (sent mail Director)
        public void DoneRelese(long Courseid, bool MasterCourse, long MasterCourseId, string courseName, string stage)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            if (configEntity != null)
            {
                CourseTrackers data = _ICourseTrackersService.GetDocument(Courseid);

                var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(Courseid);
                var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                if (directorEntity != null)
                {
                    var emailBody = UtilityHelper.GetEmailTemplate("NotificationAssignSMETracking.html").ToString();

                    emailBody = emailBody
                        .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                        .Replace("@CourseStatusMessage@", "Course Release : <b>" + courseName + "</b>")
                        .Replace("@DocumentCurrentStage@", "Course is Release ans Share Course Successfully.")
                        .Replace("@CourseName@", courseName);

                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(directorEntity.Email),
                        Subject = "Successfully Release Course : (" + courseName + ")",
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

        #region Existing Course Reminder 
        [HttpPost]
        public ActionResult GetReminderCourseForm()
        {
            try
            {
                //var model = _ICourseService.GetRecordById(Convert.ToInt64(Id));
                var data = _ICourseService.GetAllRecord().ToList();
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem { Value ="", Text ="-- Select Course --" });
                data.ForEach(a =>
                {
                    list.Add(new SelectListItem { Value = Convert.ToString(a.Id), Text = a.CourseName });
                });
                TempData["courseName"] = new SelectList(list, "Value", "Text");
               
                ViewBag.rolename = "Developer";

                return PartialView("_RenewalReminderForm", data);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseController", "GetReminderCourseForm", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }
        #endregion

        #region Save Reminder Course
        [HttpPost]
        public ActionResult SaveReminderCourse(string courseid, string renewremiderday, string courseremindertext, long developerid,DateTime reminderdate)
        {
            try
            {
                #region Renew Course And Update Course Remainder Details

                var entity = _ICourseService.GetRecordById(Convert.ToInt64(courseid));
                entity.DeveloperId = developerid;
                entity.renewal_reminder = courseremindertext;
                entity.reminder_days = Convert.ToInt32(renewremiderday);
                entity.renew_date = reminderdate.AddDays(Convert.ToInt32(Convert.ToInt32(renewremiderday)));
                entity.reminder_created_date = reminderdate;
                var courseId = _ICourseService.SaveRecord(entity);

                #region Email to developer to fill up course proposal
                var configEntity = _IConfigService.GetFirstRecord();
                if (entity.Id != 0)
                {
                    if (configEntity != null)
                    {
                        var developerEntity = _IUserService.GetDefaultList().Where(a => a.Id == entity.DeveloperId || a.RoleId == 4).ToList();

                        foreach (var item in developerEntity)
                        {
                            //string path = "";
                            //if (!string.IsNullOrEmpty(entity.doc_file))
                            //    path = UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/" + entity.doc_file;

                            string url = "https://li.eversafe.com.sg";

                            string coursereviewurl = "https://li.eversafe.com.sg/FRM-41CourseReviewForm.aspx?staffid={0}"; //refer course review form

                            string dcrurl = "https://li.eversafe.com.sg/DeveloperDCR1.aspx?staffid={0}"; //refer document change request form

                            var trainerId = 0;

                            if (item.Trainer_AirLine_id == null) trainerId = 0; else trainerId = (int)item.Trainer_AirLine_id;

                            var emailBody = UtilityHelper.GetEmailTemplate("NewCourseRemindermail.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                                .Replace("@CourseName@", entity.CourseName)
                                .Replace("@courseproposal@", trainerId > 0 ? string.Format(coursereviewurl, item.Trainer_AirLine_id) : url)
                                .Replace("@needanalysis@", trainerId > 0 ? string.Format(dcrurl, item.Trainer_AirLine_id) : url);

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(item.Email),
                                //To = Request.IsLocal ? "developer2@eversafe.com.sg" : UtilityHelper.Decrypt(item.Email),
                                Subject = "SSTM Reminder mail (" + entity.CourseName + ")",
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

                #endregion

                return Json(new { result = true, message = "Successfully Set Course Reminder." });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseReminder", "SaveNewCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        #endregion

        #region get Course Reminder data
        [HttpPost]
        public ActionResult getReminderCourse(string courseid)
        {
            try
            {
                #region Renew Course And Update Course Remainder Details

                var entity = _ICourseService.GetRecordById(Convert.ToInt64(courseid));
                string rolename = "";
                if (!string.IsNullOrEmpty(entity.DeveloperId.ToString()))
                {
                    var DeveloeperEntity = _IUserService.GetDefaultList().Where(a => a.Id == entity.DeveloperId).FirstOrDefault();
                    var role = _IRoleService.GetRecordById(DeveloeperEntity.RoleId);
                    ViewBag.role = role.Id;
                    rolename = role.RoleName;
                }
                else
                {
                    rolename = "Developer";
                }
                #endregion

                return Json(new { result = true,entity= entity,rolename= rolename });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseReminder", "SaveNewCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        #endregion

    }
}
