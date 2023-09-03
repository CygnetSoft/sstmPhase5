using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Models.CourseDocVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class CourseDocVersionController : BaseController
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;

        private readonly ICourseService _ICourseService;

        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ICourseDocVersionService _ICourseDocVersionService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public CourseDocVersionController(IExceptionLogService exceptionLogService, ICourseService courseService,
            ICourseDocumentService courseDocumentService, ICourseDocVersionService courseDocVersionService)
        {
            _IExceptionLogService = exceptionLogService;

            _ICourseService = courseService;

            _ICourseDocumentService = courseDocumentService;
            _ICourseDocVersionService = courseDocVersionService;
        }
        #endregion

        public ActionResult Index()
        {
            GetCoursesList();

            return View();
        }

        [HttpPost]
        public ActionResult GetCourseDocuments(long courseId)
        {
            try
            {
                var list = _ICourseDocumentService.GetListVersionByCourseId(courseId);

                return Json(new { result = true, list = list });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDocVersion", "GetListVersionByCourseId", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = "Exception: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetCourseDocVersions(long docId)
        {
            try
            {
                var sBuilder = new StringBuilder();

                var list = _ICourseDocVersionService.GetListByDocId(docId);

                foreach (var item in list)
                {
                    var isActiveChecked = item.isActive ? "checked" : "";
                    string varsion = item.Version != "" && string.IsNullOrWhiteSpace(item.revision)? item.Version :  item.revision;
                    string date = item.VersionDate != "" && string.IsNullOrWhiteSpace(item.revisionDate) ? item.VersionDate  : item.revisionDate;
                    sBuilder.Append(
                        "<tr id='" + item.DocId + "' course='" + item.CourseId + "'>" +
                            "<td>" + item.CourseName + "</td>" +
                            "<td>" + item.DocName + "</td>" +
                            "<td>" + varsion +"</td>" +
                            "<td>" + date + "</td>" +
                             "<td> " + item.revision + "</td>" +
                            "<td > " + item.revisionDate + "</td>" +
                             "<td>" + item.createdby_User + "</td>" +
                              "<td>" + item.Updateby_User + "</td>" +
                            "<td>" +
                                "<a href='javascript:void(0);' onclick='ViewCourseDoc(this,\"" + "Course" + "\");'>" +
                                    "<strong>" + item.FileName + "</strong>" +
                                "</a>" +
                            "</td>" +
                             "<td>" +
                                "<a href ='/CourseDoc/DownoadVersionDocFile?filename=" + item.FileName + "&CourseId=" + item.CourseId + "' class='btn btn-info btn-sm  ml-1 Download'>Download</a>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<div class='icheck-success d-inline'>" +
                                    "<input type='radio' id='rdbActiveVersion" + item.Id.ToString() + "' name='radioActiveVersion' " +
                                        "class='rdbActiveVersion' " + isActiveChecked + " value='" + item.Id.ToString() + "' />" +
                                    "<label for='rdbActiveVersion" + item.Id.ToString() + "'></label>" +
                                "</div>" +
                            "</td>" +
                             "<td>" +
                                "<a  class='btn btn-success btn-sm  ml-1 Download' onclick='EditVersion("+ item.Id+ ")'>Edit Revision</a>" +
                            "</td>" +

                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDocVersion", "GetCourseDocVersions", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = "Exception: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateDocVersion(long docVersionId, bool isActive)
        {
            try
            {
                _ICourseDocVersionService.UpdateDocVersionStatus(docVersionId, isActive, CurrentSession.UserId);

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDocVersion", "UpdateDocVersion", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = "Exception: " + ex.Message });
            }
        }

        #region User Defined functions
        public void GetCoursesList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "Select Course" });

            var reviewdCoursesList = _ICourseService.GetCoursesWithotStatus(1, 5).ToList().Where(d => d.CourseType == "other" &&  d.MasterCourse==true && d.MasterCoursId==0).ToList();//show all course in dropdown.

            reviewdCoursesList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.CourseName });
            });
            //var reviewdCoursesList = _ICourseService.GetCoursesWithStatus(1, 5).ToList().Where(d => d.CourseType == "other").ToList();

            //reviewdCoursesList.ForEach(a =>
            //{
            //    list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.CourseName });
            //});

            //var approvedCoursesList = _ICourseService.GetCoursesWithStatus(1, 6).ToList().Where(d => d.CourseType == "other").ToList();

            //approvedCoursesList.ForEach(a =>
            //{
            //    list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.CourseName });
            //});

            //var releasedCoursesList = _ICourseService.GetCoursesWithStatus(1, 7).ToList().Where(d=>d.CourseType=="other").ToList();
            //releasedCoursesList.ForEach(a =>
            //{
            //    list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.CourseName });
            //});

            //var StaffCourseList = _ICourseService.GetStaffList(1, 7, 0).ToList().OrderByDescending(a=>a.Id);
            //foreach (var Courselist in StaffCourseList)
            //{
            //    list.Add(new SelectListItem { Value = Courselist.Id.ToString(), Text = Courselist.CourseName });
            //}

            list.GroupBy(p => p.Value);

            TempData["CoursesList"] = new SelectList(list.OrderBy(o => o.Value), "Value", "Text");
        }
        #endregion

        public ActionResult DocumentRecentlyUpdated()
        {
            return View();
        }
        public ActionResult GetRecentDocumentList(int days)
        {
            try
            {
                var sBuilder = new StringBuilder();

                var list = _ICourseDocVersionService.GetRecentDocumentList(days);

                foreach (var item in list)
                {
                    var isActiveChecked = item.isActive ? "checked" : "";

                    sBuilder.Append(
                        "<tr id='" + item.DocId + "' course='" + item.CourseId + "'>" +
                            "<td>" + item.CourseName + "</td>" +
                            "<td>" + item.DocName + "</td>" +
                             "<td>" + item.createdby_User + "</td>" +
                            "<td>" +
                                "<a href='javascript:void(0);' onclick='ViewCourseDoc(this,\"" + "Course" + "\");'>" +
                                    "<strong>" + item.FileName + "</strong>" +
                                "</a>" +
                            "</td>" +
                            //"<td class='text-center'>" +
                            //    "<div class='icheck-success d-inline'>" +
                            //        "<input type='radio' id='rdbActiveVersion" + item.Id.ToString() + "' name='radioActiveVersion' " +
                            //            "class='rdbActiveVersion' " + isActiveChecked + " value='" + item.Id.ToString() + "' />" +
                            //        "<label for='rdbActiveVersion" + item.Id.ToString() + "'></label>" +
                            //    "</div>" +
                            //"</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDocVersion", "GetRecentDocumentList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = "Exception: " + ex.Message });
            }
        }
        public ActionResult Updateversion(long id, string data)
        {
            CourseDocVersionModel model = new CourseDocVersionModel();
            model = new JavaScriptSerializer().Deserialize<CourseDocVersionModel>(data);
            var courseDocVersionEntity = _ICourseDocVersionService.GetRecordById(id);
            if (courseDocVersionEntity != null)
            {   
                courseDocVersionEntity.Version = model.Version;
                courseDocVersionEntity.VersionDate =Convert.ToDateTime(model.VersionDate);
                courseDocVersionEntity.revision = model.revision;
                if (!string.IsNullOrEmpty(model.revisionDate.ToString()))
                {
                    courseDocVersionEntity.revisionDate = Convert.ToDateTime(model.revisionDate);
                }
                _ICourseDocVersionService.SaveRecord(courseDocVersionEntity);
            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }
    }
}