using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
using System.Web.Script.Serialization;
using SSTM.Filters;
using SSTM.Business.Interfaces;
using SSTM.Helpers.App;
using SSTM.Helpers.Common;
using System.IO;
using SSTM.Core.Assessment_Paper;
using SSTM.Models.Assessment_Paper;
using SSTM.Helpers.AutoMapping;
using System.Text;
using SSTM.Models.Zoho;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler, MvcApplication.AllowCrossSite]
    public class AssessmentPaperController : BaseController
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
        private readonly IAssessmentPaperService _IAssessmentPaperService;
        private readonly IDeveloperMonitorTimerService _IDeveloperMonitorTimerService;
        private static string documentPath = "";
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public AssessmentPaperController
            (IExceptionLogService exceptionLogService, IActivityLogService activityLogService, IConfigService configService,
            IUserService userService, IRoleService roleService, ICourseStatusService courseStatusService, ICourseService courseService,
            ICourseAssignmentService courseAssignmentService, ICourseDocumentService courseDocumentService,
            ICourseDocVersionService courseDocVersionService, ICourseDocRemarksService courseDocRemarksService,
            ICourseSharingService courseSharingService, IMainCourseService MainCourseService,
            ISubCourseService SubCourseService, ICourseTrackersService CourseTrackersService,
             ICourseDownloadUserService CourseDownloadUserService, IAssessmentPaperService IAssessmentPaperService,
             IDeveloperMonitorTimerService DeveloperMonitorTimerService)
        {

            _IDeveloperMonitorTimerService = DeveloperMonitorTimerService;
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
            _IAssessmentPaperService = IAssessmentPaperService;
        }
        #endregion
        // GET: AssessmentPaper
        public ActionResult Index()
        {
            ViewBag.rolename = CurrentSession.UserRole;
            return View();
        }
        public ActionResult Student_list(string course_id, string batch_id)
        {
            ViewBag.courseid = course_id;
            ViewBag.batchid = batch_id;
            return View();
        }
        public ActionResult Student_assement_paper_pdf_save(string pdf, string filename, int courseid, float batchid, string fin, List<student_result_data> finalResult)
        {
            string path = Server.MapPath("~/Content/assement_paper");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            System.Byte[] byteArray = null;
            using (System.IO.FileStream stream = System.IO.File.Create(path + "\\" + filename))
            {
                byteArray = System.Convert.FromBase64String(pdf.Split(',')[1]);
                stream.Write(byteArray, 0, byteArray.Length);

                var configEntity = _IConfigService.GetFirstRecord();
                AWSModel awsModel = new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = "AssementPaper/" + courseid.ToString(),
                    FileName = filename,
                    LocalFileStream = stream
                };
                AWSHelper.UploadFile(awsModel);

            }
            CourseService.SSTM service = new SSTM.CourseService.SSTM();

            foreach (var item in finalResult)
            {
                string data = service.insertStudentMarks(courseid, batchid, fin.Trim(), item.division.Trim(), item.marks, item.result.Trim());
            }

            #region Save record into Course Document table

            var courseDocEntity = _IAssessmentPaperService.isexist_record(courseid, Convert.ToDecimal(batchid), fin.Trim());
            if (courseDocEntity != null)
            {
                courseDocEntity.UpdatedBy = CurrentSession.UserId;
                courseDocEntity.UpdatedOn = DateTime.Now;
                courseDocEntity.courseid = courseid;
                courseDocEntity.batchid = Convert.ToDecimal(batchid);
                courseDocEntity.qty = 0;
                courseDocEntity.course_name = "";
                courseDocEntity.student_fin = fin.Trim();
                courseDocEntity.batch_name = batchid.ToString();
                courseDocEntity.filename = filename.Trim();
            }
            else
            {
                courseDocEntity = new AssessmentPaperModel
                {
                    courseid = courseid,
                    batchid = Convert.ToDecimal(batchid),
                    qty = 0,
                    course_name = "",
                    student_fin = fin.Trim(),
                    batch_name = batchid.ToString(),
                    filename = filename.Trim(),
                    CreatedBy = CurrentSession.UserId,
                    CreatedOn = DateTime.Now
                }.ToEntity();
            }
            var newId = _IAssessmentPaperService.SaveRecord(courseDocEntity);
            #endregion
            return null;
        }

        public ActionResult Student_assement_paper(string course_id, string batch_id, string fin)
        {
            ViewBag.courseid = course_id;
            ViewBag.batchid = batch_id;
            ViewBag.fin = fin.Trim();
            //  fin = "G8119829U";
            string filename = course_id + "_" + batch_id + "_" + fin.Trim() + ".pdf";
            ViewBag.filename = filename;
            var download_pdf_file = Server.MapPath("~/Content/assement_paper/" + filename);
            //if(System.IO.File.Exists(download_pdf_file))
            //{
            //    ViewBag.filepath = download_pdf_file;
            //}
            //else
            //{
            var get_files = StudentExamFile(course_id, batch_id, fin.Trim());
            int i = 0;
            foreach (var item in get_files)
            {
                if (string.IsNullOrEmpty(item.pdf))
                {
                    if (i == 0)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            ViewBag.filepath = item.Files;
                            // webClient.DownloadFile(item.Files, download_pdf_file);
                        }
                    }
                }
                else
                {
                    ViewBag.filepath = "";
                }
                i++;
            }

            //}
            return View();
        }

        public ActionResult Student_assement_paper_view(string course_id, string batch_id, string fin)
        {
            ViewBag.courseid = course_id;
            ViewBag.batchid = batch_id;
            ViewBag.fin = fin.Trim();
            //fin = "G8119829U";

            List<get_student_result_data> data = new List<get_student_result_data>();
            data = Get_StudentExammarks(course_id, batch_id, fin.Trim());
            var courseDocEntity = _IAssessmentPaperService.isexist_record(Convert.ToInt32(course_id), Convert.ToDecimal(batch_id), fin.Trim().ToLower());
            if (courseDocEntity != null)
            {
                string awsurl = Request.IsLocal ? UtilityHelper.testamazonlinkUrl + "AssementPaper/" + course_id + "/" + courseDocEntity.filename + "#toolbar=0" : UtilityHelper.amazonlinkUrl + "AssementPaper/" + course_id + "/" + courseDocEntity.filename + "#toolbar=0";
                ViewBag.filename = awsurl;
            }
            else
                ViewBag.filename = "";

            return View(data);
        }

        public string downloadfile(string course_id, string batch_id, string fin)
        {
            string filename = course_id + "_" + batch_id + "_" + fin.Trim() + ".pdf";
            var download_pdf_file = Server.MapPath("~/Content/assement_paper/" + filename);
            return filename;
        }
        public JsonResult batchid(int courseid)
        {
            try
            {
                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                string data = service.AllBatchidFromCourse(courseid);
                List<batchdata> AirlineCourseModel = (new JavaScriptSerializer()).Deserialize<List<batchdata>>(data);

                return Json(AirlineCourseModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var list = new List<SelectListItem>();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        public List<student_file> StudentExamFile(string courseid, string batchid, string fin)
        {
            try
            {
                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                string data = service.GetStudentExamFile(Convert.ToInt32(courseid), float.Parse(batchid), fin.Trim());
                List<student_file> filedata = (new JavaScriptSerializer()).Deserialize<List<student_file>>(data);
                return filedata;
            }
            catch (Exception ex)
            {
                List<student_file> list = new List<student_file>();
                return list;
            }
        }

        public JsonResult StudentExamFileJson(string courseid, string batchid, string fin)
        {
            try
            {
                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                string data = service.GetStudentExamFile(Convert.ToInt32(courseid), float.Parse(batchid), fin.Trim());
                List<student_file> filedata = (new JavaScriptSerializer()).Deserialize<List<student_file>>(data);
                return Json(filedata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                List<student_file> list = new List<student_file>();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        public List<get_student_result_data> Get_StudentExammarks(string courseid, string batchid, string fin)
        {
            try
            {
                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                string data = service.GetStudentMarks(Convert.ToInt32(courseid), float.Parse(batchid), fin.Trim());
                List<get_student_result_data> filedata = (new JavaScriptSerializer()).Deserialize<List<get_student_result_data>>(data);
                return filedata;
            }
            catch (Exception ex)
            {
                List<get_student_result_data> list = new List<get_student_result_data>();
                return list;
            }
        }

        public JsonResult trainerlist()
        {
            try
            {
                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                string data = service.AllTrainer();
                List<trainerdata> AirlineCourseModel = (new JavaScriptSerializer()).Deserialize<List<trainerdata>>(data);

                return Json(AirlineCourseModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var list = new List<SelectListItem>();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Courselist()
        {
            try
            {
                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                string data = service.AllCourse();
                List<AirlineCourseModel> AirlineCourseModel = (new JavaScriptSerializer()).Deserialize<List<AirlineCourseModel>>(data);

                return Json(AirlineCourseModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var list = new List<SelectListItem>();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetAssessment(long id)
        {
            try
            {
                var model = _IAssessmentPaperService.GetRecordById(id).ToModel();
                try
                {
                    if (model == null)
                        model = new AssessmentPaperModel();
                }
                catch (Exception)
                {
                    //var list = new List<SelectListItem>();
                    //TempData["AirlineCourse"] = new SelectList(list, "Value", "Text");
                }
                return PartialView("_AddOrUpdate", model);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "AssessmentPaper", "AddUpdateform", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        [HttpGet]
        public ActionResult GetAssessmentList(string date)
        {
            var sBuilder = new StringBuilder();
            try
            {
                if (date == "")
                {
                    date = DateTime.Now.ToString();
                }
                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                //string data = service.AnswerSheetCorectingTrainerbydate(date);

                //List<trainer> list = (new JavaScriptSerializer()).Deserialize<List<trainer>>(data);
                var list = _IAssessmentPaperService.GetRecordlist();
                list = list.Where(a => a.CreatedOn.ToString("dd/MM/yyyy") == Convert.ToDateTime(date).ToString("dd/MM/yyyy")).ToList();
                if (CurrentSession.UserRole == "CI")
                {
                    //list.Where(a => a.trainer_id.Trim() == CurrentSession.Trainer_AirLine_id.ToString().Trim());
                    //if (string.IsNullOrEmpty(CurrentSession.Trainer_AirLine_id.ToString()) || CurrentSession.Trainer_AirLine_id != 0)
                    //{
                    // var list = _IAssessmentPaperService.GetRecordlist();
                    var actions =
                           "<button type='button' title='Edit' class='btn btn-primary btn-sm btnEdit'><i class='fa fa-pen'></i></button>&nbsp;" +
                           "<button type='button' title='Delete' class='btn btn-danger btn-sm btnDelete'><i class='fa fa-trash'></i></button>";
                    foreach (var item in list)
                    {
                        //if (item.trainerid != 0 && item.trainerid == CurrentSession.Trainer_AirLine_id)
                        //{

                        var Studentlist = "<a href='/AssessmentPaper/Student_list/?course_id=" + item.courseid + "&batch_id=" + item.batchid + "' class='btn btn-info btn-sm Download'>Student list</a>";
                        //string awsurl = "<a href='/AssessmentPaper/GetCourseDocFile/?d=" + item.trainerid + "' class='btn btn-info btn - sm Download'>View File</a>";
                        sBuilder.Append(
                            "<tr id='" + item.trainer_id + "'>" +
                                "<td>" + item.course_name + "</td>" +
                                "<td>" + item.courseid + "</td>" +
                                "<td>" + item.batchid + "</td>" +
                                "<td>" + item.fin_number + "</td>" +
                                "<td class='text-center'>" + Studentlist + "</td>" +
                            "</tr>");
                        //}
                    }
                    //}
                }
                else
                {
                    list.Where(a => a.trainer_id == CurrentSession.Trainer_AirLine_id.ToString());
                    if (string.IsNullOrEmpty(CurrentSession.Trainer_AirLine_id.ToString()) || CurrentSession.Trainer_AirLine_id != 0)
                    {

                        var actions = "<button type='button' title='Delete' class='btn btn-danger btn-sm btnDelete'><i class='fa fa-trash'></i></button>";
                        foreach (var item in list)
                        {
                            if (item.trainer_id != "0" && item.trainer_id == CurrentSession.Trainer_AirLine_id.ToString().Trim())
                            {

                                var Studentlist = "<a href='/AssessmentPaper/Student_list/?course_id=" + item.courseid + "&batch_id=" + item.batchid + "' class='btn btn-info btn-sm Download'>Student list</a>";
                                //string awsurl = "<a href='/AssessmentPaper/GetCourseDocFile/?d=" + item.trainerid + "' class='btn btn-info btn - sm Download'>View File</a>";
                                sBuilder.Append(
                                    "<tr id='" + item.trainer_id + "'>" +
                                        "<td>" + item.course_name + "</td>" +
                                        "<td>" + item.courseid + "</td>" +
                                        "<td>" + item.batchid + "</td>" +
                                        "<td>" + item.fin_number + "</td>" +
                                        "<td class='text-center'>" + Studentlist + "</td>" +
                                    "</tr>");
                            }
                        }
                    }
                }

            }
            catch (Exception ex) { }
            return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetStudentList(string coureseid, string batchid)
        {
            var sBuilder = new StringBuilder();
            try
            {


                CourseService.SSTM service = new SSTM.CourseService.SSTM();
                string data = service.GetSSTMStudent(Convert.ToInt32(coureseid), float.Parse(batchid));

                List<studentdata> list = (new JavaScriptSerializer()).Deserialize<List<studentdata>>(data);


                if (string.IsNullOrEmpty(CurrentSession.Trainer_AirLine_id.ToString()) || CurrentSession.Trainer_AirLine_id != 0)
                {


                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item.Fin.ToString()))
                        {
                            string actions = "";
                            actions = "<a href='/AssessmentPaper/Student_assement_paper/?course_id=" + coureseid + "&batch_id=" + batchid + "&fin= " + item.Fin.Trim() + "' class='btn btn-info btn-sm btnview'>Add</a>";

                            var courseDocEntity = _IAssessmentPaperService.isexist_record(Convert.ToInt32(coureseid), Convert.ToDecimal(batchid), item.Fin.Trim());
                            if (courseDocEntity != null)
                                actions = "<a href='/AssessmentPaper/Student_assement_paper_view/?course_id=" + coureseid + "&batch_id=" + batchid + "&fin= " + item.Fin.Trim() + "' class='btn btn-info btn-sm btnview'>View</a>";

                            sBuilder.Append(
                                "<tr id='" + item.Fin.Trim() + "'>" +
                                    "<td>" + item.Courseid + "</td>" +
                                    "<td>" + item.Studentname + "</td>" +
                                    "<td>" + item.Fin.Trim() + "</td>" +
                                    "<td>" + item.CourseName + "</td>" +
                                    "<td class='text-center'>" + actions + "</td>" +
                                "</tr>");
                        }
                    }
                }

            }
            catch (Exception ex) { }
            return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAssessmentPaperCourseDocument(long Id)
        {
            try
            {
                var AssessmentDocEntity = _IAssessmentPaperService.GetRecordById(Id);

                if (AssessmentDocEntity != null)
                {
                    _IAssessmentPaperService.DeleteRecord(Id);

                    #region Deleting files
                    var configEntity = _IConfigService.GetFirstRecord();
                    AWSModel awsModel = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = "AssementPaper/" + AssessmentDocEntity.courseid.ToString(),

                        FileName = AssessmentDocEntity.filename
                    };
                    AWSHelper.DeleteFile(awsModel);
                    #endregion
                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "No details found." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "AssessmentPaper", "DeleteAssessmentPaperCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult SaveAssessmentDocument(long id, int courseid, decimal batchid, int qty, string trainer_id, string fin_number, string filename, string course_name)
        {

            try
            {
                if (courseid.ToString() != "")
                {
                    AssessmentPaper coursedoc = new AssessmentPaper();

                    if (string.IsNullOrEmpty(trainer_id))
                    {
                        return Json(new { result = false, message = "Trainer Selection Required !" });
                    }
                    if (string.IsNullOrEmpty(courseid.ToString()))
                    {
                        return Json(new { result = false, message = "Course Selection Required !" });
                    }

                    var isexist = _IAssessmentPaperService.GetCheckCourseExistById(courseid, id, trainer_id, qty, batchid);
                    if (isexist != null)
                        return Json(new { result = false, message = "Course Name is already exits within the same course." });



                    string fileName = null;
                    //if (Request.Files.Count > 0)
                    //{
                    ////  Get all files from Request object  
                    //HttpFileCollectionBase files = Request.Files;
                    //HttpPostedFileBase file = files[0];

                    //var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    //if (fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".docx" || fileExtension == ".pptx" || fileExtension == ".ppt" ||
                    //    fileExtension == ".xlsx" || fileExtension == ".xls" ||
                    //     fileExtension.ToLower() == ".ogg")
                    //{
                    //    #region Save file on cloud storage
                    //    // Checking for Internet Explorer  
                    //    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    //    {
                    //        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    //        fileName = testfiles[testfiles.Length - 1];
                    //    }
                    //    else
                    //        fileName = file.FileName;
                    //    int isreplace = 0;
                    //    if (isreplace == 0)
                    //    {
                    //        Random generator = new Random();
                    //        fileName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "") + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + Path.GetExtension(fileName);
                    //    }
                    //    else
                    //    {
                    //        if (Path.GetExtension(coursedoc.filename) == Path.GetExtension(fileName))
                    //        {
                    //            fileName = coursedoc.filename;
                    //        }
                    //        else
                    //        {
                    //            return Json(new { resulf = false, message = "Document extension not match same extention file upload like .docs , .pdf ...!" });
                    //        }
                    //    }


                    var configEntity = _IConfigService.GetFirstRecord();
                    //    AWSModel awsModel = new AWSModel()
                    //    {
                    //        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    //        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    //        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    //        BucketDirectory = "AssementPaper/" + courseid.ToString(),
                    //        FileName = fileName,
                    //        LocalFileStream = file.InputStream
                    //    };

                    //    AWSHelper.UploadFile(awsModel);
                    //    #endregion

                    #region Save record into Course Document table
                    var courseDocEntity = _IAssessmentPaperService.GetRecordById(id);
                    if (courseDocEntity == null)
                    {
                        courseDocEntity = new AssessmentPaperModel
                        {
                            courseid = courseid,
                            batchid = batchid,
                            qty = qty,
                            course_name = course_name.Trim(),
                            batch_name = batchid.ToString(),
                            filename = "",
                            trainer_id = trainer_id.Trim(),
                            fin_number = fin_number.Trim(),
                            CreatedBy = CurrentSession.UserId,
                            CreatedOn = DateTime.Now
                        }.ToEntity();

                        #region Send email notification to Trainer
                        var TrainerEntity = _IUserService.GetDefaultList().Where(a => a.RoleId == 6 && a.Trainer_AirLine_id == Convert.ToInt64(trainer_id)).ToList();
                        if (TrainerEntity.Count() != 0)
                        {
                            foreach (var item in TrainerEntity)
                            {

                                var toEmails = UtilityHelper.Decrypt(item.Email);

                                var emailBody = UtilityHelper.GetEmailTemplate("AssessmentPaperToTrainer.html").ToString();
                                emailBody = emailBody.Replace("@DearName@", item.FirstName + " " + item.LastName)
                                    .Replace("@DeveloperName@", CurrentSession.UserName)
                                    .Replace("@CourseName@", course_name);


                                EmailHelper.SendMail(new Models.EmailModel.EmailModel
                                {
                                    From = configEntity.Email,
                                    To = Request.IsLocal ? "meetmayur87@gmail.com" : toEmails,
                                    Subject = "SSTM Course Assessment Paper (" + course_name + ")",
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
                    //else
                    //{
                    //    courseDocEntity.DocName = DocName;
                    //    courseDocEntity.Filename = fileName;
                    //    courseDocEntity.isCompleted = currentcoursestatus == 7 ? false : true;
                    //    courseDocEntity.isReassigned = false;
                    //    courseDocEntity.UpdatedBy = CurrentSession.UserId;
                    //    courseDocEntity.UpdatedOn = DateTime.Now;
                    //}

                    var newId = _IAssessmentPaperService.SaveRecord(courseDocEntity);
                    #endregion

                    return Json(new { result = true, FileName = fileName, Id = newId });
                    //}
                    //else
                    //    return Json(new { result = false, message = AppMessages.InvalidFileExtention });
                }
                else
                    return Json(new { result = false, message = "Course Not Selected" });
                //}
                //else
                //    return Json(new { result = false, message = AppMessages.BlankDocumentName });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "SaveCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        public ActionResult GetCourseDocFile(long d)
        {
            try
            {
                documentPath = "";

                var fileName = string.Empty;
                var courseId = string.Empty;
                var bucketDirectory = string.Empty;
                long docId = 0;
                string docName = string.Empty;


                var courseDocEntity = _IAssessmentPaperService.GetRecordById(d);
                fileName = courseDocEntity.filename;
                courseId = courseDocEntity.courseid.ToString();
                bucketDirectory = "AssementPaper/" + courseDocEntity.courseid.ToString();
                docId = courseDocEntity.id;
                //docName = courseDocEntity.filename;

                if (fileName != "N/A")
                {
                    var configEntity = _IConfigService.GetFirstRecord();
                    var filePath = string.Empty;
                    var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());

                    if (Directory.Exists(sourceDir))
                        Directory.Delete(sourceDir, true);

                    Directory.CreateDirectory(sourceDir);

                    filePath = Path.Combine(sourceDir, fileName);

                    System.IO.File.Create(filePath).Close();

                    AWSHelper.GetSingleFile(new AWSModel
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = bucketDirectory,
                        FileName = fileName,
                        FilePath = filePath
                    });

                    documentPath = filePath;

                    var extension = Path.GetExtension(filePath);
                    var mimeType = UtilityHelper.GetMimeTypeFromExtension(extension);

                    ZohoResponseModel CRMDateApiResponse = new ZohoResponseModel();

                    if (extension.ToLower() == ".doc" || extension.ToLower() == ".docx" ||
                        extension.ToLower() == ".ppt" || extension.ToLower() == ".pptx" ||
                        extension.ToLower() == ".xls" || extension.ToLower() == ".xlsx")
                    {

                        string awsurl = Request.IsLocal ? UtilityHelper.testamazonlinkUrl + "AssementPaper/" + courseId + "/" + fileName : UtilityHelper.amazonlinkUrl + "AssementPaper/" + courseId + "/" + fileName;//md 04-02-2022
                        IRestResponse res = ZohoEditor.GetZohoEditor(filePath, extension, fileName, courseId, docId, docName, CurrentSession.UserId.ToString(), UtilityHelper.Decrypt(configEntity.ZohoApiKey), awsurl);
                        CRMDateApiResponse = JsonConvert.DeserializeObject<ZohoResponseModel>(res.Content);

                        if (!string.IsNullOrEmpty(CRMDateApiResponse.document_url))
                        {
                            //return Redirect(CRMDateApiResponse.document_url);
                            ViewBag.zohoOpenUrl = CRMDateApiResponse.document_url;
                            return View("viewZoho");
                        }
                        else
                        {
                            ViewBag.ZohoError = "Something went wrong. Please try again.";
                            return View("viewZoho");
                        }
                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        showPdfFile(filePath);
                        return View("../CourseDoc/CourseDocViewer");
                    }
                    else
                    {
                        ViewBag.zohoOpenUrl = "";
                        ViewBag.VideoPath = UtilityHelper.SiteUrl + "content/Temp/" + CurrentSession.UserId.ToString() + "/" + fileName;
                        ViewBag.videofomat = extension.Replace(".", "");
                        return View("viewZoho");
                    }

                    //return File(System.IO.File.ReadAllBytes(filePath), mimeType, fileName);
                }
                else
                    return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "GetCourseDocFile", Request.Url.AbsoluteUri, CurrentSession.UserId);
                throw ex;
            }
        }


        public ActionResult showPdfFile(string filePath)
        {
            ViewBag.DocName = filePath;
            return View("../CourseDoc/CourseDocViewer");
        }

        [HttpGet]
        public ActionResult DownoadDocFile(int id)
        {
            try
            {
                var AssessmentDocEntity = _IAssessmentPaperService.GetRecordById(id);
                string documentPath = "";

                var fileName = string.Empty;
                var courseId = string.Empty;
                var bucketDirectory = string.Empty;

                fileName = AssessmentDocEntity.filename;

                var configEntity = _IConfigService.GetFirstRecord();
                var filePath = string.Empty;
                var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", fileName.ToString());

                if (Directory.Exists(sourceDir))
                    Directory.Delete(sourceDir, true);

                Directory.CreateDirectory(sourceDir);

                filePath = Path.Combine(sourceDir, fileName);

                System.IO.File.Create(filePath).Close();

                string TrainerDocumentdir = "AssementPaper/" + AssessmentDocEntity.courseid.ToString();
                AWSHelper.GetSingleFile(new AWSModel
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = TrainerDocumentdir,
                    FileName = fileName,
                    FilePath = filePath
                });

                documentPath = filePath;

                var extension = Path.GetExtension(filePath);
                var mimeType = UtilityHelper.GetMimeTypeFromExtension(extension);

                return File(System.IO.File.ReadAllBytes(filePath), mimeType, fileName);

            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "DownoadCourseDocFile", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return null;
            }
        }


        #region Developer Monitor List

        public ActionResult DeveloperMonitorList()
        {
            var user = _IUserService.GetList(1).Where(l => l.Role == "Developer");
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "-- Select Developer --" });
            foreach (var item in user)
            {
                list.Add(new SelectListItem { Value = Convert.ToString(item.Id), Text = item.FirstName + " " + item.LastName });
            }

            TempData["userlist"] = new SelectList(list, "Value", "Text");
            return View();
        }
        [HttpGet]
        public ActionResult GetDeveloperMonitorList(string sdate, string edate, long userid)
        {
            var sBuilder = new StringBuilder();
            try
            {
                var list = _IDeveloperMonitorTimerService.GetDetails_all_List(sdate, edate, userid);

                foreach (var item in list)
                {
                    string timein = item.in_time.HasValue.ToString() == "True" ? item.in_time.Value.Hours.ToString().PadLeft(0) + ":" + item.in_time.Value.Minutes.ToString().PadLeft(0) : "00:00";
                    string timeout = item.out_time.HasValue.ToString() == "True" ? item.out_time.Value.Hours.ToString().PadLeft(0) + ":" + item.out_time.Value.Minutes.ToString().PadLeft(0) : "00:00";
                    string time = null;
                    string total = "";
                    if (item.in_time.HasValue.ToString() == "True" && item.out_time.HasValue.ToString() == "True")
                    {
                        var startTime = new TimeSpan(item.out_time.Value.Hours,0,0); // 6:00 AM
                        var endTime = new TimeSpan(5, 30, 0);
                        time = (item.out_time.Value.Hours - item.in_time.Value.Hours).ToString().PadLeft(0) + ":"+(item.out_time.Value.Minutes - item.in_time.Value.Minutes).ToString().PadLeft(0);
                        total = time;
                    }
                    

                    sBuilder.Append(
                        "<tr id='" + item.id + "'>" +
                            "<td>" + item.username + "</td>" +
                            "<td>" + item.date + "</td>" +
                            "<td>" + timein + "</td>" +
                            "<td>" + timeout + "</td>" +
                            "<td>" + total + "</td>" +
                        "</tr>");
                }

            }
            catch (Exception ex) { }
            return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult PdfEditor()
        {

            ViewBag.pdffilepath = "http://payroll.staging.rmskytech.com/docs/OHFL.pdf"; //Server.MapPath("~/Samples/sample.pdf");
            return View();
        }


    }
}


public class batchdata
{
    public int? Courseid { get; set; }
    public decimal? batchid { get; set; }
    public decimal? Quantity { get; set; }
}

public class student_file
{
    public string fin { get; set; }
    public string Files { get; set; }
    public string pdf { get; set; }
}

public class trainerdata
{
    public double? trainerid { get; set; }
    public string TrainerName { get; set; }
    public string dateofbatch { get; set; }
    public string courseid { get; set; }
    public string CourseShortName { get; set; }
    public double? batchid { get; set; }
    public string Sectionname { get; set; }
    public string fin { get; set; }
    public string Files { get; set; }
    public string pdf { get; set; }
}

public class trainer
{
    public double? trainerid { get; set; }
    public string fin { get; set; }
    public string Column1 { get; set; }
    public string courseid { get; set; }
    public string CourseShortName { get; set; }
    public decimal? batchid { get; set; }
    public string Sectionname { get; set; }
    public string trainername { get; set; }
}

public class studentdata
{
    public double? Courseid { get; set; }
    public string Studentname { get; set; }
    public string Fin { get; set; }
    public string CourseName { get; set; }
    public float? batchid { get; set; }
}
public class student_result_data
{
    public string division { get; set; }
    public float marks { get; set; }
    public string result { get; set; }
}

public class get_student_result_data
{
    public string Divisionname { get; set; }
    public float? DivisionMarks { get; set; }
    public string Result { get; set; }
}