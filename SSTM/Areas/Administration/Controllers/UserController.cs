using SSTM.Business.Interfaces;
using SSTM.Controllers;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SSTM.Areas.Administration.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class UserController : BaseController
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IUserService _IUserService;
        private readonly ITrainingCenterService _ITrainingCenterService;
        private readonly IRoleService _IRoleService;
        private readonly IConfigService _IConfigService;
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public UserController(IExceptionLogService exceptionLogService,
            IUserService userService, ITrainingCenterService trainingCenterService,
            IRoleService roleService, IConfigService configService)
        {
            _IExceptionLogService = exceptionLogService;
            _IUserService = userService;
            _ITrainingCenterService = trainingCenterService;
            _IRoleService = roleService;
            _IConfigService = configService;
        }
        #endregion

        public ActionResult Index()
        {
            return View(CurrentSession);
        }

        [HttpGet]
        public ActionResult GetUsersList(int isActive)
        {
            try
            {
                var sBuilder = new StringBuilder();

                var list = _IUserService.GetList(isActive).ToList();

                foreach (var item in list)
                {
                    var status = item.isActive ?
                        "<label class='badge badge-success'>Active</label>" : "<label class='badge badge-warning'>Inactive</label>";

                    var actions =
                        "<button type='button' title='Edit' class='btn btn-primary btn-sm btnEditUser'><i class='fa fa-pen'></i></button>&nbsp;" +
                        "<button type='button' title='Delete' class='btn btn-danger btn-sm btnDeleteUser'><i class='fa fa-trash'></i></button>";

                    var name = item.LastName != null ? item.FirstName + " " + item.LastName : item.FirstName;

                    sBuilder.Append(
                        "<tr id='" + item.Id + "'>" +
                            "<td>" + name + "</td>" +
                            "<td>" + item.Role + "</td>" +
                            "<td>" + UtilityHelper.Decrypt(item.Email) + "</td>" +
                            "<td>" + UtilityHelper.Decrypt(item.Mobile) + "</td>" +
                            "<td class='text-center'>" + status + "</td>" +
                            "<td class='text-center'>" + actions + "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "User", "GetUsersList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OuterUserList()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetOuterUsersList(int isActive)
        {
            try
            {
                var sBuilder = new StringBuilder();

                var list = _IUserService.GetOuterRegisterUserList(Convert.ToBoolean(isActive)).ToList();

                foreach (var item in list)
                {
                    var status = item.isActive ?
                        "<label class='badge badge-success'>Active</label>" : "<label class='badge badge-warning'>Inactive</label>";

                    var actions = item.isActive ?
                        "<a title='Click to Inactive' class='btn btn-danger btn-sm btnInactive'>Click to Inactive</a>&nbsp;"
                        : "<a  title='Click to Active' class='btn btn-primary btn-sm btnActive'>Click to Active</a>";


                    //var actions =
                    //    "<button type='button' title='Edit' class='btn btn-primary btn-sm btnEditUser'><i class='fa fa-pen'></i></button>&nbsp;" +
                    //    "<button type='button' title='Delete' class='btn btn-danger btn-sm btnDeleteUser'><i class='fa fa-trash'></i></button>";

                    var name = item.LastName != null ? item.FirstName + " " + item.LastName : item.FirstName;

                    sBuilder.Append(
                        "<tr id='" + item.Id + "'>" +
                            "<td>" + name + "</td>" +
                            "<td>Trainer</td>" +
                            "<td>" + UtilityHelper.Decrypt(item.Email) + "</td>" +
                            "<td>" + UtilityHelper.Decrypt(item.Password) + "</td>" +
                            "<td>" + UtilityHelper.Decrypt(item.Mobile) + "</td>" +
                            "<td class='text-center'>" + status + "</td>" +
                            "<td class='text-center'>" + actions + "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "User", "GetOuterUsersList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetUserById(long Id)
        {
            try
            {
                GetTrainingCentersList();
                GetUsersRolesList();

                var model = _IUserService.GetRecordById(Id).ToModel();

                if (model == null)
                {
                    model = new UserModel();
                    model.isActive = true;
                }
                else
                {
                    model.Mobile = UtilityHelper.Decrypt(model.Mobile);
                    model.Email = UtilityHelper.Decrypt(model.Email);
                    model.Password = UtilityHelper.Decrypt(model.Password);
                    model.MacAddress = UtilityHelper.Decrypt(model.MacAddress);
                    model.MacAddress1 = UtilityHelper.Decrypt(model.MacAddress1);

                }

                return PartialView("_AddOrUpdate", model);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "User", "GetUserById", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }


        [HttpPost]
        public ActionResult ChangeUserStatus(string userid, string status)
        {
            try
            {
                var entity = _IUserService.GetOuterLoginRecordById(Convert.ToInt64(userid)).ToModel();

                if (entity != null)
                {
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                }
                entity.isActive = Convert.ToBoolean(status);
                _IUserService.SaveRecord(entity.ToEntity());
                try
                {
                    var configEntity = _IConfigService.GetFirstRecord();
                    var emailBody = UtilityHelper.GetEmailTemplate("OuterTrainerActivetedStatus.html").ToString();
                    emailBody = emailBody.Replace("@@email@@", entity.FirstName + " " + entity.LastName)
                        .Replace("@@status@@", status=="true"? "Activated":"Deactivated")
                        .Replace("@date@", DateTime.Now.ToString());

                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(entity.Email),
                        Subject = "YOUR ACCOUNT STATUS",
                        Message = emailBody,
                        SMTPHost = configEntity.Host,
                        SMTPPort = configEntity.Port,
                        SMTPEmail = configEntity.Email,
                        SMTPPassword = configEntity.Pass,
                        EnableSsl = configEntity.EnableSsl
                    });
                }
                catch (Exception)
                {
                    var configEntity = _IConfigService.GetFirstRecord();
                    throw;
                }
                return Json(new { result = true });

            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "User", "SaveUser", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveUser(UserModel model)
        {
            try
            {
                var isEmailExists = _IUserService.isEmailExists(model.Id, UtilityHelper.Encrypt(model.Email));
                //var isMobileExists = _IUserService.isMobileExists(model.Id, UtilityHelper.Encrypt(model.Mobile));
                //var isPasswordExists = _IUserService.isPasswordExists(model.Id, UtilityHelper.Encrypt(model.Password));

                var isMobileExists = false;

                if (isEmailExists || isMobileExists)
                    return Json(new
                    {
                        result = false,
                        validation = true,
                        isEmailExists = isEmailExists,
                        isMobileExists = isMobileExists,
                        //isPasswordExists = isPasswordExists,
                        message = ""
                    });

                if (ModelState.IsValid)
                {
                    if (model.RoleId == 6)
                    {
                        if (string.IsNullOrEmpty(model.Trainer_AirLine_id.ToString()))
                        {
                            return Json(new { result = false, message = "Li Trainer id required fields." });
                        }
                    }
                    var entity = _IUserService.GetRecordById(model.Id).ToModel();

                    if (entity != null)
                    {
                        entity.UpdatedBy = CurrentSession.UserId;
                        entity.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        entity = new UserModel();
                        model.CreatedBy = CurrentSession.UserId;
                        model.CreatedOn = DateTime.Now;
                    }

                    entity.FirstName = model.FirstName;
                    entity.LastName = model.LastName;
                    entity.Email = UtilityHelper.Encrypt(model.Email);
                    entity.Password = UtilityHelper.Encrypt(model.Password);
                    entity.Mobile = UtilityHelper.Encrypt(model.Mobile);
                    entity.MacAddress = UtilityHelper.Encrypt(model.MacAddress);
                    entity.MacAddress1 = UtilityHelper.Encrypt(model.MacAddress1);
                    entity.RoleId = model.RoleId;
                    
                    entity.isActive = model.isActive;
                    if (model.Trainer_AirLine_id.ToString() == "")
                        entity.Trainer_AirLine_id = 0;
                    else
                        entity.Trainer_AirLine_id = model.Trainer_AirLine_id;


                    _IUserService.SaveRecord(entity.ToEntity());

                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "Please complete all the required fields." });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "User", "SaveUser", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult DeleteUser(long Id)
        {
            try
            {
                _IUserService.DeleteRecord(Id);
                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "User", "DeleteUser", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        #region User Defined functions
        public void GetTrainingCentersList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "Select" });

            var messages = _ITrainingCenterService.GetList(1).ToList();
            messages.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.Name });
            });

            TempData["TrainingCenters"] = new SelectList(list, "Value", "Text");
        }

        public void GetUsersRolesList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "Select" });

            var messages = _IRoleService.GetList().ToList();
            messages.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.RoleName });
            });

            TempData["UserRoles"] = new SelectList(list, "Value", "Text");
        }
        #endregion
    }
}