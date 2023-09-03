using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Models.QPRequest;
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
    public class QPRequestController : BaseController
    {
        #region Class Properties Declarations
        private static IQPRequestService _IQPRequestService;
        private readonly IRoleService _IRoleService;
        private readonly IUserService _IUserService;
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }

        public QPRequestController(IQPRequestService IQPRequestService, IRoleService IRoleService, IUserService IUserService)
        {
            _IQPRequestService = IQPRequestService;
            _IRoleService = IRoleService;
            _IUserService = IUserService;
        }
        #endregion

        // GET: QPRequest
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QPRequestList()
        {
            try
            {
                var sBuilder = new StringBuilder();
                var data = _IQPRequestService.GetAllRecord();
                foreach (var item in data)
                {
                    var actions =
                           //"<button type='button' title='Edit' class='btn btn-primary btn-sm btnEditCourse'><i class='fa fa-pen'></i></button>&nbsp;" +
                           "<button type='button' title='Delete' class='btn btn-danger btn-sm btnDeleteCourse'><i class='fa fa-trash'></i></button>";

                    sBuilder.Append(
                            "<tr id='" + item.Id + "'>" +
                                "<td>" + item.CourseName + "</td>" +
                                "<td>" + item.CourseCode + "</td>" +
                                "<td>" + item.EnterFooter + "</td>" +
                                "<td>" + item.DeveloperName + "</td>" +
                                "<td>" + actions + "</td>");
                }
                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Error " + ex.Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveQPRequest(QPRequestModel model)
        {
            try
            {
                var entity = _IQPRequestService.GetRecordById(model.Id);
                if (entity != null)
                {
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                }
                else
                {
                    entity = new QPRequestModel().ToEntity();
                    entity.CreatedBy = CurrentSession.UserId;
                    entity.CreatedOn = DateTime.Now;
                }
                entity.CourseName = model.CourseName;
                entity.CourseCode = model.CourseCode;
                entity.EnterFooter = model.EnterFooter;
                entity.DeveloperName = model.DeveloperName;
                _IQPRequestService.SaveRecord(entity);
                return View("index");
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                return View("index");
            }
        }

        public ActionResult GetQPRequestById(long Id)
        {
            QPRequestModel model=null;
            try
            {
                 model = _IQPRequestService.GetRecordById(Id).ToModel();
                if (model == null)
                {
                    model = new QPRequestModel();
                }
                #region Course Name
                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "", Text = "--- Select Course Name  ----" });

                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.AllCourse();

                    List<Li_CourseModel> Course_Li_Reminder = (new JavaScriptSerializer()).Deserialize<List<Li_CourseModel>>(data);
                    Course_Li_Reminder.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.CourseName), Text = a.CourseName });
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
                #region Course Code
                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "", Text = "--- Select Course Code  ----" });

                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string data = service.AllCourse();

                    List<Li_CourseModel> Course_Li_Reminder = (new JavaScriptSerializer()).Deserialize<List<Li_CourseModel>>(data);
                    Course_Li_Reminder.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.CourseShortname), Text = a.CourseShortname });
                    });
                    TempData["CourseCode"] = new SelectList(list, "Value", "Text");
                }
                catch (Exception ex)
                {
                    string e = ex.Message;
                    var list = new List<SelectListItem>();
                    TempData["CourseCode"] = new SelectList(list, "Value", "Text");
                }
                #endregion
                var list1 = new List<SelectListItem>();
                list1.Add(new SelectListItem { Value = "", Text = "--- Select Developer  ----" });
                var roleId = _IRoleService.GetRecordIdByName("Developer");
                var DeveloeperEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == roleId).ToList();
                DeveloeperEntity.ForEach(a =>
                {
                    list1.Add(new SelectListItem { Value = Convert.ToString(a.FirstName), Text = a.FirstName });
                });
                TempData["Developer"] = new SelectList(list1, "Value", "Text");
                return PartialView("_AddOrUpdate", model);
            }
            catch (System.Exception ex)
            {
                string e = ex.Message;
            }
            return PartialView("_AddOrUpdate", model);
        }

        [HttpPost]
        public ActionResult DeleteQPRequest(long Id)
        {
            try
            {
                _IQPRequestService.DeleteRecord(Id);
                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                return Json(new { result = false });
            }
        }
    }
}

public class Li_CourseModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseShortname { get; set; }
}
