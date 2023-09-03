using SSTM.Business.Interfaces;
using SSTM.Helpers.App;
using SSTM.Helpers.Common;
using SSTM.Models.Login;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using SSTM.CourseService;
using SSTM.sg.com.eversafe.li;
using SSTM.Core.IntroPage;
using System.Threading.Tasks;
using System.Collections.Generic;
using SSTM.Core.Course_Reminder;
using Hangfire.Storage;
using Hangfire;
using IoC;
using Autofac;
using System.Timers;
using System.IO;
using SSTM.Models.Developer_Monitor_Timer;
using SSTM.Helpers.AutoMapping;
using System.Web.Script.Serialization;

namespace SSTM.Controllers
{
    [HandleError]
    public class UserLoginController : Controller
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IUserService _IUserService;
        private readonly IRoleService _IRoleService;
        private readonly ITrainingCenterService _ITrainingCenterService;
        private readonly IConfigService _IConfigService;
        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ITrainnerMacAddressService _ITrainnerMacAddressService;
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
        public UserLoginController
            (IExceptionLogService exceptionLogService, IUserService userService, IRoleService roleService,
            ITrainingCenterService trainingCenterService, IConfigService configService,
            ICourseDocumentService courseDocumentService, ICourseReminderService ICourseReminderService,
            ITrainnerMacAddressService TrainnerMacAddressService, ICreateIntropageService createIntropageService,
            IDeveloperMonitorTimerService DeveloperMonitorTimerService
            )
        {
            _IExceptionLogService = exceptionLogService;
            _IUserService = userService;
            _IRoleService = roleService;
            _ITrainingCenterService = trainingCenterService;
            _IConfigService = configService;
            _ICourseDocumentService = courseDocumentService;
            _ITrainnerMacAddressService = TrainnerMacAddressService;
            _createIntropageService = createIntropageService;
            _ICourseReminderService = ICourseReminderService;
            _IDeveloperMonitorTimerService = DeveloperMonitorTimerService;
        }
        #endregion
        public void time_elapsed(object sender, ElapsedEventArgs e)
        {
            AutoReminderMail();
        }
        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("### Timer Stopped ### \n");
            AutoReminderMail();
            timer.Stop();
            Console.WriteLine("### Scheduled Task Started ### \n\n");
            Console.WriteLine("Hello World!!! - Performing scheduled task\n");
            Console.WriteLine("### Task Finished ### \n\n");
            schedule_Timer();
        }
        static Timer timer;
        public void schedule_Timer()
        {
            Console.WriteLine("### Timer Started ###");

            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 23, 11, 0, 0); //Specify your scheduled time HH,MM,SS [8am and 42 minutes]
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            timer = new Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public ActionResult Index(string m)
        {
            DriveInfo drive = DriveInfo.GetDrives().Where(x => x.Name == @"E:\").FirstOrDefault();
            try
            {
                // schedule_Timer();
                // CreateRecurringJobs();
            }
            catch (Exception)
            {

            }

            //CourseService.SSTM service = new SSTM.CourseService.SSTM();
            //var list = service.TodayCoursesOnlyTrainer("2021/07/17");

            var model = new LoginModel();
            model.isLocationVerified = false;
            string isdelveloper = ConfigurationManager.AppSettings["isdeveloper"];
            if (Request.IsLocal)
            {
                model.isLocationVerified = true;
                model.Macaddress = m;
                return View(model);
            }
            //var encryptedPassword = UtilityHelper.Decrypt("ZAdHgForh4c/Tvr1 HwKohzGXR /RldmDiGuT8if2bQqkqNXIMrHGywEep rRzemEmiilwnIr TCHe84zn9ZBpWAp5cBqGgi7EZhlr9txH5aV0184jQGhYQBV8VhbsYu8riWorP9yfAGLgsovIJCUkO SX3EcBtdZ3GfS0ElD5wjTxlkAgnOvzHA26TFuRXVLxgydj7eDbh0awDmHFUSpH4jUOnMvrLRGCaO2ykwwhwwMdayBbkT7WUmhnmZ4P44gHIAdaAD8WXFf98RRO1B/n7SrU  4DtzIeeTCE/ok7I/zdeuWRjxxH/eC8yE0RIsY bCWFxZSN7n9uflleqljLOhVzc xz0OB14e5xWalOZHhCPH3CfVfTrgK0S euHfIX0OJL02Ec6WZI 1Z6tHkccuaXMURc66d2ZdtWN3AmY=");//live connection
            try
            {
                var decryptedMac = UtilityHelper.Decrypt(m);
                if (decryptedMac != "In Valid")
                {
                    int maccnt = 0;
                    var userCount = _IUserService.GetDefaultList().Where(a => a.MacAddress == m || a.MacAddress1 == m).Count();
                    var Maccount = _ITrainnerMacAddressService.GetAllMacAddress().Where(a => a.MacAddress == m).Count();
                    maccnt = userCount + Maccount;
                    if (maccnt > 0)
                    {
                        model.isLocationVerified = true;
                        model.Macaddress = m;
                    }
                    else
                        return RedirectToAction("AccessForbidden", "Error");
                }
                else
                    return RedirectToAction("AccessForbidden", "Error");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "UserLogin", "Index", Request.Url.AbsoluteUri, CurrentSession.UserId);
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model)
        {
            try
            {
                if (model.isLocationVerified)
                {
                    var encryptedEmail = UtilityHelper.Encrypt(model.Email);
                    var encryptedPassword = UtilityHelper.Encrypt(model.Password);

                    var userEntity = _IUserService.GetRecordForLogin(encryptedEmail, encryptedPassword);
                    if (userEntity != null)
                    {
                        //var hostIP = UtilityHelper.Encrypt(UtilityHelper.GetIPAddress());
                        //if (userEntity.MachineIP == hostIP)
                        //{
                        if (userEntity.isActive)
                        {

                            var roleEntity = _IRoleService.GetRecordById(userEntity.RoleId);
                            var userRole = roleEntity != null ? roleEntity.RoleName : "";

                            var trainingCenterEntity = _ITrainingCenterService.GetRecordById(userEntity.TrainingCenterId);
                            var trainingCenterName = trainingCenterEntity != null ? trainingCenterEntity.Name : "";
                            UtilityHelper.CurrentUserloginId = userEntity.Id;
                            CurrentSession = new AppSession();
                            CurrentSession.UserId = userEntity.Id;
                            CurrentSession.EncryptedUserId = UtilityHelper.Encrypt(userEntity.Id.ToString());
                            CurrentSession.UserName = userEntity.LastName != null ? userEntity.FirstName + " " + userEntity.LastName : userEntity.FirstName;
                            CurrentSession.UserEmail = model.Email;
                            CurrentSession.UserRole = userRole.Trim();
                            CurrentSession.Trainer_AirLine_id = userEntity.Trainer_AirLine_id;
                            CurrentSession.TrainingCenterName = trainingCenterName;
                            CurrentSession.isOTPVerified = false;
                            CurrentSession.isLocationVerified = model.isLocationVerified;
                            CurrentSession.Macaddress = model.Macaddress;

                            TrainerSectionDetails getCurrentCourseAndBatch = await _createIntropageService.GetCurrentCourseAndBatch(DateTime.Now.ToString("yyyy/MM/dd"), CurrentSession.Trainer_AirLine_id.ToString());
                            if (getCurrentCourseAndBatch != null)
                            {
                                CurrentSession.CourseId = getCurrentCourseAndBatch.Courseid;
                                CurrentSession.BatchId = getCurrentCourseAndBatch.BatchId;
                                CurrentSession.TrainerCourseId = getCurrentCourseAndBatch.TrainerCourseid;
                                CurrentSession.TrainerBatchId = getCurrentCourseAndBatch.TrainerBatchId;

                            }


                            return Json(new { result = true, code = "success", URL = "/Administration/Login/VerifyLogin" });
                        }
                        else
                            return Json(new { result = false, code = "AccountInactive", message = AppMessages.AccountInactive });
                        //}
                        //else
                        //    return Json(new { result = false, code = "NotAuthorized", URL = "/Error/AccessForbidden" });
                    }
                    else
                        return Json(new { result = false, code = "InValidCredentials", message = AppMessages.InValidCredentials });
                }
                else
                    return Json(new { result = false, code = "NotAuthorized", URL = "/Error/AccessForbidden" });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "UserLogin", "Index", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception });
            }
        }

        public void resetUserDocumentStatus()
        {


            long Userid = UtilityHelper.CurrentUserloginId;

            if (Userid != 0)
            {
                try
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
                catch (Exception)
                {
                }

                DateFormat dtsing = new DateFormat();
                var data = _IUserService.GetRecordById(Userid);
                if (data != null)
                {
                    try
                    {
                        if (data.RoleId == 2)
                        {
                            var monitorEntity = _IDeveloperMonitorTimerService.GetUserLastRecord(Userid);
                            monitorEntity.user_id = data.Id;
                            monitorEntity.out_time = dtsing.GetSingaporeTime().TimeOfDay;
                            monitorEntity.date_time = dtsing.GetSingaporeTime();
                            var newId = _IDeveloperMonitorTimerService.SaveRecord(monitorEntity);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
               // UserBlockWithIPR();
            }
        }

        public void UserBlockWithIPR()
        {
            DateFormat dtformat = new DateFormat();
            GetLessonPlanDetails_final GetLessonPlanDetails_model = new GetLessonPlanDetails_final();
            string topic_data = "";
            try
            {
                CourseService.SSTM service = new CourseService.SSTM();

                string data = service.GetLessonPlanDetails(Convert.ToInt32(UtilityHelper.CurrentCourseId), UtilityHelper.CurrentbatchId);
                GetLessonPlanDetails_model = (new JavaScriptSerializer()).Deserialize<GetLessonPlanDetails_final>(data);
                ViewBag.Rem_Time = DateTime.Now.AddMinutes(2).ToString("dd-MM-yyyy h:mm:ss tt");
                int i = 1;
                if (GetLessonPlanDetails_model.data != null)
                {
                    foreach (var item in GetLessonPlanDetails_model.data)
                    {
                        i++;
                        DateTime time = DateTime.Today;

                        #region Convert 12 hr to 24hrs                       
                        string starttime = Convert.ToDateTime(item.starttime + " " + item.startmeridian).ToString("HH:mm");
                        string endtime = Convert.ToDateTime(item.endtime + " " + item.endmeridian).ToString("HH:mm");
                        #endregion

                        DateTime currentdt = dtformat.GetSingaporeTime();
                        //DateTime currentdt = new DateTime(2023, 08, 26, 01, 16, 20);
                        DateTime hr1 = new DateTime(currentdt.Year, currentdt.Month, currentdt.Day, Convert.ToInt32(starttime.Split(':')[0]), Convert.ToInt32(starttime.Split(':')[1]), 0);
                        DateTime hr2 = new DateTime(currentdt.Year, currentdt.Month, currentdt.Day, Convert.ToInt32(endtime.Split(':')[0]), Convert.ToInt32(endtime.Split(':')[1]), 0);


                        if (currentdt > hr1 && currentdt < hr2)
                        {
                            if (item.topic.Trim() == "Instant Performance Reflection")
                            {
                                //var user = _IUserService.GetRecordById(UtilityHelper.CurrentUserloginId);
                               
                                //_IUserService.SaveRecord(user);
                                break;
                            }
                        }

                    }
                }

            }
            catch (Exception)
            {
                ViewBag.Error = "";
            }
        }
        public void CreateRecurringJobs()
        {
            List<RecurringJobDto> list;
            using (var connection = JobStorage.Current.GetConnection())
            {
                list = connection.GetRecurringJobs();
            }

            var builder = new ContainerBuilder();


            var job = list?.FirstOrDefault(j => j.Id == "Daily");  // jobId is the recurring job ID, whatever that is
            if (job == null)
                RecurringJob.AddOrUpdate("Daily", () => AutoReminderMail(), Cron.Daily(11, 00));
        }


        public void AutoReminderMail()
        {
            try
            {
                var configEntity = _IConfigService.GetFirstRecord();
                List<CourseReminder> NewCourseList = new List<CourseReminder>();

                NewCourseList = _ICourseReminderService.GetAllRecord().Where(a => a.steps == 4).ToList();
                foreach (var devitem in NewCourseList)
                {
                    #region Email to admin and director to developer sign added for approval.
                    double reminingDays = Convert.ToDateTime(devitem.renew_date).Subtract(DateTime.Today).TotalDays;
                    if (reminingDays == 2)
                    {
                        if (configEntity != null)
                        {
                            // var dirAdminEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == 1 || a.RoleId == 4).ToList();
                            var developerEntity = _IUserService.GetRecordById(devitem.DeveloperId);

                            var emailBody = UtilityHelper.GetEmailTemplate("NewCourseAutoReminder.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", developerEntity.FirstName + " " + developerEntity.LastName)
                                .Replace("@courseName@", devitem.course_name)
                                .Replace("@developername@", developerEntity.FirstName + " " + developerEntity.LastName);

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(developerEntity.Email),
                                Subject = "SSTM developer New Course Auto Reminder Mail (" + devitem.course_name + ")",
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
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}