using SSTM.Business.Interfaces;
using SSTM.Core.TrainerUploadDocument;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.AWS;
using SSTM.Models.TrainerUploadDocumentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class TrainerUploadDocumentController : Controller
    {
        #region Class Properties Declarations
        private readonly ITrainerUploadDocumentService _ITrainerUploadDocumentService;
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        public TrainerUploadDocumentController(ITrainerUploadDocumentService ITrainerUploadDocumentService,
            IExceptionLogService exceptionLogService,
             IConfigService configService, IUserService userService)
        {
            _ITrainerUploadDocumentService = ITrainerUploadDocumentService;
            _IExceptionLogService = exceptionLogService;
            _IConfigService = configService;
            _IUserService = userService;
        }
        #endregion
        // GET: TrainerUploadDocument
        public ActionResult Index(bool? MasterCourse, long? MasterCourseId)
        {
            if (MasterCourse == null)
                MasterCourse = true;
            if (MasterCourseId == null)
                MasterCourseId = 0;

            ViewBag.MasterDoc = MasterCourse.ToString();
            ViewBag.MasterDocId = MasterCourseId;

            var list = _ITrainerUploadDocumentService.GetUploadDocsList(0);
            ViewBag.docstatus = 0;//TrainerUploadDocuments
            return View(list);
        }
        public ActionResult CommonDocument(bool? MasterDoc, long? MasterDocId,string DocumentName)
        {
            if (MasterDoc == null)
                MasterDoc = true;
            if (MasterDocId == null)
                MasterDocId = 0;

            ViewBag.MasterDoc = MasterDoc.ToString();
            ViewBag.MasterDocId = MasterDocId;
            ViewBag.DocumentName = DocumentName;

            var list = _ITrainerUploadDocumentService.GetCommonUploadDocsList(2, MasterDoc, MasterDocId);
            ViewBag.docstatus = 2;//TrainerUploadCommonDoc
            return View(list);
        }
        public ActionResult VideoFiles(bool? MasterDoc, long? MasterDocId, string DocumentName)
        {
            if (MasterDoc == null)
                MasterDoc = true;
            if (MasterDocId == null)
                MasterDocId = 0;

            ViewBag.MasterDoc = MasterDoc.ToString();
            ViewBag.MasterDocId = MasterDocId;
            ViewBag.DocumentName = DocumentName;

            var list = _ITrainerUploadDocumentService.GetCommonUploadDocsList(1, MasterDoc, MasterDocId);
            ViewBag.docstatus = 1;//TrainerUploadvideo
            return View(list);
        }

        public ActionResult ConnfidentialDocument(bool? MasterDoc, long? MasterDocId, string DocumentName)
        {
            if (MasterDoc == null)
                MasterDoc = true;
            if (MasterDocId == null)
                MasterDocId = 0;

            ViewBag.MasterDoc = MasterDoc.ToString();
            ViewBag.MasterDocId = MasterDocId;
            ViewBag.DocumentName = DocumentName;

            var list = _ITrainerUploadDocumentService.GetCommonUploadDocsList(3, MasterDoc, MasterDocId);
            ViewBag.docstatus = 1;//ConfindentialDocument
            return View(list);
        }
        [HttpPost]
        public ActionResult SaveDocument(long Id, string DocFileName, int status,bool MasterDoc, long MasterDocId,string ismainfolder)
        {
            #region one table multle menu document Upload
            int statusdata = 0;
            if (status == 0)
                statusdata = (int)TrainingDocumentStatus.UploadDocs;
            else if (status == 1)
                statusdata = (int)TrainingDocumentStatus.video;
            else if (status == 3)
                statusdata = (int)TrainingDocumentStatus.ConfidentialDoc;
            else
                statusdata = (int)TrainingDocumentStatus.CommonDoc;
            #endregion
            if(statusdata==0)
                ismainfolder = "false";

            JsonResult jsOutput = new JsonResult();

            //status => 0 for Upload Docs,1 Video, 2 for common Docs
            try
            {
                TrainerUploadDocument TrainerDocument = new TrainerUploadDocument();

                if (DocFileName != "" && DocFileName != "undefined")
                {
                    string fileName = null;
                    if (ismainfolder != "true")
                    {
                        if (Request.Files.Count > 0)
                        {
                            //  Get all files from Request object  
                            HttpFileCollectionBase files = Request.Files;

                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];

                                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                                if (statusdata == 2 || statusdata == 0 || statusdata == 3)
                                {
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
                                        //fileName = generator.Next(0, 1000000).ToString("D10") + Path.GetExtension(fileName);
                                        fileName = DocFileName.Replace(" ", "") + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + Path.GetExtension(fileName);

                                        #region one table multle menu document Upload with folder in s3 bucket

                                        string TrainerDocumentdir = "TrainerDocument";
                                        if (status == 0)
                                            TrainerDocumentdir = "TrainerUploadDocuments";
                                        else if (status == 1)
                                            TrainerDocumentdir = "TrainerUploadvideo";
                                        else if (status == 3)
                                            TrainerDocumentdir = "ConfindentialDocument";
                                        else
                                            TrainerDocumentdir = "TrainerUploadCommonDoc";
                                        #endregion

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

                                        #region Save record into Trainer Upload Document table
                                        var DocEntity = _ITrainerUploadDocumentService.GetRecordById(Id);
                                        long TrainerCurrentuser = 0;
                                        if (CurrentSession.UserRole == "Trainer")
                                            TrainerCurrentuser = CurrentSession.UserId;

                                        if (DocEntity == null)
                                        {
                                            DocEntity = new TrainerUploadDocumentModel
                                            {
                                                DocumentName = DocFileName,
                                                DocumentPath = fileName,
                                                Status = statusdata,
                                                TrainerId = TrainerCurrentuser,
                                                MasterDoc = MasterDoc,
                                                MasterDocId = MasterDocId,
                                                CreatedBy = CurrentSession.UserId,
                                                CreatedOn = DateTime.Now,
                                            }.ToEntity();
                                        }
                                        else
                                        {
                                            DocEntity.DocumentName = DocFileName;
                                            DocEntity.DocumentPath = fileName;
                                            DocEntity.Status = statusdata;
                                            DocEntity.MasterDoc = MasterDoc;
                                            DocEntity.MasterDocId = MasterDocId;
                                            DocEntity.UpdatedBy = CurrentSession.UserId;
                                            DocEntity.UpdatedOn = DateTime.Now;
                                        }

                                        var newId = _ITrainerUploadDocumentService.SaveRecord(DocEntity);
                                        #endregion


                                        jsOutput = Json(new { result = true, FileName = fileName, Id = newId });
                                    }
                                    else
                                        jsOutput= Json(new { result = false, message = "Upload Document File Extention (doc,ppt or xlxs,PDF files)" });
                                }
                                else
                                {
                                    if (fileExtension.ToLower() == ".webm")
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
                                        //fileName = generator.Next(0, 1000000).ToString("D10") + Path.GetExtension(fileName);
                                        fileName = DocFileName.Replace(" ", "") + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + Path.GetExtension(fileName);

                                        #region one table multle menu document Upload with folder in s3 bucket

                                        string TrainerDocumentdir = "TrainerDocument";
                                        if (status == 0)
                                            TrainerDocumentdir = "TrainerUploadDocuments";
                                        else if (status == 1)
                                            TrainerDocumentdir = "TrainerUploadvideo";
                                        else if (status == 3)
                                            TrainerDocumentdir = "ConfindentialDocument";
                                        else
                                            TrainerDocumentdir = "TrainerUploadCommonDoc";
                                        #endregion

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

                                        #region Save record into Trainer Upload Document table
                                        var DocEntity = _ITrainerUploadDocumentService.GetRecordById(Id);
                                        long TrainerCurrentuser = 0;
                                        if (CurrentSession.UserRole == "Trainer")
                                            TrainerCurrentuser = CurrentSession.UserId;

                                        if (DocEntity == null)
                                        {
                                            DocEntity = new TrainerUploadDocumentModel
                                            {
                                                DocumentName = DocFileName,
                                                DocumentPath = fileName,
                                                Status = statusdata,
                                                TrainerId = TrainerCurrentuser,
                                                CreatedBy = CurrentSession.UserId,
                                                MasterDoc = MasterDoc,
                                                MasterDocId = MasterDocId,
                                                CreatedOn = DateTime.Now,
                                            }.ToEntity();
                                        }
                                        else
                                        {
                                            DocEntity.DocumentName = DocFileName;
                                            DocEntity.DocumentPath = fileName;
                                            DocEntity.Status = statusdata;
                                            DocEntity.MasterDoc = MasterDoc;
                                            DocEntity.MasterDocId = MasterDocId;
                                            DocEntity.UpdatedBy = CurrentSession.UserId;
                                            DocEntity.UpdatedOn = DateTime.Now;
                                        }

                                        var newId = _ITrainerUploadDocumentService.SaveRecord(DocEntity);
                                        #endregion

                                        jsOutput= Json(new { result = true, FileName = fileName, Id = newId, message = "File Upload Successfully" });
                                    }
                                    else
                                        jsOutput= Json(new { result = false, message = "Upload Document File Extention (webm)" });
                                }
                            }
                        }
                        else
                            jsOutput=Json(new { result = false, message = AppMessages.NoFileSelected });
                    }
                    else
                    {
                        #region Save record into Trainer Upload Document table
                        var DocEntity = _ITrainerUploadDocumentService.GetRecordById(Id);
                        long TrainerCurrentuser = 0;
                        if (CurrentSession.UserRole == "Trainer")
                            TrainerCurrentuser = CurrentSession.UserId;

                        if (DocEntity == null)
                        {
                            DocEntity = new TrainerUploadDocumentModel
                            {
                                DocumentName = DocFileName,
                                DocumentPath = "",
                                Status = statusdata,
                                TrainerId = TrainerCurrentuser,
                                CreatedBy = CurrentSession.UserId,
                                MasterDoc = MasterDoc,
                                MasterDocId = MasterDocId,
                                CreatedOn = DateTime.Now,
                            }.ToEntity();
                        }
                        else
                        {
                            DocEntity.DocumentName = DocFileName;
                            DocEntity.DocumentPath = "";
                            DocEntity.Status = statusdata;
                            DocEntity.MasterDoc = MasterDoc;
                            DocEntity.MasterDocId = MasterDocId;
                            DocEntity.UpdatedBy = CurrentSession.UserId;
                            DocEntity.UpdatedOn = DateTime.Now;
                        }
                        var newId = _ITrainerUploadDocumentService.SaveRecord(DocEntity);
                        #endregion

                        jsOutput=Json(new { result = true, FileName = "", Id = newId });
                    }
                }
                else
                    jsOutput= Json(new { result = false, message = AppMessages.BlankDocumentName });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainerUploadDocument", "SaveCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new
                {
                    result = false,
                    message = AppMessages.Exception
                });
            }

            return jsOutput;
        }


        [HttpGet]
        public ActionResult DownoadDocFile(string filename, int status)
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
                    string TrainerDocumentdir = "TrainerDocument";
                    if (status == 0)
                        TrainerDocumentdir = "TrainerUploadDocuments";
                    else if (status == 1)
                        TrainerDocumentdir = "TrainerUploadvideo";
                    else if (status == 3)
                        TrainerDocumentdir = "ConfindentialDocument";
                    else
                        TrainerDocumentdir = "TrainerUploadCommonDoc";
                    #endregion
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
        public ActionResult DeleteCourseDocument(long Id)
        {
            try
            {
                var courseDocEntity = _ITrainerUploadDocumentService.GetRecordById(Id);

                if (courseDocEntity != null)
                {
                    _ITrainerUploadDocumentService.DeleteRecord(Id);

                    string TrainerDocumentdir = "TrainerDocument";
                    if (courseDocEntity.Status == 0)
                        TrainerDocumentdir = "TrainerUploadDocuments";
                    else if (courseDocEntity.Status == 1)
                        TrainerDocumentdir = "TrainerUploadvideo";
                    else if (courseDocEntity.Status == 3)
                        TrainerDocumentdir = "ConfindentialDocument";
                    else
                        TrainerDocumentdir = "TrainerUploadCommonDoc";
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


                    return Json(new { result = true, message = "Successfully delete Document" });
                }
                else
                    return Json(new { result = false, message = "No details found." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "DeleteCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
    }
}