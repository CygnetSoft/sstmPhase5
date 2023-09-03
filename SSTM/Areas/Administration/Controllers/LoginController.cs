using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.Common;
using SSTM.Models.Login;
using SSTM.sg.com.eversafe.li;
using SSTM.CourseService;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SSTM.Core.IntroPage;
using System.Threading.Tasks;
using Hangfire;
using System.Collections.Generic;
using Hangfire.Storage;
using SSTM.Core.Course_Reminder;
using Ninject;
using SSTM.Models.Developer_Monitor_Timer;
using SSTM.Helpers.AutoMapping;

namespace SSTM.Areas.Administration.Controllers
{
    [ErrorHandler]
    public class LoginController : Controller
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IUserService _IUserService;
        private readonly IRoleService _IRoleService;
        private readonly ITrainingCenterService _ITrainingCenterService;
        private readonly IConfigService _IConfigService;
        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ICreateIntropageService _createIntropageService;
        private readonly ICourseReminderService _ICourseReminderService;
        private readonly IDeveloperMonitorTimerService _IDeveloperMonitorTimerService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public LoginController
            (IExceptionLogService exceptionLogService, IUserService userService, IRoleService roleService,
            ITrainingCenterService trainingCenterService, IConfigService configService,
            ICourseDocumentService courseDocumentService, ICourseReminderService ICourseReminderService,
            ICreateIntropageService createIntropageService, IDeveloperMonitorTimerService DeveloperMonitorTimerService)
        {
            _IExceptionLogService = exceptionLogService;
            _IUserService = userService;
            _IRoleService = roleService;
            _ITrainingCenterService = trainingCenterService;
            _IConfigService = configService;
            _ICourseDocumentService = courseDocumentService;
            _createIntropageService = createIntropageService;
            _ICourseReminderService = ICourseReminderService;
            _IDeveloperMonitorTimerService = DeveloperMonitorTimerService;
        }
        #endregion

        public ActionResult Index()
        {


            CourseService.SSTM service = new SSTM.CourseService.SSTM();
            var list = service.AllCourse();
            var model = new LoginModel();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var encryptedEmail = UtilityHelper.Encrypt(model.Email);
                    var encryptedPassword = UtilityHelper.Encrypt(model.Password);

                    var userEntity = _IUserService.GetRecordForLogin(encryptedEmail, encryptedPassword);
                    if (userEntity != null)
                    {
                        if (userEntity.isActive)
                        {
                            var roleEntity = _IRoleService.GetRecordById(userEntity.RoleId);
                            var userRole = roleEntity != null ? roleEntity.RoleName : "";

                            var trainingCenterEntity = _ITrainingCenterService.GetRecordById(userEntity.TrainingCenterId);
                            var trainingCenterName = trainingCenterEntity != null ? trainingCenterEntity.Name : "";

                            CurrentSession = new AppSession();
                            CurrentSession.UserId = userEntity.Id;
                            UtilityHelper.CurrentUserloginId = userEntity.Id;
                            CurrentSession.EncryptedUserId = UtilityHelper.Encrypt(userEntity.Id.ToString());
                            CurrentSession.UserName = userEntity.LastName != null ? userEntity.FirstName + " " + userEntity.LastName : userEntity.FirstName;
                            CurrentSession.UserEmail = model.Email;
                            CurrentSession.UserRole = userRole.Trim();
                            CurrentSession.Trainer_AirLine_id = userEntity.Trainer_AirLine_id;
                            CurrentSession.TrainingCenterName = trainingCenterName;
                            CurrentSession.isOTPVerified = false;
                            CurrentSession.isLocationVerified = true;

                            TrainerSectionDetails getCurrentCourseAndBatch = await _createIntropageService.GetCurrentCourseAndBatch(DateTime.Now.ToString("yyyy/MM/dd"), CurrentSession.Trainer_AirLine_id.ToString());


                            CurrentSession.CourseId = getCurrentCourseAndBatch.Courseid;
                            CurrentSession.BatchId = getCurrentCourseAndBatch.BatchId;
                            CurrentSession.TrainerCourseId = getCurrentCourseAndBatch.TrainerCourseid;
                            CurrentSession.TrainerBatchId = getCurrentCourseAndBatch.TrainerBatchId;



                            if (userRole.Trim() == "Administration" || userRole.Trim() == "Director" || userRole.Trim() == "Manager")
                                return Json(new { result = true, code = "success", URL = "/Administration/Login/VerifyLogin" });
                            else
                                return Json(new { result = false, code = "NotAdministrator", message = AppMessages.NotAdministrator });
                        }
                        else
                            return Json(new { result = false, code = "AccountInactive", message = AppMessages.AccountInactive });
                    }
                    else
                        return Json(new { result = false, code = "InValidCredentials", message = AppMessages.InValidCredentials });
                }
                else
                    return Json(new { result = false, code = "InValidCredentials", message = AppMessages.InValidCredentials });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Login", "Index", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception });
            }
        }

        #region OTP verification functions
        public ActionResult VerifyLogin()
        {

            try
            {
                if (CurrentSession == null)
                    return RedirectToAction("Index", "Login", new { area = "Administration" });
                else
                {
                    var userEntity = _IUserService.GetRecordById(CurrentSession.UserId);
                    if (userEntity != null)
                    {
                        MyWeb smsClient = new MyWeb();

                        var sixDigitOTP = UtilityHelper.GenerateSixDigitOTP();
                        DateTime currentTime = UtilityHelper.GetCurrentSGTDate();

                        if (!Request.IsLocal && userEntity.Mobile != null)
                            smsClient.SendSMS(UtilityHelper.Decrypt(userEntity.Mobile), string.Format(AppMessages.OTPMessage, sixDigitOTP, currentTime.ToString()));

                        ViewBag.UserMobile = UtilityHelper.Decrypt(userEntity.Mobile);

                        userEntity.OTP = UtilityHelper.Encrypt(sixDigitOTP);

                        _IUserService.SaveRecord(userEntity);

                        #region Send OTP email for developer team
                        try
                        {
                            var configEntity = _IConfigService.GetFirstRecord();
                            if (configEntity != null)
                            {
                                //if (UtilityHelper.Decrypt(userEntity.Email).Contains("mayursasp.net") ||
                                //    UtilityHelper.Decrypt(userEntity.Email).Contains("mayur.iblazing"))
                                //{
                                //    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                                //    {
                                //        From = configEntity.Email,
                                //        To = "mayur.iblazing@gmail.com",
                                //        Subject = "Login OTP",
                                //        Message = string.Format(AppMessages.OTPMessage, sixDigitOTP, currentTime.ToString()),
                                //        SMTPHost = configEntity.Host,
                                //        SMTPPort = configEntity.Port,
                                //        SMTPEmail = configEntity.Email,
                                //        SMTPPassword = configEntity.Pass,
                                //        EnableSsl = configEntity.EnableSsl
                                //    });
                                //}
                                //else
                                //{
                                EmailHelper.SendMail(new Models.EmailModel.EmailModel
                                {
                                    From = configEntity.Email,
                                    To = UtilityHelper.Decrypt(userEntity.Email).Trim(),
                                    Subject = "Login OTP",
                                    Message = string.Format(AppMessages.OTPMessage, sixDigitOTP, currentTime.ToString()),
                                    SMTPHost = configEntity.Host,
                                    SMTPPort = configEntity.Port,
                                    SMTPEmail = configEntity.Email,
                                    SMTPPassword = configEntity.Pass,
                                    EnableSsl = configEntity.EnableSsl
                                });
                                //}
                            }
                        }
                        catch (Exception) { }
                        #endregion

                        return View(CurrentSession);
                    }
                    else
                        return Json(new { result = false, code = "", message = "" });
                }
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Login", "VerifyLogin", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult OTPExpired(long Id)
        {
            try
            {
                var userEntity = _IUserService.GetRecordById(Id);
                if (userEntity != null)
                {
                    userEntity.OTP = null;
                    _IUserService.SaveRecord(userEntity);

                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, code = "Exception", message = AppMessages.Exception });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Login", "OTPExpired", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception });
            }
        }

        [HttpGet]
        public ActionResult ResendOTP(string Id)
        {
            try
            {
                var decryptedId = UtilityHelper.Decrypt(Id);
                if (decryptedId != "In Valid")
                {
                    var userEntity = _IUserService.GetRecordById(Convert.ToInt32(decryptedId));
                    if (userEntity != null)
                    {
                        MyWeb smsClient = new MyWeb();

                        var sixDigitOTP = UtilityHelper.GenerateSixDigitOTP();
                        DateTime currentTime = UtilityHelper.GetCurrentSGTDate();

                        if (!Request.IsLocal && userEntity.Mobile != null)
                            smsClient.SendSMS(UtilityHelper.Decrypt(userEntity.Mobile), string.Format(AppMessages.OTPMessage, sixDigitOTP, currentTime.ToString()));

                        ViewBag.UserMobile = UtilityHelper.Decrypt(userEntity.Mobile);

                        userEntity.OTP = UtilityHelper.Encrypt(sixDigitOTP);

                        _IUserService.SaveRecord(userEntity);

                        #region Send OTP email for developer team
                        try
                        {
                            var configEntity = _IConfigService.GetFirstRecord();
                            if (configEntity != null)
                            {
                                //if (UtilityHelper.Decrypt(userEntity.Email).Contains("mayursasp.net") ||
                                //    UtilityHelper.Decrypt(userEntity.Email).Contains("mayur.iblazing"))
                                //{
                                //    EmailHelper.SendMail(new Models.EmailModel.EmailModel()
                                //    {
                                //        From = configEntity.Email,
                                //        To = "mayur.iblazing@gmail.com",
                                //        Subject = "Login OTP",
                                //        Message = string.Format(AppMessages.OTPMessage, sixDigitOTP, currentTime.ToString()),
                                //        SMTPHost = configEntity.Host,
                                //        SMTPPort = configEntity.Port,
                                //        SMTPEmail = configEntity.Email,
                                //        SMTPPassword = configEntity.Pass,
                                //        EnableSsl = configEntity.EnableSsl
                                //    });
                                //}
                                //else
                                //{
                                EmailHelper.SendMail(new Models.EmailModel.EmailModel()
                                {
                                    From = configEntity.Email,
                                    To = UtilityHelper.Decrypt(userEntity.Email).Trim(),
                                    Subject = "Login OTP",
                                    Message = string.Format(AppMessages.OTPMessage, sixDigitOTP, currentTime.ToString()),
                                    SMTPHost = configEntity.Host,
                                    SMTPPort = configEntity.Port,
                                    SMTPEmail = configEntity.Email,
                                    SMTPPassword = configEntity.Pass,
                                    EnableSsl = configEntity.EnableSsl
                                });
                                //}
                            }
                        }
                        catch (Exception ex) { string s = ex.Message; }
                        #endregion

                        return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { result = false, code = "Exception", message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { result = false, code = "Exception", message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Login", "ResendOTP", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult VerifyOTP(string Id, string otp)
        {
            try
            {
                var decryptedId = UtilityHelper.Decrypt(Id);
                if (decryptedId != "In Valid")
                {
                    var userEntity = _IUserService.GetRecordById(Convert.ToInt32(decryptedId));
                    if (userEntity != null)
                    {
                        if (otp == UtilityHelper.Decrypt(userEntity.OTP))
                        {
                            CurrentSession = (AppSession)Session["AppSession"];
                            CurrentSession.isOTPVerified = true;

                            //Developer role users should not access SSTM from different machine rather than accessing from their machine
                            if (CurrentSession.UserRole == "Developer")
                            {
                              //  var userMacEntity = _IUserService.GetUserByMacAddress(CurrentSession.Macaddress, CurrentSession.UserId);

                                //var user = userMacEntity.Where(x => x.RoleId == 2).ToList();

                                //if (userMacEntity != null && userMacEntity.Any())
                                //{
                                //    foreach (var item in userMacEntity)
                                //    {
                                //        item.MacAddress = "FqaUSZ/BhgYVvxQxNyWwFg=="; //means 1
                                //        item.MacAddress1 = "FqaUSZ/BhgYVvxQxNyWwFg=="; //means 1
                                //        _IUserService.SaveRecord(item);
                                //    }
                                //}

                                //if (!string.IsNullOrEmpty(CurrentSession.Macaddress) && !string.IsNullOrEmpty(userEntity.MacAddress))
                                //{
                                //    if (userEntity.MacAddress != CurrentSession.Macaddress)
                                //    {
                                //        return Json(new { result = true, URL = "/Error/AccessForbidden" });
                                //    }
                                //}
                            }

                            if (CurrentSession.UserRole != "Developer")
                            {
                                if (_IUserService.isMacAddressExists(CurrentSession.Macaddress, CurrentSession.UserId))
                                {
                                    //var userMacEntity = _IUserService.GetUserByMacAddress(CurrentSession.Macaddress, CurrentSession.UserId);

                                    //if (userMacEntity != null)
                                    //{
                                        //var user = userMacEntity.Where(x => x.RoleId == 2).FirstOrDefault(); //select developer role if any from the list

                                        //if (user != null)
                                        //{
                                        //    if (user.MacAddress != CurrentSession.Macaddress)
                                        //        return Json(new { result = true, URL = "/Error/AccessForbidden" });
                                        //}

                                    //    foreach (var item in userMacEntity)
                                    //    {
                                    //        if (item.RoleId != 2) //Developer role or not
                                    //        {
                                    //            item.MacAddress = "FqaUSZ/BhgYVvxQxNyWwFg=="; //means 1
                                    //            item.MacAddress1 = "FqaUSZ/BhgYVvxQxNyWwFg=="; //means 1
                                    //            _IUserService.SaveRecord(item);
                                    //        }
                                    //    }
                                    //}
                                }

                                if (!string.IsNullOrEmpty(CurrentSession.Macaddress))
                                    userEntity.MacAddress = CurrentSession.Macaddress == userEntity.MacAddress ? userEntity.MacAddress : CurrentSession.Macaddress;
                            }

                            FormsAuthentication.SetAuthCookie(userEntity.Email, false);

                            userEntity.OTP = null;
                            _IUserService.SaveRecord(userEntity);

                            try
                            {
                                DateFormat dtsing = new DateFormat();
                                if (userEntity.RoleId == 2)
                                {
                                    var DeveloperLoginEntity = new DeveloperMonitorTimerModel
                                    {
                                        user_id = userEntity.Id,
                                        in_time = dtsing.GetSingaporeTime().TimeOfDay,
                                        date_time = dtsing.GetSingaporeTime()
                                    }.ToEntity();
                                    var newId = _IDeveloperMonitorTimerService.SaveRecord(DeveloperLoginEntity);
                                }
                            }
                            catch (Exception ex)
                            {
                            }


                            if (CurrentSession.UserRole == "Developer" || CurrentSession.UserRole == "SME" || CurrentSession.UserRole == "HR" ||
                                CurrentSession.UserRole == "Director" || CurrentSession.UserRole == "Staff" || CurrentSession.UserRole == "Manager"
                                )
                            {
                                return Json(new { result = true, URL = "/Course/Index" });
                                // return Json(new { result = true, URL = "/Course/MainCourse" });
                            }
                            else if (CurrentSession.UserRole == "Trainer")
                                return Json(new { result = true, URL = "/TodayClassDocs/Index" });
                            else if (CurrentSession.UserRole == "Aassociate Developer")
                                return Json(new { result = true, URL = "/CourseReminder/Index" });
                            else if (CurrentSession.UserRole == "AEB")
                                return Json(new { result = true, URL = "/TrainerQPUpload/Index" });
                            else if (CurrentSession.UserRole == "Print Incharge")
                                return Json(new { result = true, URL = "/Course/SharedCoursesList" });
                            else if (CurrentSession.UserRole == "DownloadLogin")
                                return Json(new { result = true, URL = "/CourseDownload" });
                            else if (CurrentSession.UserRole == "QP_Approval_Level1" || CurrentSession.UserRole == "QP_Approval_Level2" || CurrentSession.UserRole == "QP_Approval_Level3")
                                return Json(new { result = true, URL = "/TrainerQPUpload/QP_Approval_Level" });
                            //return Json(new { result = true, URL = "/Course/MainCourse" });
                            else if (CurrentSession.UserRole == "DirectorStaffs")
                                return Json(new { result = true, URL = "/TrainerUploadDocument/ConnfidentialDocument" });
                            else if (CurrentSession.UserRole == "CI")
                                return Json(new { result = true, URL = "/AssessmentPaper/Index" });
                            else
                                return Json(new { result = true, URL = "/Administration/User/Index" });
                        }
                        else
                            return Json(new { result = false, code = "InvalidOTP", AppMessages.InvalidOTPMessage }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { result = false, code = "Exception", message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { result = false, code = "Exception", message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Login", "VerifyOTP", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public ActionResult Signout()
        {
            DateFormat dtsing1 = new DateFormat();
            var userRole = CurrentSession.UserRole;

            try
            {
                if (CurrentSession.UserRole == "Developer")
                {
                    var monitorEntity = _IDeveloperMonitorTimerService.GetUserLastRecord(CurrentSession.UserId);
                    monitorEntity.user_id = CurrentSession.UserId;
                    monitorEntity.out_time = dtsing1.GetSingaporeTime().TimeOfDay;
                    monitorEntity.date_time = dtsing1.GetSingaporeTime();
                    var newId = _IDeveloperMonitorTimerService.SaveRecord(monitorEntity);
                }
            }
            catch (Exception ex)
            {
            }

            long Userid = UtilityHelper.CurrentUserloginId;

            if (Userid != 0)
            {
                var courseDocEntity = _ICourseDocumentService.UpdateDocumentStatusByUserRecordById(Userid).ToList();

                for (int i = 0; i < courseDocEntity.Count; i++)
                {
                    courseDocEntity[i].isOpened = false;
                    courseDocEntity[i].UserId = 0;
                    courseDocEntity[i].UpdatedBy = Userid;
                    courseDocEntity[i].UpdatedOn = DateTime.Now;

                    _ICourseDocumentService.SaveRecord(courseDocEntity[i]);
                }
            }

            var userEntity = _IUserService.GetRecordById(CurrentSession.UserId);
            var macAddress = userEntity.MacAddress;

            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            if (userRole == "Administration" || userRole == "Director" || userRole == "Manager")
                return RedirectToAction("Index", "Login", new { area = "Administration" });
            else
                return RedirectToAction("Index", "UserLogin", new { area = "", m = macAddress });
        }


    }
}