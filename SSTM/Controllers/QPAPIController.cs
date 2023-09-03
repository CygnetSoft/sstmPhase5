using SSTM.Business.Interfaces;
using SSTM.Core.Course_Reminder;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Helpers.DBHandlers;
using SSTM.Helpers.Helpers;
using SSTM.Models.AWS;
using SSTM.Models.CourseTrackers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SSTM.Controllers
{
    public class QPAPIController : Controller
    {
        #region Class Properties Declarations
        private readonly ITrainerUploadDocumentService _ITrainerUploadDocumentService;
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        private readonly ITrainerQPUploadService _ITrainerQPUploadService;
        private readonly ITrainerQPSharedStudentService _TrainerQPSharedStudentService;
        private readonly ICourseReminderService _ICourseReminderService;
        private readonly ICourseReminderLatterUndertakingService _ICourseReminderLatterUndertakingService;
        private readonly ICourseReminderTrackerService _ICourseReminderTrackerService;
        private readonly IDeveloperMonitorTimerService _IDeveloperMonitorTimerService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        public QPAPIController(ITrainerUploadDocumentService ITrainerUploadDocumentService,
            IExceptionLogService exceptionLogService,
             IConfigService configService, IUserService userService, ITrainerQPUploadService ITrainerQPUploadService,
             ITrainerQPSharedStudentService ITrainerQPSharedStudentService,
             ICourseReminderService ICourseReminderService,
             ICourseReminderLatterUndertakingService ICourseReminderLatterUndertakingService,
             ICourseReminderTrackerService ICourseReminderTrackerService,
              IDeveloperMonitorTimerService DeveloperMonitorTimerService)
        {
            _ITrainerUploadDocumentService = ITrainerUploadDocumentService;
            _IExceptionLogService = exceptionLogService;
            _IConfigService = configService;
            _IUserService = userService;
            _ITrainerQPUploadService = ITrainerQPUploadService;
            _TrainerQPSharedStudentService = ITrainerQPSharedStudentService;
            _ICourseReminderService = ICourseReminderService;
            _ICourseReminderLatterUndertakingService = ICourseReminderLatterUndertakingService;
            _ICourseReminderTrackerService = ICourseReminderTrackerService;
            _IDeveloperMonitorTimerService = DeveloperMonitorTimerService;
        }
        #endregion

        public ActionResult GetQPToStudent(string courseid, string batch_id)
        {
            if (courseid == "")
            {
                return Json(new { result = false, message = "Course Id required !", data = "" }, JsonRequestBehavior.AllowGet);
            }
            if (batch_id == "")
            {
                return Json(new { result = false, message = "Batch Id required !", data = "" }, JsonRequestBehavior.AllowGet);
            }
            var list = _TrainerQPSharedStudentService.GetQpToStundetList(courseid, batch_id);
            return Json(new { result = true, message = "Success", data = list }, JsonRequestBehavior.AllowGet);
        }

        #region course proposal Api
        [System.Web.Http.HttpPost]
        public ActionResult courseproposal(coursePropesalModel course)
        {
            if (string.IsNullOrEmpty(course.course_id.ToString()))
            {
                return Json(new { result = false, message = "Course Id required !", data = "" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(course.status.ToString()))
            {
                return Json(new { result = false, message = "Status required !", data = "" }, JsonRequestBehavior.AllowGet);
            }
            var model = _ICourseReminderService.GetRecordByLiId(Convert.ToInt64(course.course_id));
            if (model == null)
            {
                return Json(new { result = false, message = "data not found", data = model }, JsonRequestBehavior.AllowGet);
            }
            model.course_proposal_link = course.status;
            if (model.course_proposal_link != "Pending" && model.need_analysis_link != "Pending")
            {
                model.steps = course.status == "2" ? Convert.ToInt32(coursestep.courseproposal) : 1;
            }

            model.course_proposal_link = course.status;
            _ICourseReminderService.SaveRecord(model);

            #region New course Track Entry

            var NewcourseTrackentity = _ICourseReminderTrackerService.GetDocument(model.Id);

            NewcourseTrackentity.CourseProposalUserid = model.DeveloperId;
            NewcourseTrackentity.CourseProposalDate = DateTime.Now;
            _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);

            #endregion

            return Json(new { result = true, message = "Success", data = model }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Course Need Analysis API
        [System.Web.Http.HttpPost]
        public ActionResult CourseNeedAnalysis(coursePropesalModel course)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            if (string.IsNullOrEmpty(course.course_id.ToString()))
            {
                return Json(new { result = false, message = "Course Id required !", data = "" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(course.status.ToString()))
            {
                return Json(new { result = false, message = "Status required !", data = "" }, JsonRequestBehavior.AllowGet);
            }
            var model = _ICourseReminderService.GetRecordByLiId(Convert.ToInt64(course.course_id));
            //if (string.IsNullOrEmpty(course.resetstatus))
            //{
            //    if (!string.IsNullOrEmpty(model.need_analysis_link) && !string.IsNullOrEmpty(model.course_proposal_link))
            //    {
            //        return Json(new { result = false, message = "Already set status", data = model }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            model.need_analysis_link = course.status;

            if (model.course_proposal_link != "Pending" && model.need_analysis_link != "Pending")
            {
                model.steps = course.status == "2" ? Convert.ToInt32(coursestep.courseproposal) : 1;
            }
            _ICourseReminderService.SaveRecord(model);

            #region New course Track Entry

            var NewcourseTrackentity = _ICourseReminderTrackerService.GetDocument(Convert.ToInt64(model.Id));

            NewcourseTrackentity.NeedanalysisUserid = model.DeveloperId;
            NewcourseTrackentity.NeedanalysisDate = DateTime.Now;
            _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);

            #endregion

            #region AEB meeting to fix developer Mail
            if (model.Id != 0)
            {
                if (configEntity != null)
                {
                    // string toEmails = "";
                    var developerEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == 11 || a.RoleId == 4).ToList();
                    //developerEntity.ForEach(a => { toEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                    foreach (var item in developerEntity)
                    {
                        var emailBody = UtilityHelper.GetEmailTemplate("AEBMeetingToDeveloper.html").ToString();
                        emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                            .Replace("@CourseName@", model.course_name)
                            .Replace("@status@", course.status)
                            .Replace("@courseproposal@", "li.eversafe.com.sg/")
                            .Replace("@needanalysis@", "li.eversafe.com.sg/");

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(item.Email),
                            Subject = "SSTM AEB meeting to fix developer (" + model.course_name + ")",
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

            return Json(new { result = true, message = "Success", data = model }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Latter undertaiking With developer
        public ActionResult Letterofundertaking(string id)
        {
            try
            {
                string[] get_id = id.ToString().Split('@');
                string courseid = get_id[0];
                var configEntity = _IConfigService.GetFirstRecord();
                var model = _ICourseReminderService.GetRecordById(Convert.ToInt64(courseid));
                if (!string.IsNullOrEmpty(model.latter_signature))
                {

                    ViewBag.newcourseid = 0;
                    ViewBag.latter = "";
                    ViewBag.error = "already taken sign";
                    return View(model);
                }
                model.latter_signature = !string.IsNullOrEmpty(model.latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.latter_signature : "";
                model.director_latter_signature = !string.IsNullOrEmpty(model.director_latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.director_latter_signature : "";
                var entity = _ICourseReminderLatterUndertakingService.GetFirstRecord();
                var userEntity = _IUserService.GetRecordById(model.DeveloperId);
                //StringBuilder latter_data =new StringBuilder();
                var latter_data = entity.latter_content.ToString();
                if (userEntity != null)
                {
                    var Li_trainer = Li_trainer_detail(userEntity.Trainer_AirLine_id);
                    latter_data = latter_data.Replace("@@name@@", userEntity.FirstName + " " + userEntity.LastName).Replace("@@icno@@", Li_trainer.fin);

                }
                ViewBag.newcourseid = Convert.ToInt64(courseid);
                ViewBag.latter = latter_data;
                ViewBag.error = "";
                return View(model);
            }
            catch (System.Exception ex)
            {

                var model = new CourseReminder();
                ViewBag.newcourseid = 0;
                ViewBag.latter = "";
                ViewBag.error = ex.Message;
                return View(model);
            }
        }

        #endregion

        #region Latter undertaiking With Director
        public ActionResult directorLetterofundertaking(string id)
        {
            try
            {
                string[] get_id = id.ToString().Split('@');
                string courseid = get_id[0];
                var configEntity = _IConfigService.GetFirstRecord();
                var model = _ICourseReminderService.GetRecordById(Convert.ToInt64(courseid));
                if (!string.IsNullOrEmpty(model.director_latter_signature))
                {

                    ViewBag.newcourseid = 0;
                    ViewBag.latter = "";
                    ViewBag.error = "already taken sign";
                    return View(model);
                }
                model.latter_signature = !string.IsNullOrEmpty(model.latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.latter_signature : "";
                model.director_latter_signature = !string.IsNullOrEmpty(model.director_latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.director_latter_signature : "";
                var entity = _ICourseReminderLatterUndertakingService.GetFirstRecord();
                var userEntity = _IUserService.GetRecordById(model.DeveloperId);
                //StringBuilder latter_data =new StringBuilder();
                var latter_data = entity.latter_content.ToString();
                if (userEntity != null)
                {
                    var Li_trainer = Li_trainer_detail(userEntity.Trainer_AirLine_id);
                    latter_data = latter_data.Replace("@@name@@", userEntity.FirstName + " " + userEntity.LastName).Replace("@@icno@@", Li_trainer.fin);
                }
                ViewBag.newcourseid = Convert.ToInt64(courseid);
                ViewBag.latter = latter_data;
                ViewBag.error = "";
                return View(model);
            }
            catch (System.Exception ex)
            {

                var model = new CourseReminder();
                ViewBag.newcourseid = 0;
                ViewBag.latter = "";
                ViewBag.error = ex.Message;
                return View(model);
            }
        }
        #endregion

        public LITrainerModel Li_trainer_detail(long? trainerid)
        {
            LITrainerModel LitrinModel = new LITrainerModel();
            CourseService.SSTM service = new SSTM.CourseService.SSTM();
            string data = service.AllTrainer();

            List<LITrainerModel> Li_trainer_list = (new JavaScriptSerializer()).Deserialize<List<LITrainerModel>>(data);
            LitrinModel = Li_trainer_list.Where(a => a.TrainerID == trainerid).FirstOrDefault();

            return LitrinModel;
        }

        #region Save Latter undertaiking With Director or Developer

        public ActionResult CourseReminder_Latter(string id, bool isdeveloper, long developerid, System.Web.HttpPostedFileBase file)
        {
            var configEntity = _IConfigService.GetFirstRecord();
            var entity = _ICourseReminderService.GetRecordById(Convert.ToInt64(id));
            string fileName = null;
            if (Request.Files.Count > 0)
            {

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


            if (isdeveloper == true)
            {
                entity.latter_signature = fileName;
                entity.latter_undertaking = !string.IsNullOrEmpty(entity.director_latter_signature) || !string.IsNullOrEmpty(entity.latter_signature) ? true : false;
            }
            else
            {
                entity.steps = Convert.ToInt32(coursestep.latter);
                entity.director_latter_signature = fileName;
                entity.latter_undertaking = !string.IsNullOrEmpty(entity.director_latter_signature) || !string.IsNullOrEmpty(entity.latter_signature) ? true : false;
            }
            if (isdeveloper == false)
            {
                #region New course Track Entry

                var NewcourseTrackentity = _ICourseReminderTrackerService.GetDocument(entity.Id);

                NewcourseTrackentity.LetterofundertakingUserId = CurrentSession.UserId;
                NewcourseTrackentity.LetterofundertakingDate = DateTime.Now;
                _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);

                #endregion
            }

            // entity.steps = Convert.ToInt32(coursestep.latter);
            _ICourseReminderService.SaveRecord(entity);
            if (isdeveloper == true)
            {

                #region Email to admin and director to developer sign added for approval.

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

                #endregion
            }
            return Json(new { result = true, message = "Successfully save." });

        }

        #endregion

        public void AutoReminderMail()
        {
            var configEntity = _IConfigService.GetFirstRecord();
            List<CourseReminder> NewCourseList = new List<CourseReminder>();

            NewCourseList = _ICourseReminderService.GetAllRecord().Where(a => a.steps == 4).ToList();
            foreach (var devitem in NewCourseList)
            {
                #region Email to admin and director to developer sign added for approval.
                double reminingDays = Convert.ToDateTime(devitem.renew_date).Subtract(DateTime.Today).TotalDays < 0 ? 0 : Convert.ToDateTime(devitem.renew_date).Subtract(DateTime.Today).TotalDays;
                if (reminingDays == 2 || reminingDays == 1)
                {
                    if (configEntity != null)
                    {

                        // var dirAdminEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == 1 || a.RoleId == 4).ToList();
                        var developerEntity = _IUserService.GetRecordById(devitem.DeveloperId);


                        #region New course Track Entry

                        var NewcourseTrackentity = _ICourseReminderTrackerService.GetDocument(devitem.Id);
                        NewcourseTrackentity.RenewReminderUser = "auto mail";
                        NewcourseTrackentity.RenewReminderDate = DateTime.Now;
                        _ICourseReminderTrackerService.SaveRecord(NewcourseTrackentity);

                        #endregion


                        var emailBody = UtilityHelper.GetEmailTemplate("NewCourseAutoReminder.html").ToString();
                        emailBody = emailBody.Replace("@DearName@", developerEntity.FirstName + " " + developerEntity.LastName)
                            .Replace("@courseName@", devitem.course_name)
                            .Replace("@developername@", developerEntity.FirstName + " " + developerEntity.LastName);

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(developerEntity.Email),
                            Subject = "SSTM auto reminder mail for (" + devitem.course_name + ")",
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


        public JsonResult DriveBackupCourse()
        {
            string drivepath = @"C:\SyncSSTM5DriveBackup\";
            DriveInfo drive = DriveInfo.GetDrives().Where(x => x.Name == @"C:\").FirstOrDefault();
            if (drive == null)
            {
                return Json(new { result = true, message = "Drive not found" });
            }

            Utility.WriteToFile(drivepath +" SSTMDownloads Service start.");
            try
            {
                Utility.WriteToFile("************************************************************************");
                Utility.WriteToFile("{0}: SSTMDownloads Service started.");

                var objDbLibrary = new DbLibrary();
                var configAWS = objDbLibrary.GetQueryDataTable("SELECT * FROM sstmo.Config");

                var decryptedMac = Utility.StaticEncrypt(Utility.GetMacAddress());

                if (configAWS != null && configAWS.Rows.Count > 0)
                {
                    if (Convert.ToString(configAWS.Rows[0]["AWSAccessKey"]) != "" &&
                        Convert.ToString(configAWS.Rows[0]["AWSSecretKey"]) != "" &&
                        Convert.ToString(configAWS.Rows[0]["BucketName"]) != "")
                    {
                        var awsModel = new SSTM.Helpers.Model.AWSModel()
                        {
                            AccessKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSAccessKey"])),
                            SecreteKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSSecretKey"])),
                            BucketName = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["BucketName"]))
                        };
                        objDbLibrary.MasterCourse = true; //Main Course
                        objDbLibrary.MasterCoursId = 0;//master course
                        var dtMainCourses = objDbLibrary.GetDataTable("sstmo.GetListOfSharedCourses_download", objDbLibrary.GrvFilldataCoursewithCourseandSubCourse);
                        Utility.WriteToFile("{0}: SP RUn.");
                        if (dtMainCourses != null && dtMainCourses.Rows.Count > 0)
                        {
                            foreach (DataRow courseRowMain in dtMainCourses.Rows)
                            {
                                //main start
                                var destinationDirMain = Path.Combine(drivepath, Convert.ToString(courseRowMain["CourseName"]));

                                try
                                {
                                    if (Directory.Exists(destinationDirMain))
                                        Directory.Delete(destinationDirMain, true);
                                }
                                catch (Exception)
                                {
                                }

                                if (!Directory.Exists(destinationDirMain))
                                    Directory.CreateDirectory(destinationDirMain);

                                Utility.WriteToFile("{0}: SP RUn.");
                                downloadDocument(destinationDirMain, Convert.ToString(courseRowMain["CourseName"]), long.Parse(courseRowMain["CourseId"].ToString()), awsModel);

                                //main end

                                //Sub course 1 start
                                var objDbLibrarysub1 = new DbLibrary();
                                objDbLibrarysub1.MasterCourse = false; //sub Course
                                objDbLibrarysub1.MasterCoursId = long.Parse(Convert.ToString(courseRowMain["CourseId"])); //sub Course

                                var dtSub1 = objDbLibrarysub1.GetDataTable("sstmo.GetListOfSharedCourses_download", objDbLibrarysub1.GrvFilldataCoursewithCourseandSubCourse);
                                if (dtSub1 != null && dtSub1.Rows.Count > 0)
                                {
                                    foreach (DataRow courseRowsub in dtSub1.Rows)
                                    {
                                        var destinationDirsub1 = Path.Combine(drivepath + courseRowMain["CourseName"], Convert.ToString(courseRowsub["CourseName"]));

                                        if (!Directory.Exists(destinationDirsub1))
                                            Directory.CreateDirectory(destinationDirsub1);

                                        downloadDocument(destinationDirsub1, Convert.ToString(courseRowsub["CourseName"]), long.Parse(courseRowsub["CourseId"].ToString()), awsModel);

                                        //start sub 2
                                        var objDbLibrarysub2 = new DbLibrary();
                                        objDbLibrarysub2.MasterCourse = false; //sub Course
                                        objDbLibrarysub2.MasterCoursId = long.Parse(Convert.ToString(courseRowsub["CourseId"])); //sub Course

                                        var dtSub2 = objDbLibrarysub2.GetDataTable("sstmo.GetListOfSharedCourses_download", objDbLibrarysub2.GrvFilldataCoursewithCourseandSubCourse);
                                        if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                        {
                                            foreach (DataRow courseRowsub2 in dtSub2.Rows)
                                            {
                                                var destinationDirsub2 = Path.Combine(drivepath + courseRowMain["CourseName"] + "\\" + Convert.ToString(courseRowsub["CourseName"]), Convert.ToString(courseRowsub2["CourseName"]));

                                                if (!Directory.Exists(destinationDirsub2))
                                                    Directory.CreateDirectory(destinationDirsub2);

                                                downloadDocument(destinationDirsub2, Convert.ToString(courseRowsub2["CourseName"]), long.Parse(courseRowsub2["CourseId"].ToString()), awsModel);

                                                var objDbLibrarysub3 = new DbLibrary();
                                                objDbLibrarysub3.MasterCourse = false; //sub Course
                                                objDbLibrarysub3.MasterCoursId = long.Parse(Convert.ToString(courseRowsub2["CourseId"])); //sub Course

                                                var dtSub3 = objDbLibrarysub3.GetDataTable("sstmo.GetListOfSharedCourses_download", objDbLibrarysub3.GrvFilldataCoursewithCourseandSubCourse);
                                                if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                                {
                                                    foreach (DataRow courseRowsub3 in dtSub3.Rows)
                                                    {
                                                        var destinationDirsub3 = Path.Combine(drivepath + courseRowMain["CourseName"] + "\\" + Convert.ToString(courseRowsub["CourseName"]) + "\\" + Convert.ToString(courseRowsub2["CourseName"]), Convert.ToString(courseRowsub3["CourseName"]));

                                                        if (!Directory.Exists(destinationDirsub3))
                                                            Directory.CreateDirectory(destinationDirsub3);

                                                        downloadDocument(destinationDirsub3, Convert.ToString(courseRowsub3["CourseName"]), long.Parse(courseRowsub3["CourseId"].ToString()), awsModel);

                                                    }
                                                }
                                                //end 
                                            }
                                        }
                                        //end sub 2
                                    }
                                    // end sub 1

                                }
                            }
                        }
                        else
                            Utility.WriteToFile("{0}: No data found.");
                    }
                    else
                        Utility.WriteToFile("{0}: No details found to connect cloud storage.");
                }
                else
                    Utility.WriteToFile("{0}: No details found to connect cloud storage.");

                Utility.WriteToFile("{0}: SSTMDownloads Service stopped.");
                Utility.WriteToFile("************************************************************************");

                //Stop the Windows Service.
                //using (ServiceController serviceController = new ServiceController("SSTMDownloads"))
                //{
                //    serviceController.Stop();
                //}
            }
            catch (Exception ex)
            {
                Utility.WriteToFile("{0}: Exception :: " + ex.Message + ex.StackTrace);
                Utility.WriteToFile("{0}: ************************************************************************");

            }

            return Json(new { result = true, message = "" });
            Utility.WriteToFile("{0}: SSTMDownloads Service end.");
        }

        public void downloadDocument(string destinationDir, string CourseName, long CourseId, SSTM.Helpers.Model.AWSModel awsModel)
        {
            Utility.WriteToFile("{0}: Checking for Course: " + Convert.ToString(CourseName));
            var objDbLibrary = new DbLibrary();
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            awsModel.BucketDirectory = Convert.ToString(CourseId);

            var parameters = new SqlParameter[1] { new SqlParameter("@CourseId", Convert.ToInt32(CourseId)) };

            var sharedCoursesDocs = objDbLibrary.GetDataTable("sstmo.GetListofSharedCourseDocs", parameters);
            if (sharedCoursesDocs != null && sharedCoursesDocs.Rows.Count > 0)
            {
                foreach (DataRow sharedCoursesDocRow in sharedCoursesDocs.Rows)
                {
                    Utility.WriteToFile("{0}: Checking for Course Document: " + Convert.ToString(sharedCoursesDocRow["DocName"]));

                    awsModel.FileName = Convert.ToString(sharedCoursesDocRow["Filename"]);
                    string s = sharedCoursesDocRow["dates"].ToString().Replace(".", "");
                    string s1 = s.ToString().Replace("-", "");
                    string s3 = s1.ToString().Replace(":", "");
                    string s2 = s3.ToString().Replace(" ", "");

                    string Fname = Convert.ToString(sharedCoursesDocRow["DocName"]) + "_" + s2.Replace("/", "").Replace("am", "").Replace("pm", "");

                    var destinationFile = Path.Combine(destinationDir, Fname) + Path.GetExtension(awsModel.FileName);

                    Utility.WriteToFile("{0}: path : " + destinationFile);
                    string[] values = Fname.Split('_');
                    string names = values[0];

                    DirectoryInfo d = new DirectoryInfo(destinationDir);
                    FileInfo[] Files = d.GetFiles("*");

                    Utility.WriteToFile("{0}: Updating course document: " + Convert.ToString(sharedCoursesDocRow["DocName"]));

                    awsModel.FilePath = destinationFile;
                    try
                    {
                        AWS.GetSingleFile(awsModel);
                    }
                    catch (Exception)
                    {
                    }
                    Utility.WriteToFile("{0}: Course document updated: " + Convert.ToString(sharedCoursesDocRow["DocName"]) + ".");

                }
            }
            else
                Utility.WriteToFile("{0}: No documents found for " + Convert.ToString(CourseName) + ".");

        }



        public void GetDeveloperMonitorList()
        {
            var sBuilder = new StringBuilder();
            try
            {
                var list = _IDeveloperMonitorTimerService.GetDetailsList(DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), 0).ToList();
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item.totaltime))
                    {
                        string[] data = item.totaltime.ToString().Split(':');
                        if (Convert.ToInt32(data[0]) < 04)
                        {
                            #region Send email notification to Trainer
                            var directorEntity = _IUserService.GetDirectorAndAdminList();
                            if (directorEntity.Count() != 0)
                            {
                                foreach (var itemd in directorEntity)
                                {
                                    var toEmails = UtilityHelper.Decrypt(itemd.Email);
                                    var configEntity = _IConfigService.GetFirstRecord();
                                    var emailBody = item.username + " Date " + item.date + " working is < 4 HRS";

                                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                                    {
                                        From = configEntity.Email,
                                        To = Request.IsLocal ? "meetmayur87@gmail.com" : toEmails,
                                        Subject = "SSTM Developer Monitoring",
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
                }

            }
            catch (Exception ex) { }

        }
    }
}
public class coursePropesalModel
{
    public long? course_id { get; set; }
    public string status { get; set; }
    public string resetstatus { get; set; }
}
