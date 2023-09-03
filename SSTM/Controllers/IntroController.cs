using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SSTM.Business.Interfaces;
using SSTM.Core.IntroPage;
using SSTM.Helpers.App;
using SSTM.Helpers.Common;
using SSTM.Models.AWS;
using SSTM.Models.IntroPage;
using Newtonsoft.Json;
using SSTM.Websockets;
using SSTM.Controllers;
using SSTM.Models;
using System.Web.Script.Serialization;
using SSTM.Models.Course;

namespace SSTM.Controllers
{
    public class IntroController : BaseController
    {
        private readonly IIntroService _introService;
        private readonly ICreateIntropageService _createIntropageService;
        private readonly IFeedbackService _feedbackService;
        private readonly IStudentMcqService _studentMcqService;
        private readonly IStudentNotification _studentNotification;
        private readonly IRiskAssessmentDeclarationService _riskAssessmentDeclarationService;

        DateFormat dtformat;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return Session["AppSession"] as AppSession; }
        }
        private readonly IConfigService _configService;
        private readonly dynamic configEntity;
        public IntroController(IIntroService _introService,
            IConfigService _configService,
            ICreateIntropageService _createIntropageService,
            IFeedbackService _feedbackService, IStudentMcqService _studentMcqService, IStudentNotification _studentNotification
            ,IRiskAssessmentDeclarationService _riskAssessmentDeclarationService)
        {
            this._introService = _introService;
            this._configService = _configService;
            this._createIntropageService = _createIntropageService;
            this._feedbackService = _feedbackService;
            configEntity = _configService.GetFirstRecord();
            this._studentMcqService = _studentMcqService;
            this._studentNotification = _studentNotification;
            this._riskAssessmentDeclarationService = _riskAssessmentDeclarationService;

            dtformat = new DateFormat();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                TrainerSectionDetails getCurrentCourseAndBatch = await _createIntropageService.GetCurrentCourseAndBatch(dtformat.GetSingaporeTime().ToString("yyyy/MM/dd"), CurrentSession.Trainer_AirLine_id.ToString());
                //CurrentSession.CourseId = getCurrentCourseAndBatch.Courseid;
                //CurrentSession.BatchId = getCurrentCourseAndBatch.BatchId;
                //CurrentSession.TrainerCourseId = getCurrentCourseAndBatch.TrainerCourseid;
                //CurrentSession.TrainerBatchId = getCurrentCourseAndBatch.TrainerBatchId;
                string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(CurrentSession.CourseId, CurrentSession.BatchId));
                List<TodayStudentDetails> student = new List<TodayStudentDetails>();
                if (getTodayStudent != "[[]]")
                {
                    student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
                }
                List<StudentIntroPage> getIntroCompleted = _createIntropageService.GetAllStudent(getCurrentCourseAndBatch.Courseid, getCurrentCourseAndBatch.BatchId.ToString(), dtformat.GetSingaporeTime()).ToList();
                int total = student.Where(n => n.Courseid == getCurrentCourseAndBatch.Courseid && n.Batchid == getCurrentCourseAndBatch.BatchId).Count();
                IntroStudentCount introStudent = new IntroStudentCount()
                {
                    IntroCompleted = getIntroCompleted.Count(),
                    IntroPending = total - getIntroCompleted.Count(),
                    TotalStudent = total,
                    TrainerId = CurrentSession.Trainer_AirLine_id.ToString()
                };
                return View(introStudent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetIntroSummary(int courseId, int batchId)
        {
            try
            {
                TrainerSectionDetails getCurrentCourseAndBatch = await _createIntropageService.GetCurrentCourseAndBatch(dtformat.GetSingaporeTime().ToString("yyyy/MM/dd"), CurrentSession.Trainer_AirLine_id.ToString());
                //CurrentSession.CourseId = getCurrentCourseAndBatch.Courseid;
                //CurrentSession.BatchId = getCurrentCourseAndBatch.BatchId;
                //CurrentSession.TrainerCourseId = getCurrentCourseAndBatch.TrainerCourseid;
                //CurrentSession.TrainerBatchId = getCurrentCourseAndBatch.TrainerBatchId;
                string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(courseId, batchId));
                List<TodayStudentDetails> student = new List<TodayStudentDetails>();
                if (getTodayStudent != "[[]]")
                {
                    student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
                }
                List<StudentIntroPage> getIntroCompleted = _createIntropageService.GetAllStudent(courseId, batchId.ToString(), dtformat.GetSingaporeTime()).ToList();
                int total = student.Where(n => n.Courseid == courseId && n.Batchid == batchId).Count();
                IntroStudentCount introStudent = new IntroStudentCount()
                {
                    IntroCompleted = getIntroCompleted.Count(),
                    IntroPending = total - getIntroCompleted.Count(),
                    TotalStudent = total,
                    TrainerId = CurrentSession.Trainer_AirLine_id.ToString()
                };



                return Json(new { Result =  introStudent }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetTrainerDetails()
        {
            try
            {
                TrainerDetails result = await _createIntropageService.GetTrainerDetails(CurrentSession.TrainerCourseId, CurrentSession.TrainerBatchId, (long)CurrentSession.Trainer_AirLine_id);
                if (result == null)
                {
                    return Json(new { Status = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = HttpStatusCode.InternalServerError, Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetTrainerDetails(int courseId, int batchId)
        {
            try
            {
                TrainerDetails result = await _createIntropageService.GetTrainerDetails(Convert.ToInt64(courseId), Convert.ToInt64(batchId), (long)CurrentSession.Trainer_AirLine_id);
                if (result == null)
                {
                    return Json(new { Status = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }

                CurrentSession = (AppSession)Session["AppSession"];
                CurrentSession.Photo = result.Photopath;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = HttpStatusCode.InternalServerError, Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retireve course from api's
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetAllCourse()
        {
            try
            {
                object result = null;
                List<AirlineCourseModel> course = new List<AirlineCourseModel>();
                List<AirlineCourseModel> batch = new List<AirlineCourseModel>();               

                CurrentSession = (AppSession)Session["AppSession"];

                if (CurrentSession.UserRole != "Trainer")
                {
                    result = await _introService.GetAllCourse();
                }
                else
                {
                    CourseService.SSTM service = new CourseService.SSTM();
                    List<TodayClassDocsModel> TodayClass_data = new List<TodayClassDocsModel>();
                    string dt = dtformat.GetSingaporeTime().ToString("yyyy-MM-dd");
                    string data = service.TodayCoursesWithSectionTainerid(dt, Convert.ToInt32(CurrentSession.Trainer_AirLine_id));
                    List<TodayClassDocsModel> model = (new JavaScriptSerializer()).Deserialize<List<TodayClassDocsModel>>(data);

                    

                    if (model.Any())
                    {
                        foreach (var item in model)
                        {
                            var airlineCourseModel = new AirlineCourseModel();
                            airlineCourseModel.CourseId = (int)item.Courseid;
                            airlineCourseModel.CourseName = item.coursename;
                            airlineCourseModel.BatchId = (int)item.batchid;                           

                            var isExist = course.Where(x => x.CourseId == airlineCourseModel.CourseId).ToList();

                            if (!isExist.Any())
                            {
                                course.Add(airlineCourseModel);
                            }
                        }
                    }
                    result = JsonConvert.SerializeObject(course);

                    var courseId = course[0].CourseId;

                    batch = course.Where(x => x.CourseId == courseId).ToList();
                }

                if (result == null)
                {
                    return Json(new { Status = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }

                if (CurrentSession.UserRole != "Trainer")
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Status = HttpStatusCode.OK, Result = result, Batchs = batch }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = HttpStatusCode.InternalServerError, Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetBatchesBasedOnCourseId(int courseId)
        {
            try
            {
                object result = null;
                List<AirlineCourseModel> course = new List<AirlineCourseModel>();
                List<AirlineCourseModel> batch = new List<AirlineCourseModel>();

                CurrentSession = (AppSession)Session["AppSession"];

                if (CurrentSession.UserRole != "Trainer")
                {
                    result = await _introService.GetAllCourse();
                }
                else
                {
                    CourseService.SSTM service = new CourseService.SSTM();
                    List<TodayClassDocsModel> TodayClass_data = new List<TodayClassDocsModel>();
                    string dt = dtformat.GetSingaporeTime().ToString("yyyy-MM-dd");
                    string data = service.TodayCoursesWithSectionTainerid(dt, Convert.ToInt32(CurrentSession.Trainer_AirLine_id));
                    List<TodayClassDocsModel> model = (new JavaScriptSerializer()).Deserialize<List<TodayClassDocsModel>>(data);

                    if (model.Any())
                    {
                        foreach (var item in model)
                        {
                            var airlineCourseModel = new AirlineCourseModel();
                            airlineCourseModel.CourseId = (int)item.Courseid;
                            airlineCourseModel.CourseName = item.coursename;
                            airlineCourseModel.BatchId = (int)item.batchid;

                            var isExist = course.Where(x => x.CourseId == airlineCourseModel.CourseId).ToList();

                            if (!isExist.Any())
                            {
                                course.Add(airlineCourseModel);
                            }
                        }
                    }
                    result = JsonConvert.SerializeObject(course);                    

                    batch = course.Where(x => x.CourseId == courseId).ToList();
                }

                if (result == null)
                {
                    return Json(new { Status = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { Status = HttpStatusCode.OK, Batchs = batch }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = HttpStatusCode.InternalServerError, Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GetAllIntroStudent(int courseId, int batchId)
        {
            try
            {
                //IEnumerable<StudentIntroPage> result = _createIntropageService.GetAllStudent(CurrentSession.CourseId, CurrentSession.BatchId.ToString(), dtformat.GetSingaporeTime());

                IEnumerable<StudentIntroPage> result = _createIntropageService.GetAllStudent(Convert.ToInt64(courseId), batchId.ToString(), dtformat.GetSingaporeTime());
                if (result == null)
                {
                    return Json(new { Status = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = HttpStatusCode.InternalServerError, Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult CreateStudentMCQ()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateStudentMCQ(string query)
        {
            try
            {
                //query = UtilityHelper.JsonSstmDecrypt(query);
                List<StudentQP> student = JsonConvert.DeserializeObject<List<StudentQP>>(query);
                List<string> deletedQp = student.Where(x => !(bool)x.IsQp && !string.IsNullOrEmpty(x.Qp_Doc_Name)).Select(x => x.Qp_Doc_Name).ToList();
                if (deletedQp.Any())
                {
                    DeleteStudentMCQ(deletedQp);
                }
                string result = _introService.AddStudentMultipleChoiceQp(student);
                return Json(result, JsonRequestBehavior.AllowGet);
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
        public ActionResult ViewStudentMCQ()
        {
            ViewBag.BatchId = CurrentSession.BatchId;
            return View();
        }


        [HttpGet]
        public ActionResult StudentOverallMarkSummary()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAllTestCompleteStudent(string courseId, string batchId)
        {
            List<StudentQP_Written> getTestStudent = _studentMcqService.GetAllTestCompleteStudent(Convert.ToInt64(courseId), Convert.ToInt64(batchId), dtformat.GetSingaporeTime()).ToList();

            //var getStudent = getTestStudent.Where(x => x.StudentNo == studentId).ToList();

            return Json(getTestStudent, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetOverAllStudentMarksList(string courseId, string chapterId, string batchId)
        {
            try
            {
                List<StudentMarkList> markResult = _studentMcqService.GetOverAllStudentMarksList(courseId, chapterId);
                IEnumerable<StudentIntroPage> studentList = _createIntropageService.GetAllStudent(Convert.ToInt64(courseId), batchId, dtformat.GetSingaporeTime());

                //if (studentList.Count() == 0)
                //{
                //    string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(Convert.ToInt64(courseId), CurrentSession.BatchId));
                //    List<TodayStudentDetails> student = new List<TodayStudentDetails>();
                //    if (getTodayStudent != "[[]]")
                //    {
                //        student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
                //    }

                //    var studentmark_result1 = from s in markResult
                //                              join st in student
                //                              on s.StudentNo equals st.StudentId.ToString()
                //                              where st.Courseid == Convert.ToInt64(courseId)
                //                              select new
                //                              {
                //                                  StudentName = st.Studentname,
                //                                  s.TotalCorrectMark,
                //                                  s.TotalMark,
                //                                  s.Photo,
                //                                  s.StudentNo
                //                              };
                //    return Json(studentmark_result1, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{

                    var studentmark_result = from s in markResult
                                             join st in studentList
                                             on s.StudentNo equals st.StudentId.ToString()
                                             where st.CourseId == Convert.ToInt64(courseId)
                                             select new
                                             {
                                                 st.StudentName,
                                                 s.TotalCorrectMark,
                                                 s.TotalMark,
                                                 Photo = st.StudentImage,
                                                 StudentNo = st.StudentId
                                                 
                                             };
                

                return Json(studentmark_result, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult UploadStudentMCQ()
        {
            try
            {
                dynamic file = Request.Files[0];
                AWSModel awsModel = new AWSModel();
                if (file != null)
                {
                    FileInfo fi = new FileInfo(Request.Files[0].FileName);
                    awsModel = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        FileName = string.Concat(dtformat.GetSingaporeTime().Ticks.ToString(), fi.Extension.ToLower()),
                        BucketDirectory = "StudentMCQPapers",
                        FilePath = UtilityHelper.amazonlinkUrl,
                        LocalFileStream = Request.Files[0].InputStream
                    };
                    AWSHelper.UploadFile(awsModel);
                }
                return Json(new { Status = HttpStatusCode.OK, Result = awsModel.FilePath + awsModel.BucketDirectory + '/' + awsModel.FileName, Doc_Name = awsModel.FileName });
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
        [HttpPost]
        public JsonResult DeleteStudentMCQ(List<string> fileName)
        {
            try
            {
                bool isDeleted = false;
                if (fileName.Any())
                {
                    foreach (string file in fileName)
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            AWSModel awsModel = new AWSModel()
                            {
                                AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                                SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                                BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                                FileName = file,
                                BucketDirectory = "StudentMCQPapers",
                            };
                            isDeleted = AWSHelper.DeleteFile(awsModel);
                        }
                    }
                    if (isDeleted)
                    {
                        return Json(new { Status = HttpStatusCode.OK, Result = "deleted", Path = string.Empty });
                    }
                }
                return Json(new { Status = HttpStatusCode.NotFound, Result = "not deleted" });
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
        public async Task<ActionResult> GetAllFeedback(string courseId, string batchId, string date)
        {
            try
            {
                FeedbackViewDetails feedbackView = new FeedbackViewDetails();
                List<StudentFeedback> result = _feedbackService.GetAllFeedback(Convert.ToInt64(courseId), batchId, Convert.ToDateTime(date)).ToList();
                IEnumerable<StudentIntroPage> getStud = _createIntropageService.GetAllStudent(Convert.ToInt64(courseId), batchId.ToString(), Convert.ToDateTime(date));

                string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(Convert.ToInt64(courseId), Convert.ToInt64(batchId)));
                List<TodayStudentDetails> student = new List<TodayStudentDetails>();
                if (getTodayStudent != "[[]]")
                {
                    student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
                }
                FeedbackView feedback = new FeedbackView
                {
                    TotalStudent = student.Count().ToString(),
                    FeedbackCompletedStudent = result.Count().ToString(),
                    FeedbackPendingStudent = (student.Count() - result.Count()).ToString()
                };
                if (result.Count() != 0)
                {
                    feedback.Rating1 = result.Where(x => x.Rating == "1").Count().ToString();
                    feedback.Rating2 = result.Where(x => x.Rating == "2").Count().ToString();
                    feedback.Rating3 = result.Where(x => x.Rating == "3").Count().ToString();
                }
                else
                {
                    feedback.Rating1 = "0";
                    feedback.Rating2 = "0";
                    feedback.Rating3 = "0";
                }
                List<AlterStudentIntroPage> introPageFeedback = new List<AlterStudentIntroPage>();
                List<long> studentId = new List<long>();
                studentId = result.Select(x => x.StudentId).ToList();
                foreach (var item in getStud.Where(s => studentId.Contains(s.StudentId)))
                {
                    AlterStudentIntroPage alterStudent = new AlterStudentIntroPage()
                    {
                        IsActive = item.IsActive,
                        Rating = result.Where(x => x.StudentId == item.StudentId).FirstOrDefault().Rating,
                        CreatedOn = item.CreatedOn,
                        BatchId = item.BatchId,
                        CompanyName = item.CompanyName,
                        CourseId = item.CourseId,
                        IndustryType = item.IndustryType,
                        PurposeOfStudy = item.PurposeOfStudy,
                        Qualification = item.Qualification,
                        StudentId = item.StudentId,
                        StudentImage = item.StudentImage,
                        StudentIntroPageId = item.StudentIntroPageId,
                        StudentName = item.StudentName
                    };
                    introPageFeedback.Add(alterStudent);
                }
                feedbackView.FeedbackView = feedback;
                feedbackView.StudentFeedback = introPageFeedback;

                if (result == null)
                {
                    return Json(new { Status = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }
                return Json(feedbackView, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = HttpStatusCode.InternalServerError, Result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FeedbackStudentView()
        {
            TempData["courseId"] = CurrentSession.CourseId;
            TempData["batchId"] = CurrentSession.BatchId;
            TempData["date"] = dtformat.GetSingaporeTime();
            return View();
        }
        public ActionResult ViewAnswerSheet()
        {
            TempData["batchId"] = CurrentSession.BatchId;
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> SendNotification(string windowLocation, int courseId, int batchId)
        {
            string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(courseId, batchId));
            List<TodayStudentDetails> student = new List<TodayStudentDetails>();
            if (getTodayStudent != "[[]]")
            {
                student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
            }
            windowLocation += "/IntroStudent/intropage?studentId=";

            List<StudentIntroPage> getIntroStudent = _createIntropageService.GetAllStudent(courseId, batchId.ToString(), dtformat.GetSingaporeTime()).ToList();
            List<long> getStudentId = new List<long>();
            if (getIntroStudent.Any())
            {
                getStudentId = getIntroStudent.Select(x => x.StudentId).ToList();
            }
            student = student.Where(x => !getStudentId.Contains(x.StudentId)).ToList();
            string message = string.Empty;
            dynamic result = _createIntropageService.SendNotification(student, windowLocation, "Intro Page Notification", message + "\n\n", CurrentSession.Trainer_AirLine_id.ToString());
            NotificationResponse noti = JsonConvert.DeserializeObject<NotificationResponse>(result);
            if (noti.SaveNotification != null)
            {
                _studentNotification.SaveStudentNotification(noti.SaveNotification);
            }
            return Json(noti.Notification, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> InstantFeedbackNotification(string windowLocation)
        {
            try
            {
                string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(CurrentSession.CourseId, CurrentSession.BatchId));
                List<StudentIntroPage> getIntroStudent = _createIntropageService.GetAllStudent(CurrentSession.CourseId, CurrentSession.BatchId.ToString(), dtformat.GetSingaporeTime()).ToList();
                List<StudentFeedback> studentFeedbacks = _feedbackService.GetAllFeedback(CurrentSession.CourseId, CurrentSession.BatchId.ToString(), dtformat.GetSingaporeTime()).ToList();

                List<long> getStudentId = new List<long>();
                if (getIntroStudent.Any())
                {
                    getStudentId = getIntroStudent.Select(x => x.StudentId).ToList();
                }
                List<long> getFeedbackStudentId = new List<long>();
                if (getIntroStudent.Any())
                {
                    getFeedbackStudentId = studentFeedbacks.Select(x => x.StudentId).ToList();
                }
                List<TodayStudentDetails> student = new List<TodayStudentDetails>();
                if (getTodayStudent != "[[]]")
                {
                    student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
                }
                student = student.Where(x => getStudentId.Contains(x.StudentId) && !getFeedbackStudentId.Contains(x.StudentId)).ToList();
                windowLocation += "/IntroStudent/feedback?studentId=";
                string message = "<b>Dear (Student_Name)<b><br/><br/>Welcome to Eversafe's Instant performance reflection(IPR) session.<br/> Please click the below link to participant in the Intro Page, <br/>Instant Testing and Instant Feedback Link : ";
                dynamic result = _createIntropageService.SendNotification(student, windowLocation, "Feedback Page Notification", message + "\n\n", CurrentSession.Trainer_AirLine_id.ToString());
                NotificationResponse noti = JsonConvert.DeserializeObject<NotificationResponse>(result);
                _studentNotification.SaveStudentNotification(noti.SaveNotification);
                return Json("Feedback Notification sent...", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public async Task<ActionResult> InstantTestNotification(string windowLocation, string courseId, string chapterId, string batchId)
        {
            try
            {
                //string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(CurrentSession.CourseId, CurrentSession.BatchId));
                //List<TodayStudentDetails> student = new List<TodayStudentDetails>();

                string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(Convert.ToInt64(courseId), Convert.ToInt64(batchId)));
                List<StudentIntroPage> getIntroStudent = _createIntropageService.GetAllStudent(Convert.ToInt64(courseId), batchId.ToString()).ToList();
                List<StudentQP_Written> getTestStudent = _studentMcqService.GetAllTestCompleteStudent(Convert.ToInt64(courseId), Convert.ToInt64(batchId), dtformat.GetSingaporeTime()).ToList();

                List<long> getStudentId = new List<long>();
                if (getIntroStudent.Any())
                {
                    getStudentId = getIntroStudent.Select(x => x.StudentId).ToList();
                }
                List<string> getTestStudentId = new List<string>();
                if (getIntroStudent.Any())
                {
                    getTestStudentId = getTestStudent.Select(x => x.StudentNo).ToList();
                }
                List<TodayStudentDetails> student = new List<TodayStudentDetails>();
                if (getTodayStudent != "[[]]")
                {
                    student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);                   
                }
                student = student.Where(x => getStudentId.Contains(x.StudentId)).ToList(); // && !getTestStudentId.Contains(x.StudentId.ToString()

                if (!student.Any())
                {
                    student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
                }

                if (getTodayStudent != "[[]]")
                {

                    student.ForEach(x =>
                    {
                        x.ChapterId = chapterId;
                        x.Courseid = Convert.ToInt64(courseId);
                    });
                }
                windowLocation += "/IntroStudent/AnswerStudentMCQ?studentId=";
                dynamic result = _createIntropageService.SendNotification(student, windowLocation, "Instant testing Page Notification", "Please Try this test questions(click the below link to try your test)" + "\n\n", CurrentSession.Trainer_AirLine_id.ToString());
                NotificationResponse noti = JsonConvert.DeserializeObject<NotificationResponse>(result);
                _studentNotification.SaveStudentNotification(noti.SaveNotification);
                return Json("Instant Testing Notification sent...", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult FeedbackSendNotify(string windowLocation, string classDet)
        {
            try
            {
                windowLocation = windowLocation + "/IntroStudent/FeedbackView?studentId=" + classDet;

                _createIntropageService.SendNotificationFromEmail("", windowLocation, "Instant Feedback Review", "Instant Feedback Review Notification Please Check(click the below link)" + "\n\n\n\n");

                return Json("Feedback Notification Submited...", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ViewBlendedLearning()
        {
            return View();
        }
        
        public ActionResult RedirectLi(string courseId)
        {
            string url1 = "https://li.eversafe.com.sg/StudentDisplay.aspx?attid=0&courseid={0}";
            var finalurl = string.Format(url1, courseId);

            return Json(new { url = finalurl });

            //Response.Redirect(finalurl);
        }       

        public ActionResult AssignBatch(string courseId, string batchId)
        {
            CurrentSession = (AppSession)Session["AppSession"];

            CurrentSession.CourseId =Convert.ToInt64(batchId);

            CurrentSession.BatchId = Convert.ToInt64(batchId);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RiskAssessment(string requestType)
        {
            ViewBag.RequestType = requestType;
            return View();
        }

        public ActionResult GetRiskAssessmentDeclaration(int courseId, int batchId, string filter="all")
        {
            CurrentSession = (AppSession)Session["AppSession"];

            var result = _riskAssessmentDeclarationService.GetRiskAssessmentDeclaration(Convert.ToInt64(CurrentSession.Trainer_AirLine_id), courseId, batchId, 0, filter);

            CourseService.SSTM service = new CourseService.SSTM();
            string dt = dtformat.GetSingaporeTime().ToString("yyyy-MM-dd");
            string data = service.TodayCoursesWithSectionTainerid(dt, Convert.ToInt32(CurrentSession.Trainer_AirLine_id));
            List<TodayClassDocsModel> model = (new JavaScriptSerializer()).Deserialize<List<TodayClassDocsModel>>(data);

            var courseName = "";

            if (model.Any())
            {
                foreach (var item in model)
                {
                    if (item.Courseid == courseId)
                    {
                        courseName = item.coursename;
                        break;
                    }
                }
            }

            if (result.Any())
            {
                foreach(var item in result)
                {
                    if(item.Id == Convert.ToInt64(CurrentSession.Trainer_AirLine_id))
                    {
                        item.Name = CurrentSession.UserName;
                        item.Image = CurrentSession.Photo;                        
                    }

                    item.CourseName = courseName;
                    item.CreatedOnString = item.CreatedOn.ToString("dd-MMM-yyyy HH:mm");
                }
            }

            return Json( new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        [ActionName("GetBatchIdsFromRA")]
        public ActionResult GetRiskAssessmentDeclaration()
        {
            var result = _riskAssessmentDeclarationService.GetRiskAssessmentDeclaration(0, 0, 0, 0, "batchonly");           

            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveRiskAssessmentDeclaration(int courseId, int batchId)
        {

            var checkExists = _riskAssessmentDeclarationService.GetRiskAssessmentDeclaration(Convert.ToInt64(CurrentSession.Trainer_AirLine_id), courseId, batchId, 0, "trainercheck");

            if(checkExists.Any())
            {
                return Json("exists", JsonRequestBehavior.AllowGet);
            }

            CurrentSession = (AppSession)Session["AppSession"];

            RiskAssessmentDeclaration riskAssessmentDeclaration = new RiskAssessmentDeclaration();

            riskAssessmentDeclaration.BatchId = batchId;
            riskAssessmentDeclaration.CourseId = courseId;
            riskAssessmentDeclaration.TrainerId = CurrentSession.Trainer_AirLine_id;
            riskAssessmentDeclaration.CreatedOn = DateTime.Now;

            _riskAssessmentDeclarationService.SaveRecord(riskAssessmentDeclaration);

            try
            {

                CourseService.SSTM service = new CourseService.SSTM();
                //List<TodayClassDocsModel> TodayClass_data = new List<TodayClassDocsModel>();
                //string dt = dtformat.GetSingaporeTime().ToString("yyyy-MM-dd");
                //string data = service.(dt, Convert.ToInt32(CurrentSession.Trainer_AirLine_id));

                var resultLi = service.RAandSWPDeclaration("insert", courseId, batchId, 0, (int)CurrentSession.Trainer_AirLine_id, DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch(Exception ex)
            {
                throw ex;
            }


            var result = "success";

            return Json(result, JsonRequestBehavior.AllowGet);
        }        

        public async Task<ActionResult> SendRANotification(int courseId, int batchId, string windowLocation)
        {
            string getTodayStudent = (await _createIntropageService.GetAllTodayStudent(courseId, batchId));
            List<TodayStudentDetails> student = new List<TodayStudentDetails>();
            if (getTodayStudent != "[[]]")
            {
                student = JsonConvert.DeserializeObject<List<TodayStudentDetails>>(getTodayStudent);
            }            

            //List<StudentIntroPage> getIntroStudent = _createIntropageService.GetAllTodayStudent(courseId, batchId).ToList();
            //List<long> getStudentId = new List<long>();
            //if (getIntroStudent.Any())
            //{
            //    getStudentId = getIntroStudent.Select(x => x.StudentId).ToList();
            //}

            //student = student.Where(x => getStudentId.Contains(x.StudentId)).ToList();            

            windowLocation += "/introstudent/RADeclarationByStudents?studentId=";            
                   
            string message = string.Empty;
            dynamic result = _createIntropageService.SendRANotification(student, windowLocation, "RA and SWP Acknowledgement", message + "\n\n", CurrentSession.Trainer_AirLine_id.ToString());
            //dynamic result = _createIntropageService.SendRANotification(lst, windowLocation, "Risk Assessment and Safer Procedure Acknowledgement", message + "\n\n", CurrentSession.Trainer_AirLine_id.ToString());
            NotificationResponse noti = JsonConvert.DeserializeObject<NotificationResponse>(result);
            if (noti.SaveNotification != null)
            {
                _studentNotification.SaveStudentNotification(noti.SaveNotification);
            }

            var trainer = GetTrainerDetails(courseId, batchId);

            return Json(new { Notify = noti.Notification, Students = student, Trainer = trainer, Date = DateTime.Now.ToString("dd-MMM-yyyy HH:mm") }, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult OpenRASWP()
        {
            return PartialView();
           
        }
    }
}