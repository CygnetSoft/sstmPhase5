using SSTM.Business.Interfaces;
using SSTM.Core.TrainerQPUpload;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.AWS;
using SSTM.Models.TrainerQPUpload;
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
    public class TrainerQPUploadController : BaseController
    {
        #region Class Properties Declarations
        private readonly ITrainerUploadDocumentService _ITrainerUploadDocumentService;
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        private readonly ITrainerQPUploadService _ITrainerQPUploadService;
        private readonly ITrainerQPSharedStudentService _TrainerQPSharedStudentService;
        private readonly ITrainerQP_Level_ApprovalService _TrainerQP_Level_ApprovalService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        public TrainerQPUploadController(ITrainerUploadDocumentService ITrainerUploadDocumentService,
            IExceptionLogService exceptionLogService,
             IConfigService configService, IUserService userService, ITrainerQPUploadService ITrainerQPUploadService,
             ITrainerQPSharedStudentService ITrainerQPSharedStudentService,
             ITrainerQP_Level_ApprovalService TrainerQP_Level_ApprovalService)
        {
            _ITrainerUploadDocumentService = ITrainerUploadDocumentService;
            _IExceptionLogService = exceptionLogService;
            _IConfigService = configService;
            _IUserService = userService;
            _ITrainerQPUploadService = ITrainerQPUploadService;
            _TrainerQPSharedStudentService = ITrainerQPSharedStudentService;
            _TrainerQP_Level_ApprovalService = TrainerQP_Level_ApprovalService;
        }
        #endregion

        // GET: Trainer QP Upload
        public ActionResult Index()
        {
            long TrainerCurrentuser = 0, SmeId = 0;
            if (CurrentSession.UserRole == "Trainer")
                TrainerCurrentuser = CurrentSession.UserId;
            if (CurrentSession.UserRole == "SME")
                SmeId = CurrentSession.UserId;
            var list = _ITrainerQPUploadService.GetAllQPUploadDocsList(TrainerCurrentuser, SmeId).OrderByDescending(a => a.Id);
            return View(list);
        }

        public ActionResult SharedStudentList(long id = 0)
        {
            ViewBag.QPId = id;
            get_li_courseList();
            string dt = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.currentdata = dt;
            //get_li_BatchList(dt,"0");
            return View();
        }
        public void get_li_courseList()
        {
            CourseService.SSTM service = new CourseService.SSTM();
            string data = service.AllCourse();
            List<li_coursemodel> li_courselist = (new JavaScriptSerializer()).Deserialize<List<li_coursemodel>>(data);
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = "0", Text = "Select course name" });
            li_courselist.ForEach(a => { selectList.Add(new SelectListItem { Value = a.CourseId.ToString(), Text = a.CourseName }); });
            TempData["Li_courselist"] = new SelectList(selectList, "Value", "Text");
        }
        public ActionResult get_li_BatchList(string todaydate,string CourseId)
        {
            CourseService.SSTM service = new CourseService.SSTM();

            string data = service.TodayCoursesWithSection(todaydate);
            
            List<li_coursemodel> li_courselist = (new JavaScriptSerializer()).Deserialize<List<li_coursemodel>>(data);
           var coursedata= li_courselist.Where(a=>a.CourseId.Contains(CourseId)).Distinct().ToList();
            return Json(coursedata, JsonRequestBehavior.AllowGet);
            //var selectList = new List<SelectListItem>();
            //selectList.Add(new SelectListItem { Value = "0", Text = "Select batch name" });

            //coursedata.ForEach(a => { selectList.Add(new SelectListItem { Value = a.batchid.ToString(), Text = a.batchid }); });
            //TempData["Li_Batchlist"] = new SelectList(selectList, "Value", "Text");
        }


        [HttpPost]
        public ActionResult Shared_QP_Document(long Id)
        {
            try
            {
                var DocEntity = _ITrainerQPUploadService.GetRecordById(Id);
                DocEntity.Status = Convert.ToInt32(QPStatus.shared);
                DocEntity.isShared = true;
                DocEntity.UpdatedBy = CurrentSession.UserId;
                DocEntity.UpdatedOn = DateTime.Now;
                _ITrainerQPUploadService.SaveRecord(DocEntity);
                return Json(new { result = true, message = "Successfully Shared QP" });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainerQPUploadDocument", "QP_Document_Remark", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new
                {
                    result = false,
                    message = AppMessages.Exception
                });
            }
            return Json(new { result = true });
        }
        [HttpPost]
        public ActionResult QP_Document_Remark(long Id, string txtRemark)
        {
            try
            {
                var DocEntity = _ITrainerQPUploadService.GetRecordById(Id);
                DocEntity.Comment = txtRemark;
                if (txtRemark != "")
                {
                    DocEntity.Status = Convert.ToInt32(QPStatus.rework);
                }

                DocEntity.UpdatedBy = CurrentSession.UserId;
                DocEntity.UpdatedOn = DateTime.Now;
                _ITrainerQPUploadService.SaveRecord(DocEntity);
                if (txtRemark != "")
                {
                    #region Send notification to selected SME
                    var configEntity = _IConfigService.GetFirstRecord();
                    if (configEntity != null)
                    {
                        var documents = string.Empty;

                        var developerEntity = _IUserService.GetRecordById(DocEntity.TrainerId);

                        var developerName = string.Empty;
                        if (developerEntity != null)
                            developerName = developerEntity.FirstName + " " + developerEntity.LastName;

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToSMEAndDeveloper.html").ToString();
                        emailBody = emailBody.Replace("@DearName@", developerEntity.FirstName + " " + developerEntity.LastName)
                            .Replace("@CourseName@", DocEntity.DocumentName)
                            .Replace("@Documents@", DocEntity.DocumentPath)
                                .Replace("@DeveloperName@", developerName);

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(developerEntity.Email),
                            Subject = "SSTM QP RE-ASSIGNMENTS UPDATES (" + DocEntity.DocumentName + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                        #endregion
                    }
                    return Json(new { result = true });
                }
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainerQPUploadDocument", "QP_Document_Remark", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new
                {
                    result = false,
                    message = AppMessages.Exception
                });
            }
            return Json(new { result = true });

        }
        [HttpPost]
        public ActionResult SaveDocument(long Id, string DocFileName, int status)
        {
            try
            {
                if (DocFileName != "" && DocFileName != "undefined")
                {
                    string fileName = null;
                    if (Request.Files.Count > 0)
                    {
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = files[0];

                        var fileExtension = Path.GetExtension(file.FileName).ToLower();


                        if (fileExtension == ".pdf" || fileExtension.ToLower() == ".doc" || fileExtension.ToLower() == ".docx" || fileExtension.ToLower() == ".pptx" || fileExtension.ToLower() == ".ppt" ||
                        fileExtension.ToLower() == ".xlsx" || fileExtension.ToLower() == ".xls")
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

                            fileName = DocFileName.Replace(" ", "") + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + Path.GetExtension(fileName);

                            #region one table multle menu document Upload with folder in s3 bucket
                            string TrainerDocumentdir = "TrainerQP";

                            var configEntity = _IConfigService.GetFirstRecord();
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

                            #region Save record into Trainer QP Upload Document table
                            var DocEntity = _ITrainerQPUploadService.GetRecordById(Id);

                            #region delete old file
                            if (Id != 0)
                            {
                                try
                                {
                                    AWSModel awsModel1 = new AWSModel()
                                    {
                                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                                        BucketDirectory = TrainerDocumentdir,
                                        FileName = DocEntity.DocumentPath
                                    };
                                    AWSHelper.DeleteFile(awsModel1);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            #endregion
                            long TrainerCurrentuser = 0;
                            if (CurrentSession.UserRole == "Trainer")
                                TrainerCurrentuser = CurrentSession.UserId;
                            else
                                TrainerCurrentuser = CurrentSession.UserId;


                            if (DocEntity == null)
                            {
                                DocEntity = new TrainerQPUploadModel
                                {
                                    DocumentName = DocFileName,
                                    DocumentPath = fileName,
                                    Status = Convert.ToInt32(QPStatus.pending),
                                    TrainerId = TrainerCurrentuser,
                                    isShared = Convert.ToBoolean(TrainingQPStatus.notshared),
                                    CreatedBy = CurrentSession.UserId,
                                    CreatedOn = DateTime.Now,
                                }.ToEntity();
                            }
                            else
                            {
                                DocEntity.DocumentName = DocFileName;
                                DocEntity.DocumentPath = fileName;
                                DocEntity.Status = Convert.ToInt32(QPStatus.pending);
                                DocEntity.TrainerId = TrainerCurrentuser;
                                DocEntity.UpdatedBy = CurrentSession.UserId;
                                DocEntity.UpdatedOn = DateTime.Now;
                            }

                            var newId = _ITrainerQPUploadService.SaveRecord(DocEntity);
                            #endregion
                            if (Id == 0)
                            {
                                #region Send notification to selected SME
                                if (configEntity != null)
                                {
                                    var hrEmails = string.Empty;
                                    var documents = string.Empty;

                                    var developerEntity = _IUserService.GetRecordById(TrainerCurrentuser);
                                    var ManuList = _IUserService.GetList(1).Where(a => a.Role == "AEB" && a.Role == "Administration" && a.Role == "Developer");

                                    ManuList.ForEach(a => { hrEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                                    var developerName = string.Empty;
                                    if (developerEntity != null)
                                        developerName = developerEntity.FirstName + " " + developerEntity.LastName;

                                    var emailBody = UtilityHelper.GetEmailTemplate("TrainerQPReview.html").ToString();
                                    emailBody = emailBody.Replace("@CourseName@", DocFileName)
                                        .Replace("@TrainerName@", CurrentSession.UserName.Trim())
                                        .Replace("@QPDocument@", fileName.Trim())
                                        .Replace("@DocumentName@", developerName);
                                    try
                                    {


                                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                                        {
                                            From = configEntity.Email,
                                            To = Request.IsLocal ? "teversafe@gmail.com" : hrEmails.Trim().TrimEnd(';'),
                                            Subject = "SSTM QP Document (" + DocFileName + ")",
                                            Message = emailBody,
                                            SMTPHost = configEntity.Host,
                                            SMTPPort = configEntity.Port,
                                            SMTPEmail = configEntity.Email,
                                            SMTPPassword = configEntity.Pass,
                                            EnableSsl = configEntity.EnableSsl
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        _IExceptionLogService.SaveRecord(ex, "TrainerQPUploadDocument", "SaveCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                                        return Json(new
                                        {
                                            result = false,
                                            message = AppMessages.Exception
                                        });
                                    }
                                }
                                #endregion
                            }
                            if (Id == 0)
                            {
                                return Json(new { result = true, FileName = fileName, Id = newId, message = "Successfully Upload QP " });
                            }
                            else
                                return Json(new { result = true, FileName = fileName, Id = newId, message = "Successfully Update QP " });

                        }
                        else
                            return Json(new { result = false, message = "QP Upload Document File Extention (doc,ppt or xlxs,PDF files)" });

                    }
                    else
                        return Json(new { result = false, message = AppMessages.NoFileSelected });
                }
                else
                    return Json(new { result = false, message = AppMessages.BlankDocumentName });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainerQPUploadDocument", "SaveCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new
                {
                    result = false,
                    message = AppMessages.Exception
                });
            }
        }

        [HttpPost]
        public ActionResult DeleteQPCourseDocument(string Id)
        {
            try
            {
                var courseDocEntity = _ITrainerQPUploadService.GetRecordById(Convert.ToInt64(Id));

                if (courseDocEntity != null)
                {
                    _ITrainerQPUploadService.DeleteRecord(Convert.ToInt64(Id));

                    string TrainerDocumentdir = "TrainerQP";
                    #region Deleting files for course document version files
                    try
                    {
                        var configEntity = _IConfigService.GetFirstRecord();
                        AWSModel awsModel = new AWSModel()
                        {
                            AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                            SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                            BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                            BucketDirectory = TrainerDocumentdir,
                            FileName = courseDocEntity.DocumentPath
                        };
                        AWSHelper.DeleteFile(awsModel);
                    }
                    catch (Exception)
                    {
                    }
                    #endregion


                    return Json(new { result = true, message = "Successfully Delete QP Document" });
                }
                else
                    return Json(new { result = false, message = "No details found." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "DeleteQPCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        [HttpGet]
        public ActionResult DownoadDocFile(string filename)
        {
            try
            {
                string documentPath = "";

                var fileName = string.Empty;
                var courseId = string.Empty;
                var bucketDirectory = string.Empty;

                fileName = filename;
                if (fileName != "N/A")
                {
                    var configEntity = _IConfigService.GetFirstRecord();
                    var filePath = string.Empty;
                    var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", filename.ToString());

                    if (Directory.Exists(sourceDir))
                        Directory.Delete(sourceDir, true);

                    Directory.CreateDirectory(sourceDir);

                    filePath = Path.Combine(sourceDir, fileName);

                    System.IO.File.Create(filePath).Close();

                    #region one table multle menu document Upload with folder in s3 bucket
                    string TrainerDocumentdir = "TrainerQP";

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
                    #endregion

                    var extension = Path.GetExtension(filePath);
                    var mimeType = UtilityHelper.GetMimeTypeFromExtension(extension);

                    return File(System.IO.File.ReadAllBytes(filePath), mimeType, fileName);
                }
                else
                    return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "DownoadCourseDocFile", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return null;
            }
        }
        [HttpPost]
        public ActionResult AssignCourseToSME(long Id, long SMEId)
        {
            try
            {
                var DocEntity = _ITrainerQPUploadService.GetRecordById(Id);

                var smeEntity = _IUserService.GetOuterLoginRecordById(SMEId);

                if (DocEntity != null && smeEntity != null)
                {

                    #region Update course status in Db
                    DocEntity.Status = Convert.ToInt32(QPStatus.review);
                    DocEntity.SMEId = SMEId;
                    DocEntity.UpdatedBy = CurrentSession.UserId;
                    DocEntity.UpdatedOn = DateTime.Now;

                    _ITrainerQPUploadService.SaveRecord(DocEntity);
                    #endregion


                    #region Send notification to selected SME
                    var configEntity = _IConfigService.GetFirstRecord();
                    if (configEntity != null)
                    {
                        var documents = string.Empty;


                        var developerEntity = _IUserService.GetRecordById(smeEntity.Id);

                        var developerName = string.Empty;
                        if (developerEntity != null)
                            developerName = developerEntity.FirstName + " " + developerEntity.LastName;

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToSMEAndDeveloper.html").ToString();
                        emailBody = emailBody.Replace("@DearName@", smeEntity.FirstName + " " + smeEntity.LastName)
                            .Replace("@CourseName@", DocEntity.DocumentName)
                            .Replace("@Documents@", DocEntity.DocumentPath)
                                .Replace("@DeveloperName@", developerName);

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(smeEntity.Email),
                            Subject = "SSTM QP ASSIGNMENTS UPDATES (" + DocEntity.DocumentName + ")",
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
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "AssignCourseToSME", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult OpenSMEList()
        {
            try
            {
                GetSMEList();

                return PartialView("_SMEQPListModal");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TraierQpController", "OpenSMEQpList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }
        public void GetSMEList()
        {
            var list = _IUserService.GetDefaultList().Where(a => a.RoleId == 0).ToList();
            // var list = _IUserService.GetList(1).Where(a => a.Role.Trim() == "SME").ToList();

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = "0", Text = "Select" });

            list.ForEach(a => { selectList.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.FirstName + " " + a.LastName }); });

            TempData["SMEList"] = new SelectList(selectList, "Value", "Text");
        }

        #region  Assign QP Student 
        public ActionResult QPAssignStudentList(long id)
        {
            var sBuilder = new StringBuilder();
            var list = _TrainerQPSharedStudentService.GetRecordList(id);
            foreach (var item in list)
            {
                sBuilder.Append(
                     "<tr id='" + item.id + "'>" +
                         "<td>" + item.course_name + "</td>" +
                          "<td>" + item.batch_name + "</td>" +
                          "<td class='text-center'>" +
                                "<a href='javascript: void(0)' id='btndelete' class='btn btn-danger btn-sm delete' onclick='Delete(" + item.id + ")'> Delete</a>" +
                            "</td>" +
                          "</tr>"
                         );
            }
            return Json(new { result = true, listdata = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveQP(TrainerQP_Shared_StudentModel model)
        {
            try
            {
                if (model.course_id == "0")
                {
                    return Json(new { result = true, message = "Course Required" }, JsonRequestBehavior.AllowGet);
                }
                if (model.batch_id == "0")
                {
                    return Json(new { result = true, message = "Batch Required" }, JsonRequestBehavior.AllowGet);
                }

                var macaddr = new TrainerQP_Shared_StudentModel().ToEntity();
                macaddr.course_id = model.course_id;
                macaddr.batch_id = model.batch_id;
                macaddr.QP_id = model.QP_id;
                macaddr.course_name = model.course_name;
                macaddr.batch_name = model.batch_name;
                _TrainerQPSharedStudentService.SaveRecord(macaddr);
                return Json(new { result = true, message = "Successfully Student Map with QP" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "SaveQP", "TrainerQPUpload", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = "Error in Student QP" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteQP(long id)
        {
            try
            {
                _TrainerQPSharedStudentService.DeleteRecord(id);
                return Json(new { result = true, message = "Successfull remove link Student to QP " }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "DeleteQP", "TrainerQPUpload", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }

        }
        #endregion

        public ActionResult GetQPToStudent(string courseid, string batch_id)
        {
            var list = _TrainerQPSharedStudentService.GetQpToStundetList(courseid, batch_id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult QP_Approval_Level()
        {
            return View();
        }
        public ActionResult level1_approval(string id, string comment, string IsAccept)
        {
            try
            {
                var QPLevelEntity = _TrainerQP_Level_ApprovalService.GetRecordByQPId(Convert.ToInt64(id));
                if (QPLevelEntity == null)
                {
                    var approvEntity = new TrainerQP_Level_ApprovalModel
                    {
                        QP_Id = Convert.ToInt64(id)
                    }.ToEntity();
                    var data = _TrainerQP_Level_ApprovalService.SaveRecord(approvEntity);
                    // var QPLevelEntity1 = _TrainerQP_Level_ApprovalService.GetRecordByQPId(Convert.ToInt64(id));
                    approvEntity.Level1_IsAccept = IsAccept;
                    approvEntity.Level1_Comment = comment;
                    approvEntity.Level1_User_id = CurrentSession.UserId;
                    approvEntity.Level1_Approve_date = DateTime.Now;
                    _TrainerQP_Level_ApprovalService.SaveRecord(approvEntity);
                    return Json(new { result = true, content = "", message = "Success" }, JsonRequestBehavior.AllowGet);
                    //return Json(new { result = false, content = "", message = "record not added in level entry table" }, JsonRequestBehavior.AllowGet);
                }
                QPLevelEntity.Level1_IsAccept = IsAccept;
                QPLevelEntity.Level1_Comment = comment;
                QPLevelEntity.Level1_User_id = CurrentSession.UserId;
                QPLevelEntity.Level1_Approve_date = DateTime.Now;
                _TrainerQP_Level_ApprovalService.SaveRecord(QPLevelEntity);
                return Json(new { result = true, content = "", message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, content = "", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult level2_approval(string id, string comment, string IsAccept)
        {
            try
            {
                var QPLevelEntity = _TrainerQP_Level_ApprovalService.GetRecordByQPId(Convert.ToInt64(id));
                if (QPLevelEntity == null)
                {
                    var approvEntity = new TrainerQP_Level_ApprovalModel
                    {
                        QP_Id = Convert.ToInt64(id)
                    }.ToEntity();
                    _TrainerQP_Level_ApprovalService.SaveRecord(approvEntity);
                    // var QPLevelEntity1 = _TrainerQP_Level_ApprovalService.GetRecordByQPId(Convert.ToInt64(id));
                    approvEntity.Level1_IsAccept = IsAccept;
                    approvEntity.Level1_Comment = comment;
                    approvEntity.Level1_User_id = CurrentSession.UserId;
                    approvEntity.Level1_Approve_date = DateTime.Now;
                    _TrainerQP_Level_ApprovalService.SaveRecord(approvEntity);
                    return Json(new { result = true, content = "", message = "Success" }, JsonRequestBehavior.AllowGet);
                }
                QPLevelEntity.Level2_IsAccept = IsAccept;
                QPLevelEntity.Level2_Comment = comment;
                QPLevelEntity.Level2_User_id = CurrentSession.UserId;
                QPLevelEntity.Level2_Approve_date = DateTime.Now;
                _TrainerQP_Level_ApprovalService.SaveRecord(QPLevelEntity);
                return Json(new { result = true, content = "", message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, content = "", message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult level3_approval(string id, string comment, string IsAccept)
        {
            try
            {
                var QPLevelEntity = _TrainerQP_Level_ApprovalService.GetRecordByQPId(Convert.ToInt64(id));
                if (QPLevelEntity == null)
                {
                    var approvEntity = new TrainerQP_Level_ApprovalModel
                    {
                        QP_Id = Convert.ToInt64(id)
                    }.ToEntity();
                    _TrainerQP_Level_ApprovalService.SaveRecord(approvEntity);
                    // var QPLevelEntity1 = _TrainerQP_Level_ApprovalService.GetRecordByQPId(Convert.ToInt64(id));
                    approvEntity.Level1_IsAccept = IsAccept;
                    approvEntity.Level1_Comment = comment;
                    approvEntity.Level1_User_id = CurrentSession.UserId;
                    approvEntity.Level1_Approve_date = DateTime.Now;
                    _TrainerQP_Level_ApprovalService.SaveRecord(approvEntity);
                    return Json(new { result = true, content = "", message = "Success" }, JsonRequestBehavior.AllowGet);
                }
                QPLevelEntity.Level3_IsAccept = IsAccept;
                QPLevelEntity.Level3_Comment = comment;
                QPLevelEntity.Level3_User_id = CurrentSession.UserId;
                QPLevelEntity.Level3_Approve_date = DateTime.Now;
                _TrainerQP_Level_ApprovalService.SaveRecord(QPLevelEntity);
                return Json(new { result = true, content = "", message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, content = "", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult Get_approval_list()
        {
            try
            {
                var sBuilder = new StringBuilder();
                var list = _TrainerQP_Level_ApprovalService.Trainer_QP_Level_Approval_data().OrderByDescending(a => a.id);
                int count = 1;
                foreach (var item in list)
                {
                    string data = "";
                    if (CurrentSession.UserRole == "Administration" || CurrentSession.UserRole == "Director")
                    {
                        data = "<td class='text-center'>" +
                                       "<button type = 'button' title = 'Accpet' class='btn btn-primary btn-sm btnlevel1Approved'>Accpet</button>&nbsp;" +
                                       "<button type='button' title='Reject' class='btn btn-danger btn-sm btnlevel1Cancel'>Reject</button>";
                        data += !string.IsNullOrEmpty(item.Level1_IsAccept) ? "<br>status: " + item.Level1_IsAccept + "<br> Trainer Name: " + item.l1_Username + "</td>" : "</td>";

                        data += "<td class='text-center'>" +
                          "<button type = 'button' title = 'Accpet' class='btn btn-primary btn-sm btnlevel2Approved'>Accpet</button>&nbsp;" +
                          "<button type='button' title='Reject' class='btn btn-danger btn-sm btnlevel2Cancel'>Reject</button>";
                        data += !string.IsNullOrEmpty(item.Level2_IsAccept) ? "<br>status: " + item.Level2_IsAccept + "<br> Trainer Name: " + item.l2_Username + "</td>" : "</td>";

                        data += "<td class='text-center'>" +
                             "<button type = 'button' title = 'Accpet' class='btn btn-primary btn-sm btnlevel3Approved'>Accpet</button>&nbsp;" +
                             "<button type='button' title='Reject' class='btn btn-danger btn-sm btnlevel3Cancel'>Reject</button>";
                        data += !string.IsNullOrEmpty(item.Level1_IsAccept) ? "<br>status: " + item.Level3_IsAccept + "<br> Trainer Name: " + item.l3_Username + "</td>" : "</td>";
                    }
                    else if (CurrentSession.UserRole == "QP_Approval_Level1")
                    {
                        if (string.IsNullOrEmpty(item.Level1_IsAccept))
                        {
                            data += "<td class='text-center'>" +
                                          "<button type = 'button' title = 'Accpet' class='btn btn-primary btn-sm btnlevel1Approved'>Accpet</button>&nbsp;" +
                                          "<button type='button' title='Reject' class='btn btn-danger btn-sm btnlevel1Cancel'>Reject</button>" +
                                          "<br>status: " + item.Level1_IsAccept + "<br> Trainer Name: " + item.l1_Username + "</td>" +
                                          "<td>status: " + item.Level2_IsAccept + "<br> Trainer Name: " + item.l2_Username + "</td>" +
                                          "<td>status: " + item.Level3_IsAccept + "<br> Trainer Name: " + item.l3_Username + "</td>";
                        }
                        else
                        {
                            data += "<td class='text-center'>" +
                                   "status: " + item.Level1_IsAccept + "<br> Trainer Name: " + item.l1_Username + "</td>" +
                                   "<td>status: " + item.Level2_IsAccept + "<br> Trainer Name: " + item.l2_Username + "</td>" +
                                   "<td>status: " + item.Level3_IsAccept + "<br> Trainer Name: " + item.l3_Username + "</td>";
                        }


                    }
                    else if (CurrentSession.UserRole == "QP_Approval_Level2")
                    {
                        if (string.IsNullOrEmpty(item.Level2_IsAccept))
                        {
                            data += "<td >status: " + item.Level1_IsAccept + "<br> Trainer Name: " + item.l1_Username + "</td>" +
                                          "<td class='text-center'><button type = 'button' title = 'Accpet' class='btn btn-primary btn-sm btnlevel2Approved'>Accpet</button>&nbsp;" +
                                          "<button type='button' title='Reject' class='btn btn-danger btn-sm btnlevel2Cancel'>Reject</button>" +
                                          "<br>status: " + item.Level2_IsAccept + "<br> Trainer Name: " + item.l2_Username + "</td>" +
                                           "<td>status: " + item.Level3_IsAccept + "<br> Trainer Name: " + item.l3_Username + "</td>";
                        }
                        else
                        {
                            data += "<td class='text-center'>" +
                                   "status: " + item.Level1_IsAccept + "<br> Trainer Name: " + item.l1_Username + "</td>" +
                                   "<td>status: " + item.Level2_IsAccept + "<br> Trainer Name: " + item.l2_Username + "</td>" +
                                   "<td>status: " + item.Level3_IsAccept + "<br> Trainer Name: " + item.l3_Username + "</td>";
                        }
                    }
                    else if (CurrentSession.UserRole == "QP_Approval_Level3")
                    {
                        if (string.IsNullOrEmpty(item.Level2_IsAccept))
                        {
                            data += "<td class='text-center'>" +
                                   "status: " + item.Level1_IsAccept + "<br> Trainer Name: " + item.l1_Username + "</td>" +
                                   "<td>status: " + item.Level2_IsAccept + "<br> Trainer Name: " + item.l2_Username + "</td>" +
                                          "<td><button type = 'button' title = 'Accpet' class='btn btn-primary btn-sm btnlevel3Approved'>Accpet</button>&nbsp;" +
                                          "<button type='button' title='Reject' class='btn btn-danger btn-sm btnlevel3Cancel'>Reject</button>" +
                                          "<br>status: " + item.Level3_IsAccept + "<br> Trainer Name: " + item.l3_Username + "</td>";
                        }
                        else
                        {
                            data += "<td class='text-center'>" +
                                   "status: " + item.Level1_IsAccept + "<br> Trainer Name: " + item.l1_Username + "</td>" +
                                   "<td>status: " + item.Level2_IsAccept + "<br> Trainer Name: " + item.l2_Username + "</td>" +
                                   "<td>status: " + item.Level3_IsAccept + "<br> Trainer Name: " + item.l3_Username + "</td>";
                        }

                    }

                    sBuilder.Append(
                           "<tr id='" + item.id + "'>" +
                            "<td>" + count++ + "</td>" +
                               "<td>" + item.DocumentName + "</td>" +
                               "<td><a href='javascript: void(0)' class='viewdoc' title='view document' >" + item.DocumentPath + "</a></td>" +
                               "<td>Shared</td>" +
                               data +
                           "</tr>");
                }
                return Json(new { result = true, content = sBuilder.ToString(), message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, content = "", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

public class li_coursemodel
{
    public string CourseId { get; set; }
    public string CourseShortname { get; set; }
    public string CourseName { get; set; }
    public string batchid { get; set; }
}