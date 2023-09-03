using Newtonsoft.Json;
using SSTM.Business.Interfaces;
using SSTM.Core.Config;
using SSTM.Core.IntroPage;
using SSTM.Helpers.App;
using SSTM.Helpers.Common;
using SSTM.Models;
using SSTM.Models.AWS;
using SSTM.Models.IntroPage;
using SSTM.Websockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SSTM.Controllers
{
    public class IntroStudentController : Controller
    {
        private readonly IIntroService _introService;
        private readonly ICreateIntropageService _createIntropageService;
        private readonly IFeedbackService _feedbackService;
        private readonly IStudentMcqService _studentMcqService;
        private readonly IStudentNotification _studentNotification;
        private readonly IConfigService _configService;
        private readonly Config configEntity;
        private readonly IUserService _IUserService;
        private readonly IRiskAssessmentDeclarationService _riskAssessmentDeclarationService;
        DateFormat dtformat;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return Session["AppSession"] as AppSession; }
        }

        public IntroStudentController(IIntroService introService,
            IConfigService configService,
            ICreateIntropageService createIntropageService,
            IFeedbackService feedbackService, IStudentMcqService studentMcqService,
            IStudentNotification studentNotification, IUserService userService
            , IRiskAssessmentDeclarationService _riskAssessmentDeclarationService)
        {
            _IUserService = userService;
            _introService = introService;
            _configService = configService;
            _createIntropageService = createIntropageService;
            _feedbackService = feedbackService;
            configEntity = configService.GetFirstRecord();
            _studentMcqService = studentMcqService;
            _studentNotification = studentNotification;
            this._riskAssessmentDeclarationService = _riskAssessmentDeclarationService;
            dtformat = new DateFormat();
        }

        // GET: IntroStudent
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult IntroPage(string studentId)
        {

            var query = UtilityHelper.JsonSstmDecrypt(studentId);
            StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(query);

            TempData["studentId"] = student.StudentId;
            TempData["studentName"] = student.StudentName;
            TempData["courseId"] = student.CourseId;
            TempData["batchId"] = student.BatchId;
            TempData["trainerId"] = student.TrainerId;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult SaveStudentMcq(string query)
        {
            try
            {
                query = UtilityHelper.JsonSstmDecrypt(query);
                List<StudentQP_Written> student = JsonConvert.DeserializeObject<List<StudentQP_Written>>(query);

                IEnumerable<StudentQP_Written> getStudentTest = _studentMcqService.GetAllTestCompleteStudent(student.FirstOrDefault().CourseId, student.FirstOrDefault().BatchId, dtformat.GetSingaporeTime());

                bool isIntroExits = getStudentTest.Where(x => x.StudentNo == student[0].StudentNo && x.ChapterId == student[0].ChapterId).Any();
                if (!isIntroExits)
                {
                    string result = _studentMcqService.SaveStudentMCQ(student);
                    if (string.IsNullOrEmpty(result))
                    {
                        return Json(new
                        {
                            Status = HttpStatusCode.InternalServerError,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var data = new
                        {
                            student[0].StudentNo,
                            student[0].ChapterId,
                            student[0].BatchId,
                            Notify = "mcqresult"
                        };
                        WebSocketHub.SendToSstmClient(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(student));
                    }


                    return Json(new { status = true, response = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string result = "Test Already Submitted...";
                    return Json(new { status = false, response = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = HttpStatusCode.InternalServerError,
                    Result = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateIntroPage(string query)
        {
            try
            {
                query = UtilityHelper.JsonSstmDecrypt(query);
                StudentIntroPage student = JsonConvert.DeserializeObject<StudentIntroPage>(query);
                IEnumerable<StudentIntroPage> getIntroCompleted = _createIntropageService.GetAllStudent(student.CourseId, student.BatchId.ToString(), dtformat.GetSingaporeTime());
                bool isIntroExits = getIntroCompleted.Where(x => x.StudentId == student.StudentId).Any();

                if (!isIntroExits)
                {
                    StudentIntroPage result;
                    result = _createIntropageService.CreateIntropage(student);
                    var data = new
                    {
                        TrainerId = student.TrainerId,
                        Notify = "intro"
                    };
                    WebSocketHub.SendToSstmClient(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(result));
                    return Json(new { status = true, response = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string result = "Intro Already Submitted...";
                    return Json(new { status = false, response = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = HttpStatusCode.InternalServerError,
                    Result = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AnswerStudentMCQ(string studentId)
        {
            var query = UtilityHelper.JsonSstmDecrypt(studentId);
            StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(query);
            TempData["studentId"] = student.StudentId;
            TempData["courseId"] = student.CourseId;
            TempData["batchId"] = student.BatchId;

            TempData["chapterId"] = student.ChapterId;
            return View();
        }
        [HttpPost]
        public string StudentImageUpload()
        {
            try
            {
                FileInfo fi = new FileInfo(Request.Files[0].FileName);
                AWSModel awsModel = new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    FileName = string.Concat(dtformat.GetSingaporeTime().Ticks.ToString(), fi.Extension.ToLower()),
                    BucketDirectory = "StudentIntroImages",
                    FilePath = UtilityHelper.amazonlinkUrl,
                    LocalFileStream = Request.Files[0].InputStream
                };
                AWSHelper.UploadFile(awsModel);
                return awsModel.FilePath + awsModel.BucketDirectory + '/' + awsModel.FileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult CongratulationStudents()
        {
            return PartialView();
        }
        [HttpGet]
        public async Task<JsonResult> GetStudentMcqBasedOnCourse(long courseId, long chapterId, string batchId)
        {
            List<StudentQP> result = await _introService.GetStudentMCQ(courseId, chapterId);
            List<StudentMarkList> markResult = _studentMcqService.GetAllStudentMarksList(courseId.ToString(), chapterId.ToString());
            IEnumerable<StudentIntroPage> studentList = _createIntropageService.GetAllStudent(courseId, batchId, dtformat.GetSingaporeTime());

            var notificationStatus = false;

            foreach (var item in studentList)
            {
                notificationStatus = _studentNotification.CheckStudentNotificationExists(item.StudentId);

                if (notificationStatus) break;
            }

            var studentmark_result = from s in markResult
                                     join st in studentList
                                     on s.StudentNo equals st.StudentId.ToString()
                                     where st.CourseId == courseId
                                     select new
                                     {
                                         st.StudentName,
                                         s.PerQuestionMark,
                                         s.TotalMark
                                     };
            var data = new
            {
                AnswerSheet = result,
                MarkResult = studentmark_result,
                NotificationStatus = notificationStatus
            };
            //WebSocketHub.SendToSstmClient("json", JsonConvert.SerializeObject(data));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Feedback(string studentId)
        {
            string studentDetails = UtilityHelper.JsonSstmDecrypt(studentId);

            StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(studentDetails);
            TempData["studentId"] = student.StudentId;
            TempData["studentName"] = student.StudentName;
            TempData["courseId"] = student.CourseId;
            TempData["batchId"] = student.BatchId;
            TempData["Trainerid"] = student.TrainerId;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateFeedBackPage(string query, string TrainerEmailid)
        {
            try
            {
                query = UtilityHelper.JsonSstmDecrypt(query);
                StudentFeedback student = JsonConvert.DeserializeObject<StudentFeedback>(query);
                student.trainerid = TrainerEmailid;
                IEnumerable<StudentFeedback> getStudentFeedback = _feedbackService.GetAllFeedback(student.CourseId, student.BatchId.ToString(), dtformat.GetSingaporeTime());

                bool isIntroExits = getStudentFeedback.Where(x => x.StudentId == student.StudentId).Any();
                if (!isIntroExits)
                {
                    string result = _feedbackService.CreateFeedback(student);
                    WebSocketHub.SendToSstmClient("feedback", JsonConvert.SerializeObject(result));


                    return Json(new { status = true, response = result }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    string result = "Feedback Already Submitted...";
                    return Json(new { status = false, response = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = HttpStatusCode.InternalServerError,
                    Result = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult FeedbackView(string studentId)
        {
            try
            {
                string studentDetails = UtilityHelper.JsonSstmDecrypt(studentId);

                StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(studentDetails);
                TempData["courseId"] = student.CourseId;
                TempData["batchId"] = student.BatchId;
                TempData["date"] = student.DateString;
                return View();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult TrainerSentPoorFeedbackEmail()
        {
            string trainneremailid = "";
            string studentName = "";
            string Batchid = "";
            int counter = 0;

            var sBuilder = new StringBuilder();

            var getStudentFeedback = _feedbackService.GetTodayAllFeedback(DateTime.Now);

            var TrainerEntity = _IUserService.GetDefaultList().Where(a => a.Id == CurrentSession.UserId).FirstOrDefault();

            if (TrainerEntity != null)
                trainneremailid = UtilityHelper.Decrypt(TrainerEntity.Email);
            else
                trainneremailid = "counsel@eversafe.com.sg";

            long CourseId = 0;
            for (int i = 0; i < getStudentFeedback.Count; i++)
            {
                if (i == 0)
                    CourseId = getStudentFeedback[i].CourseId;
            }

            //get course name
            CourseService.SSTM service = new CourseService.SSTM();
            string data = service.AllCourse();
            List<LiCourseDetailModel> model = new List<LiCourseDetailModel>();
            model = (new JavaScriptSerializer()).Deserialize<List<LiCourseDetailModel>>(data);
            var courselist = model.ToList().Where(e => e.Courseid == CourseId);
            var Coursename = courselist.FirstOrDefault();
            // End course name
           
            sBuilder.Append("<table role='presentation'  border='1' width='100%'><thead><th>Name </th><th>Rating Description </th></thead><tbody>");
            foreach (var item in getStudentFeedback)
            {
                sBuilder.Append("<tr><td>" + item.StudentName + "</td><td>" + item.Rating_Description + "</td></tr>");
                Batchid = item.BatchId.ToString();
                //if (counter == 0)
                //{
                //    Batchid = item.BatchId.ToString();
                //    counter++;
                //    studentName =  item.StudentName;
                //}
                //else
                //    studentName += " , " + item.StudentName;
            }
            sBuilder.Append("</tbody></table>");
            try
            {
                if (configEntity != null)
                {
                    var emailBody = UtilityHelper.GetEmailTemplate("PoorFeedbackToTrainer.html").ToString();
                    emailBody = emailBody.Replace("@DearName@", trainneremailid)
                        .Replace("@StudentName@ ", sBuilder.ToString())
                        .Replace("@CourseId@", Coursename.CourseName.ToString())
                        .Replace("@BatchId@", Batchid);
                    EmailHelper.SendMail(new Models.EmailModel.EmailModel
                    {
                        From = configEntity.Email,
                        To = Request.IsLocal ? "teversafe@gmail.com" : "counsel@eversafe.com.sg;" + trainneremailid + "",
                        Subject = "Student poor feedback",
                        Message = emailBody,
                        SMTPHost = configEntity.Host,
                        SMTPPort = configEntity.Port,
                        SMTPEmail = configEntity.Email,
                        SMTPPassword = configEntity.Pass,
                        EnableSsl = configEntity.EnableSsl
                    });
                }
            }
            catch (Exception)
            {
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult SaveStudentRA(string inputQuery)
        {
            try
            {

                if (string.IsNullOrEmpty(inputQuery)) return Json("error", JsonRequestBehavior.AllowGet);

                inputQuery = UtilityHelper.JsonSstmDecrypt(inputQuery);
                List<StudentDetails> student = JsonConvert.DeserializeObject<List<StudentDetails>>(inputQuery);


                if (student.Count == 0) return Json("error", JsonRequestBehavior.AllowGet);

                var checkExists = _riskAssessmentDeclarationService.GetRiskAssessmentDeclaration(Convert.ToInt64(CurrentSession.Trainer_AirLine_id), Convert.ToInt32(student[0].CourseId)
                    , Convert.ToInt32(student[0].BatchId), Convert.ToInt64(student[0].StudentId), "studentcheck");

                if (checkExists.Any())
                {
                    return Json("exists", JsonRequestBehavior.AllowGet);
                }

                RiskAssessmentDeclaration riskAssessmentDeclaration = new RiskAssessmentDeclaration();

                riskAssessmentDeclaration.BatchId = Convert.ToInt32(student[0].BatchId);
                riskAssessmentDeclaration.CourseId = Convert.ToInt32(student[0].CourseId);
                riskAssessmentDeclaration.StudentId = Convert.ToInt64(student[0].StudentId);
                riskAssessmentDeclaration.TrainerId = Convert.ToInt64(student[0].TrainerId);
                riskAssessmentDeclaration.CreatedOn = DateTime.Now;

                _riskAssessmentDeclarationService.SaveRecord(riskAssessmentDeclaration);


                var data = new
                {
                    TrainerId = student[0].TrainerId,
                    Notify = "rasp"
                };
                WebSocketHub.SendToSstmClient(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(riskAssessmentDeclaration));



                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult RADeclarationByStudents(string studentId)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId)) return Content("Invalid Parameter.");

                string studentDetails = UtilityHelper.JsonSstmDecrypt(studentId);

                StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(studentDetails);

                if (student == null) return Content("Invalid Parameter.");

                string courseName = "";

                CourseService.SSTM service = new CourseService.SSTM();
                string dt = dtformat.GetSingaporeTime().ToString("yyyy-MM-dd");
                string data = service.TodayCoursesWithSectionTainerid(dt, Convert.ToInt32(student.TrainerId));
                List<TodayClassDocsModel> model = (new JavaScriptSerializer()).Deserialize<List<TodayClassDocsModel>>(data);

                if (model.Count == 0) return Content("Invalid Parameter.");

                if (model.Any())
                {
                    foreach (var item in model)
                    {
                        if (item.Courseid == Convert.ToInt32(student.CourseId))
                        {
                            courseName = item.coursename;
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(courseName)) return Content("Invalid Parameter.");

                TempData["studentId"] = student.StudentId;
                TempData["studentName"] = student.StudentName;
                TempData["courseName"] = courseName;
                TempData["courseId"] = student.CourseId;
                TempData["batchId"] = student.BatchId;
                TempData["Trainerid"] = student.TrainerId;
                TempData["Date"] = student.Date.ToString("dd-MMM-yyyy");

                return View();
            }
            catch (Exception ex)
            {
                return Content("Exception: " + ex.Message);
            }
        }

    }

    public class LiCourseDetailModel
    {
        public long Courseid { get; set; }
        public string CourseName { get; set; }
    }
    //public class IntroStudentController : Controller
    //{
    //    private readonly IIntroService _introService;
    //    private readonly ICreateIntropageService _createIntropageService;
    //    private readonly IFeedbackService _feedbackService;
    //    private readonly IStudentMcqService _studentMcqService;
    //    private readonly IStudentNotification _studentNotification;
    //    private readonly IConfigService _configService;
    //    private readonly Config configEntity;

    //    public IntroStudentController(IIntroService introService,
    //        IConfigService configService,
    //        ICreateIntropageService createIntropageService,
    //        IFeedbackService feedbackService, IStudentMcqService studentMcqService, IStudentNotification studentNotification)
    //    {
    //        _introService = introService;
    //        _configService = configService;
    //        _createIntropageService = createIntropageService;
    //        _feedbackService = feedbackService;
    //        configEntity = configService.GetFirstRecord();
    //        _studentMcqService = studentMcqService;
    //        _studentNotification = studentNotification;
    //    }

    //    // GET: IntroStudent
    //    public ActionResult Index()
    //    {
    //        return View();
    //    }

    //    [HttpGet]
    //    public ActionResult IntroPage(string studentId)
    //    {

    //        var query = UtilityHelper.JsonSstmDecrypt(studentId);
    //        StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(query);

    //        TempData["studentId"] = student.StudentId;
    //        TempData["studentName"] = student.StudentName;
    //        TempData["courseId"] = student.CourseId;
    //        TempData["batchId"] = student.BatchId;
    //        TempData["trainerId"] = student.TrainerId;

    //        return View();
    //    }

    //    [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
    //    public ActionResult SaveStudentMcq(string query)
    //    {
    //        try
    //        {
    //            query = UtilityHelper.JsonSstmDecrypt(query);
    //            List<StudentQP_Written> student = JsonConvert.DeserializeObject<List<StudentQP_Written>>(query);

    //            IEnumerable<StudentQP_Written> getStudentTest = _studentMcqService.GetAllTestCompleteStudent(student.FirstOrDefault().CourseId, student.FirstOrDefault().BatchId, dtformat.GetSingaporeTime());

    //            bool isIntroExits = getStudentTest.Where(x => x.StudentNo == student.FirstOrDefault().StudentNo).Any();
    //            if (!isIntroExits)
    //            {
    //                string result = _studentMcqService.SaveStudentMCQ(student);
    //                if (string.IsNullOrEmpty(result))
    //                {
    //                    return Json(new
    //                    {
    //                        Status = HttpStatusCode.InternalServerError,
    //                    }, JsonRequestBehavior.AllowGet);
    //                }
    //                return Json(new { status = true, response = result }, JsonRequestBehavior.AllowGet);
    //            }
    //            else
    //            {
    //                string result = "Test Already Submitted...";
    //                return Json(new { status = false, response = result }, JsonRequestBehavior.AllowGet);
    //            }               
    //        }
    //        catch (Exception ex)
    //        {
    //            return Json(new
    //            {
    //                Status = HttpStatusCode.InternalServerError,
    //                Result = ex.Message
    //            }, JsonRequestBehavior.AllowGet);
    //        }
    //    }

    //    [HttpPost, ValidateAntiForgeryToken]
    //    public ActionResult CreateIntroPage(string query)
    //    {
    //        try
    //        {
    //            query = UtilityHelper.JsonSstmDecrypt(query);
    //            StudentIntroPage student = JsonConvert.DeserializeObject<StudentIntroPage>(query);
    //            IEnumerable<StudentIntroPage> getIntroCompleted = _createIntropageService.GetAllStudent(student.CourseId, student.BatchId.ToString(), dtformat.GetSingaporeTime());
    //            bool isIntroExits = getIntroCompleted.Where(x => x.StudentId == student.StudentId).Any();

    //            if (!isIntroExits)
    //            {
    //                StudentIntroPage result;
    //                result = _createIntropageService.CreateIntropage(student);
    //                var data = new
    //                {
    //                    TrainerId = student.TrainerId,
    //                    Notify = "intro"
    //                };
    //                WebSocketHub.SendToSstmClient(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(result));
    //                return Json(new { status = true, response = result }, JsonRequestBehavior.AllowGet);
    //            }
    //            else
    //            {
    //                string result = "Intro Already Submitted...";
    //                return Json(new { status = false, response = result }, JsonRequestBehavior.AllowGet);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return Json(new
    //            {
    //                Status = HttpStatusCode.InternalServerError,
    //                Result = ex.Message
    //            }, JsonRequestBehavior.AllowGet);
    //        }
    //    }

    //    [HttpGet]
    //    public ActionResult AnswerStudentMCQ(string studentId)
    //    {
    //        var query = UtilityHelper.JsonSstmDecrypt(studentId);
    //        StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(query);
    //        TempData["studentId"] = student.StudentId;
    //        TempData["courseId"] = student.CourseId;
    //        TempData["batchId"] = student.BatchId;

    //        TempData["chapterId"] = student.ChapterId;
    //        return View();
    //    }
    //    [HttpPost]
    //    public string StudentImageUpload()
    //    {
    //        try
    //        {
    //            FileInfo fi = new FileInfo(Request.Files[0].FileName);
    //            AWSModel awsModel = new AWSModel()
    //            {
    //                AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
    //                SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
    //                BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
    //                FileName = string.Concat(dtformat.GetSingaporeTime().Ticks.ToString(), fi.Extension.ToLower()),
    //                BucketDirectory = "StudentIntroImages",
    //                FilePath = UtilityHelper.amazonlinkUrl,
    //                LocalFileStream = Request.Files[0].InputStream
    //            };
    //            AWSHelper.UploadFile(awsModel);
    //            return awsModel.FilePath + awsModel.BucketDirectory + '/' + awsModel.FileName;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }
    //    public ActionResult CongratulationStudents()
    //    {
    //        return PartialView();
    //    }
    //    [HttpGet]
    //    public async Task<JsonResult> GetStudentMcqBasedOnCourse(long courseId, long chapterId, string batchId)
    //    {
    //        List<StudentQP> result = await _introService.GetStudentMCQ(courseId, chapterId);
    //        List<StudentMarkList> markResult = _studentMcqService.GetAllStudentMarksList(courseId.ToString(), chapterId.ToString());
    //        IEnumerable<StudentIntroPage> studentList = _createIntropageService.GetAllStudent(courseId, batchId, dtformat.GetSingaporeTime());
    //        var studentmark_result = from s in markResult
    //                                 join st in studentList
    //                                 on s.StudentNo equals st.StudentId.ToString()
    //                                 where st.CourseId == courseId
    //                                 select new
    //                                 {
    //                                     StudentName = st.StudentName,
    //                                     PerQuestionMark = s.PerQuestionMark,
    //                                     TotalMark = s.TotalMark
    //                                 };
    //        var data = new
    //        {
    //            AnswerSheet = result,
    //            MarkResult = studentmark_result
    //        };
    //        WebSocketHub.SendToSstmClient("json", JsonConvert.SerializeObject(data));
    //        return Json(data, JsonRequestBehavior.AllowGet);
    //    }

    //    [HttpGet]
    //    public ActionResult Feedback(string studentId)
    //    {
    //        string studentDetails = UtilityHelper.JsonSstmDecrypt(studentId);

    //        StudentDetails student = JsonConvert.DeserializeObject<StudentDetails>(studentDetails);
    //        TempData["studentId"] = student.StudentId;
    //        TempData["studentName"] = student.StudentName;
    //        TempData["courseId"] = student.CourseId;
    //        TempData["batchId"] = student.BatchId;
    //        return View();
    //    }

    //    [HttpPost, ValidateAntiForgeryToken]
    //    public ActionResult CreateFeedBackPage(string query)
    //    {
    //        try
    //        {
    //            query = UtilityHelper.JsonSstmDecrypt(query);
    //            StudentFeedback student = JsonConvert.DeserializeObject<StudentFeedback>(query);
    //            IEnumerable<StudentFeedback> getStudentFeedback = _feedbackService.GetAllFeedback(student.CourseId, student.BatchId.ToString(), dtformat.GetSingaporeTime());

    //            bool isIntroExits = getStudentFeedback.Where(x => x.StudentId == student.StudentId).Any();
    //            if (!isIntroExits)
    //            {
    //                string result = _feedbackService.CreateFeedback(student);
    //                WebSocketHub.SendToSstmClient("feedback", JsonConvert.SerializeObject(result));
    //                return Json(new { status = true, response = result }, JsonRequestBehavior.AllowGet);

    //            }
    //            else
    //            {
    //                string result = "Feedback Already Submitted...";
    //                return Json(new { status = false, response = result }, JsonRequestBehavior.AllowGet);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return Json(new
    //            {
    //                Status = HttpStatusCode.InternalServerError,
    //                Result = ex.Message
    //            }, JsonRequestBehavior.AllowGet);
    //        }
    //    }
    //}
}