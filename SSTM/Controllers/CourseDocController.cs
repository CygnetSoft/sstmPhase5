using Newtonsoft.Json;
using RestSharp;
using SSTM.Business.Interfaces;
using SSTM.Core.CourseDocument;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.ActivityLog;
using SSTM.Models.AWS;
using SSTM.Models.CourseDocument;
using SSTM.Models.CourseDocVersion;
using SSTM.Models.CourseSharing;
using SSTM.Models.Zoho;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire]
    public class CourseDocController : BaseController
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        private readonly IActivityLogService _IActivityLogService;

        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ICourseDocVersionService _ICourseDocVersionService;
        private readonly ICourseDocRemarksService _ICourseDocRemarksService;
        private readonly ICourseSharingService _ICourseSharingService;

        private readonly ICourseStatusService _ICourseStatusService;
        private readonly ICourseService _ICourseService;
        private readonly ICourseAssignmentService _ICourseAssignmentService;
        private readonly IMainCourseService _IMainCourseService;
        private readonly ICourseTrackersService _ICourseTrackersService;
        private readonly ICentralizedMasterService _ICentralizedMasterService;
        private readonly ICentralizedDocumentFilesService _ICentralizedDocumentFilesService;
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }

        private static string documentPath = "";
        #endregion

        #region Class Properties Definitions
        public CourseDocController
            (IExceptionLogService exceptionLogService, IConfigService configService, IUserService userService,
            IActivityLogService activityLogService, ICourseDocumentService courseDocumentService,
            ICourseDocVersionService courseDocVersionService, ICourseDocRemarksService courseDocRemarksService,
            ICourseSharingService courseSharingService, ICourseStatusService courseStatusService, ICourseService courseService,
            ICourseAssignmentService courseAssignmentService, IMainCourseService MainCourseService,
             ICourseTrackersService CourseTrackersService, ICentralizedMasterService centralizedMasterService,
             ICentralizedDocumentFilesService CentralizedDocumentFilesService)
        {
            _IExceptionLogService = exceptionLogService;
            _IConfigService = configService;
            _IUserService = userService;
            _IActivityLogService = activityLogService;
            _ICentralizedMasterService = centralizedMasterService;
            _ICourseDocumentService = courseDocumentService;
            _ICourseDocVersionService = courseDocVersionService;
            _ICourseDocRemarksService = courseDocRemarksService;
            _ICourseSharingService = courseSharingService;

            _ICourseStatusService = courseStatusService;
            _ICourseService = courseService;
            _ICourseAssignmentService = courseAssignmentService;

            _IMainCourseService = MainCourseService;
            _ICourseTrackersService = CourseTrackersService;
            _ICentralizedDocumentFilesService = CentralizedDocumentFilesService;
        }
        #endregion

        [HttpPost]
        public ActionResult OpenCourseDocumentsList(long courseId, string courseType, bool MasterCourse)
        {
            try
            {
                var list = _ICourseDocumentService.GetListByCourseId(courseId, MasterCourse).ToList();

                if (CurrentSession.UserRole == "HR")
                {
                    foreach (var item in list.ToList())
                    {
                        if (item.isDeleted)
                            list.Remove(item);
                    }
                }
                if (courseType == "other")
                {
                    Get_CourseAndSubCourse("other", true, 0);
                }
                if (courseType == "isoedu")
                {
                    Get_CourseAndSubCourse("isoedu", true, 0);
                }
                if (courseType == "NewCourse")
                {
                    Get_CourseAndSubCourse("NewCourse", true, 0);
                }
                else
                {
                    Get_CourseAndSubCourse("staff", true, 0);
                }
                if (CurrentSession.UserRole == "SME" || CurrentSession.UserRole == "HR")
                    return PartialView("_SMECourseDocuments", list);
                else
                    return PartialView("_AddOrUpdateCourseDocs", list);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "OpenCourseDocumentsList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        public void Get_CourseAndSubCourse(string CourseType, bool MasterCourse, long MasterCoursId)
        {
            var list = new List<SelectListItem>();

            var statusList = _ICourseService.Get_CourseAndSubCourse(CourseType, MasterCourse, MasterCoursId, 0).ToList();

            statusList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.CourseName });
            });
            ViewBag.MainCourseList = list;

        }

        #region Course in document move

        //public string GetCouseList(string CourseType, long MasterCourseId)
        //{
        //    var statusList = _ICourseService.GetMainCourseToCourseList(CourseType, MasterCourseId).ToList();
        //    StringBuilder sBuilder = new StringBuilder();
        //    sBuilder.Append("<option value = '' >--Manage Course--</ option >");
        //    statusList.ForEach(a =>
        //    {
        //        sBuilder.Append("<option value = " + a.Id.ToString() + " >" + a.CourseName + "</ option >");
        //    });
        //    return sBuilder.ToString();
        //}

        public string UpdateCourseAndDocument(long courseId, long DocumentId, long OldcourseId, string FileName)
        {
            //var entity = _ICourseService.GetRecordById(courseId);
            //entity.MasterCoursId = courseId;
            //var courseIds = _ICourseService.SaveRecord(entity);

            var entity1 = _ICourseDocumentService.GetRecordById(DocumentId);
            entity1.CourseId = courseId;
            var DocumentIds = _ICourseDocumentService.SaveRecord(entity1);

            #region Create AWS cloud directory


            if (DocumentId != 0 && courseId != 0 && OldcourseId != 0)
            {
                var configEntity = _IConfigService.GetFirstRecord();

                #region get single file in local 


                //
                var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());
                var bucketDirectory = string.Empty;
                if (Directory.Exists(sourceDir))
                    Directory.Delete(sourceDir, true);

                Directory.CreateDirectory(sourceDir);

                string filePath = Path.Combine(sourceDir, FileName);

                System.IO.File.Create(filePath).Close();

                AWSHelper.GetSingleFile(new AWSModel
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = OldcourseId.ToString(),
                    FileName = FileName,
                    FilePath = filePath
                });
                #endregion

                #region File move one course to other course

                AWSHelper.moveFile(new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = courseId.ToString(),
                    OldBucketDirectory = OldcourseId.ToString(),
                    FilePath = filePath,
                    FileName = FileName,
                });
                #endregion
            }
            #endregion

            return "success";
        }
        #endregion
        [HttpPost]
        public ActionResult OpenCourseDocsRemarksList(long CourseId, bool MasterCourse)
        {
            try
            {
                var list = _ICourseDocumentService.GetListByCourseId(CourseId, MasterCourse).ToList();

                return PartialView("_SMECourseDocuments", list);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDocRemarks", "OpenCourseDocsRemarksList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }

        [HttpPost]
        public ActionResult OpenCourseDocsSharing(long courseId, bool MasterCourse)
        {
            try
            {
                var list = _ICourseDocumentService.GetListByCourseId(courseId, MasterCourse).ToList();

                foreach (var item in list.ToList())
                {
                    if (item.isDeleted)
                        list.Remove(item);
                }

                return PartialView("_CourseDocsSharing", list);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "OpenCourseDocsSharing", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }


        public ActionResult GetCourseDocFile(long d, int s)
        {
            try
            {
                documentPath = "";

                var fileName = string.Empty;
                var courseId = string.Empty;
                var bucketDirectory = string.Empty;
                long docId = 0;
                string docName = string.Empty;

                if (s == 1)
                {
                    var courseDocEntity = _ICourseDocumentService.GetRecordById(d);
                    fileName = courseDocEntity.Filename;
                    courseId = courseDocEntity.CourseId.ToString();
                    bucketDirectory = courseDocEntity.CourseId.ToString();
                    docId = courseDocEntity.Id;
                    docName = courseDocEntity.DocName;
                }
                else
                {
                    var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordByDocId(d);
                    fileName = courseDocRemarksEntity.ReferenceFile;
                    courseId = courseDocRemarksEntity.CourseId.ToString();
                    bucketDirectory = Path.Combine(courseDocRemarksEntity.CourseId.ToString(), "_ReferenceDocs");
                    docId = courseDocRemarksEntity.DocId;
                    docName = fileName;
                }

                if (fileName != "N/A")
                {
                    #region Save record into Course Document table With Document status
                    var courseDocEntity1 = _ICourseDocumentService.GetRecordById(docId);
                    if ((Convert.ToString(courseDocEntity1.UserId) != "" || courseDocEntity1.UserId == 0) && Convert.ToInt64(courseDocEntity1.UserId) == CurrentSession.UserId
                        && courseDocEntity1.isOpened)
                    {
                        courseDocEntity1.isOpened = false;
                        courseDocEntity1.UpdatedBy = CurrentSession.UserId;
                        courseDocEntity1.UpdatedOn = DateTime.Now;
                        _ICourseDocumentService.SaveRecord(courseDocEntity1);

                        courseDocEntity1.isOpened = true;
                        courseDocEntity1.UpdatedBy = CurrentSession.UserId;
                        courseDocEntity1.UpdatedOn = DateTime.Now;
                        _ICourseDocumentService.SaveRecord(courseDocEntity1);
                        UtilityHelper.CurrentUserloginId = CurrentSession.UserId;
                    }
                    else if (!courseDocEntity1.isOpened)
                    {
                        courseDocEntity1.isOpened = true;
                        courseDocEntity1.UserId = CurrentSession.UserId;
                        courseDocEntity1.UpdatedBy = CurrentSession.UserId;
                        courseDocEntity1.UpdatedOn = DateTime.Now;
                        var newId = _ICourseDocumentService.SaveRecord(courseDocEntity1);
                        UtilityHelper.CurrentUserloginId = CurrentSession.UserId;
                    }
                    else
                    {
                        //Response.Write("<script language='javascript'>alert('Document is currently in use and opened by the other user.'); { window.close(); }</script>");
                        return Content("<script language='javascript' type='text/javascript'>alert('Document is currently in use and opened by the other user.');</script>");
                    }
                    #endregion

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

                        string awsurl = UtilityHelper.amazonlinkUrl + courseId + "/" + fileName;//md 28-06-2021
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
                        return View("CourseDocViewer");
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

        [HttpPost]
        public ActionResult SaveCourseDocument(long Id, string DocName, long CourseId, int currentcoursestatus, int isreplace, string version, string versiondate)
        {

            try
            {
                if (DocName != "")
                {
                    if (version == "")
                        return Json(new { resulf = false, message = "Version Required ...!" });

                    if (versiondate == "")
                        return Json(new { resulf = false, message = "Version Date Required ...!" });


                    CourseDocument coursedoc = new CourseDocument();
                    if (isreplace == 0)
                    {
                        if (_ICourseDocumentService.isExistsDocNameForCourse(DocName, CourseId, Id))
                            return Json(new { resulf = false, message = "Document name is already exits within the same course." });
                    }
                    else
                    {
                        coursedoc = _ICourseDocumentService.GetRecordById(Id);
                    }
                    string fileName = null;
                    if (Request.Files.Count > 0)
                    {
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = files[0];

                        var fileExtension = Path.GetExtension(file.FileName).ToLower();

                        if (fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".docx" || fileExtension == ".pptx" || fileExtension == ".ppt" ||
                            fileExtension == ".xlsx" || fileExtension == ".xls" || fileExtension.ToLower() == ".mp4" ||
                             fileExtension.ToLower() == ".ogg")
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
                            if (isreplace == 0)
                            {
                                Random generator = new Random();
                                //fileName = generator.Next(0, 1000000).ToString("D10") + Path.GetExtension(fileName);
                                fileName = DocName.Replace(" ", "") + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + Path.GetExtension(fileName);
                            }
                            else
                            {
                                if (Path.GetExtension(coursedoc.Filename) == Path.GetExtension(fileName))
                                {
                                    fileName = coursedoc.Filename;
                                }
                                else
                                {
                                    return Json(new { resulf = false, message = "Document extension not match same extention file upload like .docs , .pdf ...!" });
                                }
                            }


                            var configEntity = _IConfigService.GetFirstRecord();
                            AWSModel awsModel = new AWSModel()
                            {
                                AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                                SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                                BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                                BucketDirectory = CourseId.ToString(),
                                FileName = fileName,
                                LocalFileStream = file.InputStream
                            };

                            AWSHelper.UploadFile(awsModel);
                            #endregion

                            #region Save record into Course Document table
                            var courseDocEntity = _ICourseDocumentService.GetRecordById(Id);
                            if (courseDocEntity == null)
                            {
                                courseDocEntity = new CourseDocumentModel
                                {
                                    CourseId = CourseId,
                                    DocName = DocName,
                                    Filename = fileName,
                                    isCompleted = currentcoursestatus == 7 ? false : true,
                                    isActive = true,
                                    isApproved = currentcoursestatus == 7 ? true : false,
                                    isReassigned = false,
                                    isDeleted = false,
                                    CreatedBy = CurrentSession.UserId,
                                    CreatedOn = DateTime.Now
                                }.ToEntity();
                            }
                            else
                            {
                                courseDocEntity.DocName = DocName;
                                courseDocEntity.Filename = fileName;
                                courseDocEntity.isCompleted = currentcoursestatus == 7 ? false : true;
                                courseDocEntity.isReassigned = false;
                                courseDocEntity.UpdatedBy = CurrentSession.UserId;
                                courseDocEntity.UpdatedOn = DateTime.Now;
                            }

                            var newId = _ICourseDocumentService.SaveRecord(courseDocEntity);
                            #endregion

                            #region Updating document versioning info
                            if (isreplace == 0)
                            {
                                string courseDocVersion = "1";
                                DateTime dt;
                                try
                                {
                                    dt = Convert.ToDateTime(versiondate);
                                }
                                catch (Exception)
                                {
                                    dt = DateTime.Now;
                                }

                                var courseDocVersionEntity = _ICourseDocVersionService.GetLatestRecordByDocId(courseDocEntity.Id);
                                if (courseDocVersionEntity != null)
                                {
                                    //courseDocVersion = version; //courseDocVersionEntity.Version;
                                    courseDocVersionEntity.Version = version;
                                    courseDocVersionEntity.VersionDate = versiondate != "" ? dt : DateTime.Now;
                                    courseDocVersionEntity.isActive = false;
                                    courseDocVersionEntity.UpdatedBy = CurrentSession.UserId;
                                    courseDocVersionEntity.UpdatedOn = DateTime.Now;

                                    _ICourseDocVersionService.SaveRecord(courseDocVersionEntity);

                                    //courseDocVersion = courseDocVersion + 1;
                                }


                                _ICourseDocVersionService.SaveRecord(new CourseDocVersionModel()
                                {
                                    AuthorId = CurrentSession.UserId,
                                    DocId = courseDocEntity.Id,
                                    FileName = fileName,
                                    Version = version,

                                    VersionDate = versiondate != "" ? dt : DateTime.Now,
                                    isActive = true,
                                    CreatedBy = CurrentSession.UserId,
                                    CreatedOn = DateTime.Now
                                }.ToEntity());
                            }
                            #endregion

                            #region Checking for available document remarks
                            var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordByDocId(Id);

                            if (courseDocRemarksEntity != null)
                            {
                                if (courseDocRemarksEntity.Remarks != null)
                                {
                                    courseDocRemarksEntity.Remarks =
                                        "<b style='background-color: rgb(255, 255, 0);'>" +
                                            "Below points are covered and completed by developer on: " +
                                                UtilityHelper.GetCurrentSGTDate().ToString("dd/MM/yyyy hh:mm tt") +
                                        "</b><br/><br/>" + courseDocRemarksEntity.Remarks;
                                }

                                if (courseDocRemarksEntity.Suggestion != null)
                                {
                                    courseDocRemarksEntity.Suggestion =
                                        "<b style='background-color: rgb(255, 255, 0);'>" +
                                            "Below points are covered and completed by developer on: " +
                                                UtilityHelper.GetCurrentSGTDate().ToString("dd/MM/yyyy hh:mm tt") +
                                        "</b><br/><br/>" + courseDocRemarksEntity.Suggestion;
                                }

                                courseDocRemarksEntity.isCompleted = true;
                                courseDocRemarksEntity.UpdatedBy = CurrentSession.UserId;
                                courseDocRemarksEntity.UpdatedOn = DateTime.Now;

                                _ICourseDocRemarksService.SaveRecord(courseDocRemarksEntity);
                            }
                            #endregion

                            return Json(new { result = true, FileName = fileName, Id = newId });
                        }
                        else
                            return Json(new { result = false, message = AppMessages.InvalidFileExtention });
                    }
                    else
                        return Json(new { result = false, message = AppMessages.NoFileSelected });
                }
                else
                    return Json(new { result = false, message = AppMessages.BlankDocumentName });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "SaveCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult SkipCourseDocument(long Id)
        {
            try
            {
                #region Update course document table
                var courseDocEntity = _ICourseDocumentService.GetRecordById(Id);
                courseDocEntity.DocName = "N/A";
                courseDocEntity.isCompleted = false;
                courseDocEntity.isApproved = false;
                courseDocEntity.isReassigned = false;
                courseDocEntity.isActive = false;
                courseDocEntity.isDeleted = true;
                courseDocEntity.UpdatedBy = CurrentSession.UserId;
                courseDocEntity.UpdatedOn = DateTime.Now;

                _ICourseDocumentService.SaveRecord(courseDocEntity);
                #endregion

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "SkipCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult ReattachCourseDocument(long Id, string DocName)
        {
            try
            {
                if (DocName != "")
                {
                    var courseDocEntity = _ICourseDocumentService.GetRecordById(Id);
                    courseDocEntity.DocName = DocName;
                    courseDocEntity.isActive = true;
                    courseDocEntity.isCompleted = true;
                    courseDocEntity.isDeleted = false;
                    courseDocEntity.UpdatedBy = CurrentSession.UserId;
                    courseDocEntity.UpdatedOn = DateTime.Now;

                    _ICourseDocumentService.SaveRecord(courseDocEntity);

                    return Json(new { result = true, DocName = DocName });
                }
                else
                    return Json(new { result = false, message = "Document name cannot be blank." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "ReattachCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult DeleteCourseDocument(long Id)
        {
            try
            {
                var courseDocEntity = _ICourseDocumentService.GetRecordById(Id);

                if (courseDocEntity != null)
                {
                    _ICourseDocumentService.DeleteRecord(Id);

                    #region Deleting files for course document version files
                    var courseDocVersionsList = _ICourseDocVersionService.GetListByDocId(courseDocEntity.Id).ToList();

                    foreach (var item in courseDocVersionsList)
                    {
                        _ICourseDocVersionService.DeleteRecord(item.Id);

                        var configEntity = _IConfigService.GetFirstRecord();
                        AWSModel awsModel = new AWSModel()
                        {
                            AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                            SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                            BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                            BucketDirectory = courseDocEntity.CourseId.ToString(),
                            FileName = item.FileName
                        };

                        AWSHelper.DeleteFile(awsModel);
                    }
                    #endregion

                    //Deleting course document remarks
                    _ICourseDocRemarksService.DeleteRecord(courseDocEntity.Id);

                    //Deleting course document sharing 
                    _ICourseSharingService.DeleteRecordByDocId(courseDocEntity.Id);

                    return Json(new { result = true });
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

        public ActionResult MoveOldCourseDocument(long Id)
        {
            try
            {
                var courseDocEntity = _ICourseDocumentService.GetRecordById(Id);

                if (courseDocEntity != null)
                {
                    courseDocEntity.isOldDocument = "Old";
                    courseDocEntity.UpdatedBy = CurrentSession.UserId;
                    courseDocEntity.UpdatedOn = DateTime.Now;
                    courseDocEntity.isOpened = false;
                    _ICourseDocumentService.SaveRecord(courseDocEntity);
                    return Json(new { result = true });
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

        [HttpPost]
        public ActionResult SubmitCourseDocuments(long CourseId, bool MasterCourse)
        {
            try
            {
                var courseStatusList = _ICourseStatusService.GetList().ToList();
                var courseEntity = _ICourseService.GetRecordById(CourseId);
                var courseDocsList = _ICourseDocumentService.GetListByCourseId(CourseId, MasterCourse).ToList();

                var isProceed = true;

                if (CurrentSession.UserRole == "Developer")
                {
                    if (courseDocsList.Where(a => !a.isCompleted && !a.isApproved && a.isActive && !a.isDeleted).Count() > 0)
                        isProceed = false;
                }

                if (CurrentSession.UserRole == "SME")
                {
                    if (courseDocsList.Where(a => !a.isApproved).Count() > 0)
                        isProceed = false;
                }

                if (isProceed)
                {
                    #region Update status in Couse table in Db
                    if (courseEntity.Statusid == _ICourseStatusService.GetRecordIdByName("Pending"))
                        courseEntity.Statusid = _ICourseStatusService.GetRecordIdByName("Submitted");
                    else if (courseEntity.Statusid == _ICourseStatusService.GetRecordIdByName("Under Improvement"))
                        courseEntity.Statusid = _ICourseStatusService.GetRecordIdByName("Under Review");

                    courseEntity.UpdatedBy = CurrentSession.UserId;
                    courseEntity.UpdatedOn = DateTime.Now;

                    _ICourseService.SaveRecord(courseEntity);
                    #endregion

                    #region Save record into course assignment table
                    long directorId = 0;
                    long smeId = 0;
                    if (CurrentSession.UserRole == "Developer" || CurrentSession.UserRole == "Administration" || CurrentSession.UserRole == "Staff" || CurrentSession.UserRole == "Director" || CurrentSession.UserRole == "Aassociate Developer")
                    {
                        var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(CourseId);
                        if (courseAssignmentEntity != null)
                        {
                            directorId = courseAssignmentEntity.DirectorId;
                            smeId = courseAssignmentEntity.SMEId;

                            courseAssignmentEntity.DeveloperId = string.IsNullOrEmpty(courseEntity.NewCourseId.ToString()) ? CurrentSession.UserId : Convert.ToInt64(courseEntity.NewCourseId);
                            courseAssignmentEntity.UpdatedBy = CurrentSession.UserId;
                            courseAssignmentEntity.UpdatedOn = DateTime.Now;
                            _ICourseAssignmentService.SaveRecord(courseAssignmentEntity);
                        }
                    }
                    #endregion

                    var configEntity = _IConfigService.GetFirstRecord();
                    if (configEntity != null)
                    {
                        #region Trackers 
                        var Trackerentity = _ICourseTrackersService.GetDocument(CourseId);
                        Trackerentity.UpdateDated = DateTime.Now;
                        Trackerentity.submitedUserId = (int)CurrentSession.UserId;
                        Trackerentity.submitedDate = DateTime.Now;
                        _ICourseTrackersService.SaveRecord(Trackerentity);
                        #endregion

                        var smeEntity = _IUserService.GetRecordById(smeId);

                        #region Send email notification to director
                        var directorEntity = _IUserService.GetRecordById(directorId);
                        if (directorEntity != null)
                        {
                            var toEmails = UtilityHelper.Decrypt(directorEntity.Email);

                            var documents = "";
                            var courseDocumentsList = _ICourseDocumentService.GetListByCourseId(courseEntity.Id, MasterCourse).ToList();
                            courseDocumentsList.ForEach(a => { documents += a.DocName != "N/A" ? a.DocName + ", " : ""; });

                            var emailBody = UtilityHelper.GetEmailTemplate("NewDocNotificationToDirector.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", directorEntity.FirstName + " " + directorEntity.LastName)
                                .Replace("@DeveloperName@", CurrentSession.UserName)
                                .Replace("@CourseName@", courseEntity.CourseName)
                                .Replace("@Documents@", documents.Trim().TrimEnd(','));

                            if (smeEntity != null)
                            {
                                emailBody = emailBody.Replace("@SME@", smeEntity.FirstName + " " + smeEntity.LastName);

                                toEmails = ";" + UtilityHelper.Decrypt(smeEntity.Email);
                            }
                            else
                                emailBody = emailBody.Replace("@SME@", "Not assigned yet");

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "teversafe@gmail.com" : toEmails,
                                Subject = "SSTM COURSE DOCUMENTS UPDATES (" + courseEntity.CourseName + ")",
                                Message = emailBody,
                                SMTPHost = configEntity.Host,
                                SMTPPort = configEntity.Port,
                                SMTPEmail = configEntity.Email,
                                SMTPPassword = configEntity.Pass,
                                EnableSsl = configEntity.EnableSsl
                            });
                        }
                        #endregion
                    }

                    return Json(new { result = true, message = "All documents for " + courseEntity.CourseName + " submitted to the Director and notification sent successfully." });
                }
                else
                    return Json(new { result = false, message = "Please complete all the documents first before submitting it." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "SubmitCourseDocuments", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult LoadCourseDocument(long courseId, long docId, string docType)
        {
            try
            {
                #region Get document details
                var filePath = string.Empty;
                var fileName = string.Empty;
                var bucketDirectory = string.Empty;

                if (docType == "Course")
                {
                    var courseDocEntity = _ICourseDocumentService.GetRecordById(docId);
                    if (courseDocEntity != null)
                    {
                        var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());

                        if (Directory.Exists(sourceDir))
                            Directory.Delete(sourceDir, true);

                        Directory.CreateDirectory(sourceDir);

                        //fileName = "ACS-(Ben)-PPT-PDF_ClassRoom_29032023_63329.pdf";//courseDocEntity.Filename;new
                        fileName = courseDocEntity.Filename;//old
                        filePath = Path.Combine(sourceDir, fileName);

                        System.IO.File.Create(filePath).Close();

                        bucketDirectory = courseId.ToString();
                    }
                }
                else
                {
                    var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordByDocId(docId);
                    if (courseDocRemarksEntity != null)
                    {
                        var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());

                        if (Directory.Exists(sourceDir))
                            Directory.Delete(sourceDir, true);

                        Directory.CreateDirectory(sourceDir);

                        fileName = courseDocRemarksEntity.ReferenceFile;
                        filePath = Path.Combine(sourceDir, courseDocRemarksEntity.ReferenceFile);

                        System.IO.File.Create(filePath).Close();

                        bucketDirectory = Path.Combine(courseId.ToString(), "_ReferenceDocs");
                    }
                }
                #endregion

                #region Download file from AWS cloud on server directory to view
                var configEntity = _IConfigService.GetFirstRecord(); //old
                AWSModel awsModel = new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = bucketDirectory,
                    FileName = fileName,
                    FilePath = filePath
                };

                if (AWSHelper.GetSingleFile(awsModel))
                {
                    var encryptedString = "?dt=" + docType + "&s=" + UtilityHelper.Encrypt(awsModel.FileName) +
                        "&c=" + UtilityHelper.Encrypt(courseId.ToString()) +
                        "&d=" + UtilityHelper.Encrypt(docId.ToString());

                    //var encryptedString = "/ViewerJS/#../Content/Temp/" + CurrentSession.UserId.ToString() + "/" + Path.GetFileName(filePath);

                    //var encryptedString = "?dt=" + docType + "&s=" + UtilityHelper.Encrypt(fileName) + //new
                    //"&c=" + UtilityHelper.Encrypt(courseId.ToString()) +
                    //"&d=" + UtilityHelper.Encrypt(docId.ToString());
                    return Json(new { result = true, fs = encryptedString });
                }
                else
                    return Json(new { result = false, message = AppMessages.Exception });
                #endregion
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "LoadCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
        public ActionResult CourseDocViewer(string dt, string s, string c, string d) //old
        {
            try
            {
                if (s != null && c != null && d != null)
                {
                    var fileName = UtilityHelper.Decrypt(s);
                    var filePath = "~/Content/Temp/" + CurrentSession.UserId.ToString() + "/" + fileName;

                    ViewBag.CourseId = UtilityHelper.Decrypt(c);
                    ViewBag.DocId = UtilityHelper.Decrypt(d);
                    ViewBag.DocName = filePath;
                    //new start
                    string ext = System.IO.Path.GetExtension(fileName);
                    if (ext.ToLower() == ".webm")
                    {
                        ViewBag.videofomat = "webm";
                        ViewBag.pdffullpath = "";
                        ViewBag.fullpath = "";
                        ViewBag.VideoPath = "https://sstmtest.s3.ap-southeast-1.amazonaws.com/" + ViewBag.CourseId + "/" + fileName;
                    }
                    else
                    {
                        ViewBag.VideoPath = "";
                        ViewBag.pdffullpath = "";
                        ViewBag.fullpath = filePath;
                    }
                    //END new

                    return View("CourseDocViewer");
                }
                else
                    return Content("<div class='alert alert-danger'>File not found.</div>");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "CourseDocViewer", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }


        public ActionResult Central_CourseDocViewer(string path, string type, int courseid, string status = "view") //old
        {
            try
            {
                var master = _ICentralizedMasterService.GetRecordById(courseid);

                if (path != null)
                {
                    #region Download file from AWS cloud on server directory to view
                    var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", master.FolderNameOutput);

                    if (Directory.Exists(sourceDir))
                        Directory.Delete(sourceDir, true);

                    Directory.CreateDirectory(sourceDir);


                    string filePath = Path.Combine(sourceDir, path.Trim());

                    System.IO.File.Create(filePath).Close();


                    var configEntity = _IConfigService.GetFirstRecord(); //old
                    AWSModel awsModel = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = "CentralCourse/CourseOutputFiles/" + master.FolderNameOutput.Trim(),
                        FileName = path.Trim(),
                        FilePath = filePath
                    };

                    AWSHelper.GetSingleFile(awsModel);
                    #endregion

                    // string filePath = "";

                    filePath = Request.IsLocal ? UtilityHelper.localUrl + "Content/Temp/" + master.FolderNameOutput + "/" + path.Trim() : UtilityHelper.SiteUrl + "Content/Temp/" + master.FolderNameOutput + "/" + path.Trim();

                    if (System.IO.File.Exists(Server.MapPath("~/TempFiles/" + System.IO.Path.GetFileNameWithoutExtension(path) + ".pdf")))
                        System.IO.File.Delete(Server.MapPath("~/TempFiles/" + System.IO.Path.GetFileNameWithoutExtension(path) + ".pdf"));

                    ViewBag.CourseId = 0;
                    ViewBag.DocId = 0;
                    ViewBag.DocName = filePath;
                    ViewBag.fullpath = filePath;
                    ViewBag.pdffullpath = filePath;

                    var extension = Path.GetExtension(filePath);
                    var mimeType = UtilityHelper.GetMimeTypeFromExtension(extension);
                    if (CurrentSession.UserRole == "Trainer" || status == "Edit")
                    {
                        if (extension.ToLower() == ".ppt" || extension.ToLower() == ".pptx" || extension.ToLower() == ".doc" || extension.ToLower() == ".docx")
                        {
                            ZohoResponseModel CRMDateApiResponse = new ZohoResponseModel();
                            //var configEntity = _IConfigService.GetFirstRecord(); //old
                            string awsurl = UtilityHelper.amazonlinkUrl + "CentralCourse/CourseOutputFiles/" + master.FolderNameOutput.Trim() + "/" + path.Trim();//md 28-06-2021

                            IRestResponse res = null;

                            res = ZohoEditorCentral.GetZohoEditor(master.FolderNameInput, "~/Content/CenteralizedCourseSamplePPT/ppt/", extension, path, path.Trim(), CurrentSession.UserId.ToString(), UtilityHelper.Decrypt(configEntity.ZohoApiKey), awsurl);

                            CRMDateApiResponse = JsonConvert.DeserializeObject<ZohoResponseModel>(res.Content);

                            if (!string.IsNullOrEmpty(CRMDateApiResponse.document_url))
                            {
                                //return Redirect(CRMDateApiResponse.document_url);
                                ViewBag.zohoOpenUrl = CRMDateApiResponse.document_url;
                                return View("_viewzohoCentralDoc");
                            }
                            else
                            {
                                ViewBag.ZohoError = "Something went wrong. Please try again.";
                                return View("_viewzohoCentralDoc");
                            }
                        }
                    }
                    return View("CourseDocViewer");
                }
                else
                    return Content("<div class='alert alert-danger'>File not found.</div>");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Central CourseDoc view", "Central CourseDocViewer", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }


        public ActionResult Central_raw_CourseDocViewer(int courseid)
        {
            try
            {
                var master = _ICentralizedMasterService.GetRecordById(courseid);

                #region Download file from AWS cloud on server directory to view
                var sourceDir = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx");

                //if (System.IO.File.Exists(Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT1.pptx")))
                //    System.IO.File.Delete(Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT1.pptx"));

                //if (System.IO.File.Exists(sourceDir))
                //    System.IO.File.Move(sourceDir, Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT1.pptx"));

                //Directory.CreateDirectory(sourceDir);
                string filePathlocal = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx");

                System.IO.File.Create(filePathlocal).Close();
                #endregion

                var configEntity = _IConfigService.GetFirstRecord(); //old
                AWSModel awsModel = new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = "CentralCourse/CourseOutputFiles/" + master.FolderNameOutput.Trim(),
                    FileName = "CentralizePPT.pptx",
                    FilePath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx")//UtilityHelper.SiteUrl +
                };

                try
                {
                    bool status = AWSHelper.GetSingleFile(awsModel);
                    if (status == false)
                        System.IO.File.Move(Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT1.pptx"), sourceDir);
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx")))
                        System.IO.File.Delete(Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx"));

                    System.IO.File.Copy(Server.MapPath("~/Content/CenteralizedCourseSamplePPT/RAW_DOC/" + master.choose_type + "/CentralizePPT.pptx"), Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx"));
                    FileStream fs = new FileStream(Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx"), FileMode.Open);
                    AWSModel awsModel1 = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = "CentralCourse/CourseOutputFiles/" + master.FolderNameOutput.Trim(),
                        FileName = "CentralizePPT.pptx",
                        LocalFileStream = fs
                    };

                    AWSHelper.UploadFile(awsModel1);
                    fs.Close();
                }


                if (master.FolderNameInput != null)
                {

                    // string filePath = "";
                    string filePath = UtilityHelper.SiteUrl + "Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + master.FolderNameInput + "/CentralizePPT.pptx";
                    ViewBag.CourseId = 0;
                    ViewBag.DocId = 0;
                    ViewBag.DocName = filePath;
                    ViewBag.fullpath = filePath;
                    ViewBag.pdffullpath = filePath;

                    var extension = Path.GetExtension(filePath);
                    var mimeType = UtilityHelper.GetMimeTypeFromExtension(extension);

                    if (extension.ToLower() == ".ppt" || extension.ToLower() == ".pptx" || extension.ToLower() == ".doc" || extension.ToLower() == ".docx")
                    {
                        ZohoResponseModel CRMDateApiResponse = new ZohoResponseModel();

                        string awsurl = filePath;//md 28-06-2021

                        IRestResponse res = null;

                        res = ZohoEditorCentral.GetZohoEditor(master.FolderNameInput, "~/Content/CenteralizedCourseSamplePPT/ppt/", extension, "CentralizePPT.pptx", "CentralizePPT.pptx", CurrentSession.UserId.ToString(), UtilityHelper.Decrypt(configEntity.ZohoApiKey), awsurl);

                        CRMDateApiResponse = JsonConvert.DeserializeObject<ZohoResponseModel>(res.Content);

                        if (!string.IsNullOrEmpty(CRMDateApiResponse.document_url))
                        {
                            //return Redirect(CRMDateApiResponse.document_url);
                            ViewBag.zohoOpenUrl = CRMDateApiResponse.document_url;
                            return View("_viewzohoCentralDoc");
                        }
                        else
                        {
                            ViewBag.ZohoError = "Something went wrong. Please try again.";
                            return View("_viewzohoCentralDoc");
                        }
                    }
                }
                return View("CourseDocViewer");

            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Central CourseDoc view", "Central CourseDocViewer", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }

        [HttpPost]
        public ActionResult LoadCourseDocument_todayCourse(long courseId, long docId, string docType, long batchid)
        {
            try
            {
                #region Get document details
                var filePath = string.Empty;
                var fileName = string.Empty;
                var bucketDirectory = string.Empty;

                if (docType == "Course")
                {
                    var courseDocEntity = _ICourseDocumentService.GetRecordById(docId);
                    if (courseDocEntity != null)
                    {
                        var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());

                        if (Directory.Exists(sourceDir))
                            Directory.Delete(sourceDir, true);

                        Directory.CreateDirectory(sourceDir);

                        //fileName = "ACS-(Ben)-PPT-PDF_ClassRoom_29032023_63329.pdf";//courseDocEntity.Filename;new
                        fileName = courseDocEntity.Filename;//old
                        filePath = Path.Combine(sourceDir, fileName);

                        System.IO.File.Create(filePath).Close();

                        bucketDirectory = courseId.ToString();
                    }
                }
                else
                {
                    var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordByDocId(docId);
                    if (courseDocRemarksEntity != null)
                    {
                        var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());

                        if (Directory.Exists(sourceDir))
                            Directory.Delete(sourceDir, true);

                        Directory.CreateDirectory(sourceDir);

                        fileName = courseDocRemarksEntity.ReferenceFile;
                        filePath = Path.Combine(sourceDir, courseDocRemarksEntity.ReferenceFile);

                        System.IO.File.Create(filePath).Close();

                        bucketDirectory = Path.Combine(courseId.ToString(), "_ReferenceDocs");
                    }
                }
                #endregion

                #region Download file from AWS cloud on server directory to view
                var configEntity = _IConfigService.GetFirstRecord(); //old
                AWSModel awsModel = new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = bucketDirectory,
                    FileName = fileName,
                    FilePath = filePath
                };

                if (AWSHelper.GetSingleFile(awsModel))
                {
                    var encryptedString = "?dt=" + docType + "&s=" + UtilityHelper.Encrypt(awsModel.FileName) +
                        "&c=" + UtilityHelper.Encrypt(courseId.ToString()) +
                        "&d=" + UtilityHelper.Encrypt(docId.ToString()) +
                        "&bid=" + UtilityHelper.Encrypt(batchid.ToString());

                    //var encryptedString = "/ViewerJS/#../Content/Temp/" + CurrentSession.UserId.ToString() + "/" + Path.GetFileName(filePath);

                    //var encryptedString = "?dt=" + docType + "&s=" + UtilityHelper.Encrypt(fileName) + //new
                    //"&c=" + UtilityHelper.Encrypt(courseId.ToString()) +
                    //"&d=" + UtilityHelper.Encrypt(docId.ToString());
                    return Json(new { result = true, fs = encryptedString });
                }
                else
                    return Json(new { result = false, message = AppMessages.Exception });
                #endregion
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "LoadCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        public ActionResult CourseDocViewer_todaytrainer(string dt, string s, string c, string d, string bid) //old
        {
            try
            {
                if (s != null && c != null && d != null)
                {
                    var fileName = UtilityHelper.Decrypt(s);
                    var filePath = "~/Content/Temp/" + CurrentSession.UserId.ToString() + "/" + fileName;

                    ViewBag.CourseId = UtilityHelper.Decrypt(c);
                    ViewBag.Batchid = UtilityHelper.Decrypt(bid);
                    ViewBag.DocId = UtilityHelper.Decrypt(d);
                    ViewBag.DocName = filePath;
                    //new start
                    string ext = System.IO.Path.GetExtension(fileName);
                    if (ext.ToLower() == ".webm")
                    {
                        ViewBag.videofomat = "webm";
                        ViewBag.pdffullpath = "";
                        ViewBag.fullpath = "";
                        ViewBag.VideoPath = "https://sstmtest.s3.ap-southeast-1.amazonaws.com/" + ViewBag.CourseId + "/" + fileName;
                    }
                    else
                    {
                        ViewBag.VideoPath = "";
                        ViewBag.pdffullpath = "";
                        ViewBag.fullpath = filePath;
                    }
                    //END new

                    return View("../TodayClassDocs/CourseDocViewer");
                }
                else
                    return Content("<div class='alert alert-danger'>File not found.</div>");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "CourseDocViewer", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }
        //public ActionResult CourseDocViewer(string dt, string s, string c, string d) //new 
        //{
        //    try
        //    {
        //        if (s != null && c != null && d != null)
        //        {
        //            var fileName = UtilityHelper.Decrypt(s);
        //            var filePath = "~/Content/Temp/" + CurrentSession.UserId.ToString() + "/" + fileName;

        //            ViewBag.CourseId = UtilityHelper.Decrypt(c);
        //            ViewBag.DocId = UtilityHelper.Decrypt(d);
        //            ViewBag.DocName = filePath;
        //            string ext = System.IO.Path.GetExtension(fileName);
        //            if (ext.ToLower() == ".pdf")
        //            {
        //                ViewBag.pdffullpath = "https://sstmtest.s3.ap-southeast-1.amazonaws.com/" + ViewBag.CourseId + "/" + fileName;
        //                ViewBag.fullpath = "";
        //                ViewBag.VideoPath = "";
        //            }
        //            else
        //            {
        //                if (ext.ToLower() == ".webm")
        //                {
        //                    ViewBag.videofomat = "webm";
        //                    ViewBag.pdffullpath = "";
        //                    ViewBag.fullpath = "";
        //                    ViewBag.VideoPath = "https://sstmtest.s3.ap-southeast-1.amazonaws.com/" + ViewBag.CourseId + "/" + fileName;
        //                }
        //                else
        //                {
        //                    ViewBag.VideoPath = "";
        //                    ViewBag.pdffullpath = "";
        //                    ViewBag.fullpath = "https://sstmtest.s3.ap-southeast-1.amazonaws.com/" + ViewBag.CourseId + "/" + fileName;
        //                }
        //            }
        //            return View("CourseDocViewer");
        //        }
        //        else
        //            return Content("<div class='alert alert-danger'>File not found.</div>");
        //    }
        //    catch (Exception ex)
        //    {
        //        _IExceptionLogService.SaveRecord(ex, "CourseDoc", "CourseDocViewer", Request.Url.AbsoluteUri, CurrentSession.UserId);
        //        return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
        //    }
        //}

        [HttpPost]
        public ActionResult LoadCommonDocument(string path, string filename) //new
        {
            try
            {
                #region Get document details
                var filePath = string.Empty;
                var fileName = string.Empty;
                var bucketDirectory = string.Empty;


                var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());

                if (Directory.Exists(sourceDir))
                    Directory.Delete(sourceDir, true);

                Directory.CreateDirectory(sourceDir);

                fileName = filename;
                filePath = Path.Combine(sourceDir, filename);

                System.IO.File.Create(filePath).Close();

                bucketDirectory = path;

                #endregion

                #region Download file from AWS cloud on server directory to view
                var configEntity = _IConfigService.GetFirstRecord();
                AWSModel awsModel = new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = bucketDirectory,
                    FileName = fileName,
                    FilePath = filePath
                };

                if (AWSHelper.GetSingleFile(awsModel))
                {
                    var doc = "Doc";
                    var encryptedString = "?dt=" + doc + "&s=" + UtilityHelper.Encrypt(awsModel.FileName) + // UtilityHelper.Encrypt(filename) +//
                        "&c=" + UtilityHelper.Encrypt(path) +
                        "&d=" + UtilityHelper.Encrypt("0");

                    //var encryptedString = "/ViewerJS/#../Content/Temp/" + CurrentSession.UserId.ToString() + "/" + Path.GetFileName(filePath);

                    return Json(new { result = true, fs = encryptedString });
                }
                else
                    return Json(new { result = false, message = AppMessages.Exception });
                #endregion
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "LoadCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        //[HttpPost]
        //public ActionResult LoadCommonDocument(string path, string filename) //old
        //{
        //    try
        //    {
        //        #region Get document details
        //        var filePath = string.Empty;
        //        var fileName = string.Empty;
        //        var bucketDirectory = string.Empty;


        //        var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", CurrentSession.UserId.ToString());

        //        if (Directory.Exists(sourceDir))
        //            Directory.Delete(sourceDir, true);

        //        Directory.CreateDirectory(sourceDir);

        //        fileName = filename;
        //        filePath = Path.Combine(sourceDir, filename);

        //        System.IO.File.Create(filePath).Close();

        //        bucketDirectory = path;

        //        #endregion

        //        #region Download file from AWS cloud on server directory to view
        //        var configEntity = _IConfigService.GetFirstRecord();
        //        AWSModel awsModel = new AWSModel()
        //        {
        //            AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
        //            SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
        //            BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
        //            BucketDirectory = bucketDirectory,
        //            FileName = fileName,
        //            FilePath = filePath
        //        };

        //        if (AWSHelper.GetSingleFile(awsModel))
        //        {
        //            var doc = "Doc";
        //        var encryptedString = "?dt=" + doc + "&s=" + UtilityHelper.Encrypt(awsModel.FileName) +
        //            "&c=" + UtilityHelper.Encrypt(path) +
        //            "&d=" + UtilityHelper.Encrypt("0");

        //        //var encryptedString = "/ViewerJS/#../Content/Temp/" + CurrentSession.UserId.ToString() + "/" + Path.GetFileName(filePath);

        //        return Json(new { result = true, fs = encryptedString });
        //        }
        //        else
        //            return Json(new { result = false, message = AppMessages.Exception });
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        _IExceptionLogService.SaveRecord(ex, "CourseDoc", "LoadCourseDocument", Request.Url.AbsoluteUri, CurrentSession.UserId);
        //        return Json(new { result = false, message = AppMessages.Exception });
        //    }
        //}

        [HttpPost]
        public ActionResult AcceptCourseDocs(long courseId, long docId, long remarksId)
        {
            try
            {
                #region Save course document remarks
                var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordById(remarksId);
                if (courseDocRemarksEntity != null)
                {
                    courseDocRemarksEntity.isCompleted = true;
                    courseDocRemarksEntity.UpdatedBy = CurrentSession.UserId;
                    courseDocRemarksEntity.UpdatedOn = DateTime.Now;

                    _ICourseDocRemarksService.SaveRecord(courseDocRemarksEntity);
                }
                #endregion

                #region Update course document status
                var courseDocEntity = _ICourseDocumentService.GetRecordById(docId);
                courseDocEntity.isCompleted = false;
                courseDocEntity.isApproved = true;
                courseDocEntity.isReassigned = false;
                courseDocEntity.UpdatedBy = CurrentSession.UserId;
                courseDocEntity.UpdatedOn = DateTime.Now;

                _ICourseDocumentService.SaveRecord(courseDocEntity);
                #endregion

                #region Send notification to the Director and Developer in case course is not approved
                var courseEntity = _ICourseService.GetRecordById(courseId);

                var configEntity = _IConfigService.GetFirstRecord();
                if (configEntity != null)
                {
                    var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(courseId);
                    var directorEntity = _IUserService.GetRecordById(courseAssignmentEntity.DirectorId);
                    var developerEntity = _IUserService.GetRecordById(courseAssignmentEntity.DeveloperId);

                    if (directorEntity != null && developerEntity != null)
                    {
                        var emailBody = UtilityHelper.GetEmailTemplate("ApprovedCourseDocNotificatoinToDirectorAndDeveloper.html").ToString();
                        emailBody = emailBody.Replace("@CourseName@", courseEntity.CourseName)
                            .Replace("@DocumentName@", courseDocEntity.DocName)
                            .Replace("@DeveloperName@", developerEntity.FirstName + " " + developerEntity.LastName)
                            .Replace("@SMEName@", CurrentSession.UserName);

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "teversafe@gmail.com" : UtilityHelper.Decrypt(directorEntity.Email) + ";" + UtilityHelper.Decrypt(developerEntity.Email),
                            Subject = "SSTM COURSE ASSIGNMENTS UPDATES (" + courseEntity.CourseName + ")",
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

                #region Clear temp user directory on server
                var tempDir = Path.Combine(Server.MapPath("Content"), "Temp", CurrentSession.UserId.ToString());

                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
                #endregion

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "AcceptCourseDocs", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult RenameCourseDoc(long docId, string docName)
        {
            try
            {
                var courseDocEntity = _ICourseDocumentService.GetRecordById(docId);
                courseDocEntity.DocName = docName;
                courseDocEntity.UpdatedBy = CurrentSession.UserId;
                courseDocEntity.UpdatedOn = DateTime.Now;

                _ICourseDocumentService.SaveRecord(courseDocEntity);

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "RenameCourseDoc", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ViewCourseDocs(long courseId)
        {
            try
            {
                var list = new List<SharedCourseListModel>();

                if (CurrentSession.UserRole == "Trainer")
                    list = _ICourseSharingService.GetListofSharedCourseDocs(courseId).Where(a => a.isTraining).ToList();
                else if (CurrentSession.UserRole == "Print Incharge")
                    list = _ICourseSharingService.GetListofSharedCourseDocs(courseId).Where(a => a.isPrinting).ToList();
                else if (CurrentSession.UserRole == "Developer")
                    list = _ICourseSharingService.GetListofSharedCourseDocs(courseId).Where(a => a.isDeveloper).ToList();

                return PartialView("_ViewCourseDocsModal", list);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "OpenCourseDocumentsList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }
        [HttpPost]
        public ActionResult ViewTodayCourseDocs(long courseId, int isCentral = 0)
        {
            try
            {
                var list = new List<SharedCourseListModel>();
                list = _ICourseSharingService.GetListofTodayCourseDocs(courseId, isCentral).Where(a => a.isTraining).ToList();
                return PartialView("_ViewCourseDocsModal", list);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "OpenCourseDocumentsList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        [HttpPost]
        public ActionResult ViewTodayCourseRASWPDocs(long airlineCourseId)
        {
            try
            {
                var list = new List<SharedCourseListModel>();
                list = _ICourseSharingService.GetListofRASWPDocs(airlineCourseId).Where(a => a.isTraining).ToList();
                return PartialView("_ViewCourseDocsModal", list);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "OpenCourseDocumentsList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        [HttpPost]
        public ActionResult DocumentPrinted(long courseId, long docId, int printedPages)
        {
            try
            {
                var courseEntity = _ICourseService.GetRecordById(courseId);
                var courseDocEntity = _ICourseDocumentService.GetRecordById(docId);

                _IActivityLogService.SaveRecord(new ActivityLogModel()
                {
                    URL = Request.Url.AbsoluteUri,
                    Duration = "Course: " + courseEntity.CourseName + " | Document: " + courseDocEntity.DocName + " | Total Pages: " + printedPages,
                    Controller = "CourseDoc",
                    Action = "PrintCourseDoc",
                    CreatedOn = DateTime.Now,
                    UserId = CurrentSession.UserId
                }.ToEntity());

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "DocumentPrinted", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        public ActionResult showPdfFile(string filePath)
        {
            ViewBag.DocName = filePath;
            return View("CourseDocViewer");
        }

        [HttpGet]
        public ActionResult DownoadCourseDocFile(long d, int s)
        {
            try
            {
                documentPath = "";

                var fileName = string.Empty;
                var courseId = string.Empty;
                var bucketDirectory = string.Empty;

                if (s == 1)
                {
                    var courseDocEntity = _ICourseDocumentService.GetRecordById(d);
                    fileName = courseDocEntity.Filename;
                    courseId = courseDocEntity.CourseId.ToString();
                    bucketDirectory = courseDocEntity.CourseId.ToString();
                }
                else
                {
                    var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordByDocId(d);
                    fileName = courseDocRemarksEntity.ReferenceFile;
                    courseId = courseDocRemarksEntity.CourseId.ToString();
                    bucketDirectory = Path.Combine(courseDocRemarksEntity.CourseId.ToString(), "_ReferenceDocs");
                }

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

                    return File(System.IO.File.ReadAllBytes(filePath), mimeType, fileName);
                }
                else
                    return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "DownoadCourseDocFile", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return RedirectToAction("NotFound", "Error");
            }
        }

        [HttpGet]
        public ActionResult DownoadVersionDocFile(string filename, long CourseId)
        {
            try
            {
                documentPath = "";

                var fileName = string.Empty;
                var courseId = string.Empty;
                var bucketDirectory = string.Empty;

                fileName = filename;
                courseId = CourseId.ToString();
                bucketDirectory = CourseId.ToString();

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

                    return File(System.IO.File.ReadAllBytes(filePath), mimeType, fileName);
                }
                else
                    return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "DownoadCourseDocFile", Request.Url.AbsoluteUri, CurrentSession.UserId);
                throw ex;
            }
        }
        public ActionResult viewZoho()
        {
            return View();
        }

        public ActionResult OldDocuments()
        {

            try
            {
                var list = _ICourseDocumentService.GetListByOldCourse().ToList();
                return View(list);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "OldDocuments", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }

        }


        public ActionResult CentralCourseDocViewer_todaytrainer(int masterId, int docId)
        {
            try
            {
                var master = _ICentralizedMasterService.GetRecordById(masterId);
                var document = _ICentralizedDocumentFilesService.GetRecordById(docId);

                var sourceDir = Server.MapPath("~/Content/Temp/" + master.FolderNameOutput+"/");
                
                if (Directory.Exists(sourceDir))
                    Directory.Delete(sourceDir, true);

                Directory.CreateDirectory(sourceDir);

                string filePath = sourceDir;
                filePath = Path.Combine(sourceDir, document.Document_File_Name);

                System.IO.File.Create(filePath).Close();
                //AWS Server Get File
                string ext = Path.GetExtension(document.Document_File_Name);
                var configEntity = _IConfigService.GetFirstRecord(); //old
                AWSModel awsModel = new AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = "CentralCourse/CourseOutputFiles/" + master.FolderNameOutput.Trim(),
                    FileName = document.Document_File_Name.Trim(),
                    FilePath = filePath
                };

                AWSHelper.GetSingleFile(awsModel);
                //END GET FILE FROM AWS SERVER
                filePath = Request.IsLocal ? UtilityHelper.localUrl + "Content/Temp/" + master.FolderNameOutput + "/" + document.Document_File_Name.Trim() : UtilityHelper.SiteUrl + "Content/Temp/" + master.FolderNameOutput + "/" + document.Document_File_Name.Trim();
                ViewBag.VideoPath = "";
                ViewBag.pdffullpath = "";
                ViewBag.fullpath = filePath;
                ViewBag.loginuserid = CurrentSession.UserId;
                //END new

                return View("../TodayClassDocs/CourseDocViewer");

            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "CourseDocViewer", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }
    }
}