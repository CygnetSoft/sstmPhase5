using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.AWS;
using SSTM.Models.CourseDocRemarks;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class CourseDocRemarksController : BaseController
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;

        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ICourseDocRemarksService _ICourseDocRemarksService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public CourseDocRemarksController
            (IExceptionLogService exceptionLogService, IConfigService configService, IUserService userService,
            ICourseDocumentService courseDocumentService, ICourseDocRemarksService courseDocRemarksService)
        {
            _IExceptionLogService = exceptionLogService;
            _IConfigService = configService;
            _IUserService = userService;

            _ICourseDocumentService = courseDocumentService;
            _ICourseDocRemarksService = courseDocRemarksService;
        }
        #endregion

        [HttpPost]
        public ActionResult SaveCourseDocRemarks(CourseDocRemarksModel model, HttpPostedFileBase ReferenceDoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordById(model.Id);

                    var isValidFile = true;
                    var newFileName = string.Empty;

                    #region Upload reference file on AWS cloud
                    if (Request.Files != null && Request.Files.Count > 0)
                    {
                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = files[0];
                        model.ReferenceDoc = file;

                        string fileName = model.ReferenceDoc.FileName;
                        string fileExtension = Path.GetExtension(fileName).ToLower();

                        if (fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".docx" || fileExtension == ".pptx" ||
                            fileExtension == ".xlsx" || fileExtension == ".xls")
                        {
                            Random generator = new Random();
                            newFileName = generator.Next(0, 1000000).ToString("D10") + Path.GetExtension(fileName);

                            var configEntity = _IConfigService.GetFirstRecord();
                            AWSModel awsModel = new AWSModel()
                            {
                                AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                                SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                                BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                                BucketDirectory = Path.Combine(model.CourseId.ToString(), "_ReferenceDocs"),
                                FileName = newFileName,
                                LocalFileStream = model.ReferenceDoc.InputStream
                            };

                            if (courseDocRemarksEntity == null)
                                AWSHelper.UploadFile(awsModel);
                            else
                            {
                                AWSHelper.DeleteFile(awsModel);
                                AWSHelper.UploadFile(awsModel);
                            }
                        }
                        else
                            isValidFile = false;
                    }
                    #endregion

                    if (isValidFile)
                    {
                        #region Save course document remarks
                        if (courseDocRemarksEntity == null)
                        {
                            courseDocRemarksEntity = new CourseDocRemarksModel().ToEntity();
                            courseDocRemarksEntity.CourseId = model.CourseId;
                            courseDocRemarksEntity.DocId = model.DocId;
                            courseDocRemarksEntity.isCompleted = false;
                            courseDocRemarksEntity.isDeleted = false;
                            courseDocRemarksEntity.CreatedBy = CurrentSession.UserId;
                            courseDocRemarksEntity.CreatedOn = DateTime.Now;
                        }
                        else
                        {
                            courseDocRemarksEntity.UpdatedBy = CurrentSession.UserId;
                            courseDocRemarksEntity.UpdatedOn = DateTime.Now;
                        }

                        if (newFileName != "")
                            courseDocRemarksEntity.ReferenceFile = newFileName;

                        courseDocRemarksEntity.Remarks = model.Remarks;
                        courseDocRemarksEntity.Suggestion = model.Suggestion;

                        _ICourseDocRemarksService.SaveRecord(courseDocRemarksEntity);
                        #endregion

                        #region Update course document status
                        var courseDocEntity = _ICourseDocumentService.GetRecordById(model.DocId);
                        courseDocEntity.isCompleted = false;
                        courseDocEntity.isApproved = false;
                        courseDocEntity.isReassigned = true;
                        courseDocEntity.UpdatedBy = CurrentSession.UserId;
                        courseDocEntity.UpdatedOn = DateTime.Now;

                        _ICourseDocumentService.SaveRecord(courseDocEntity);
                        #endregion

                        #region Clear temp user directory on server
                        var tempDir = Path.Combine(Server.MapPath("Content"), "Temp", CurrentSession.UserId.ToString());

                        if (Directory.Exists(tempDir))
                            Directory.Delete(tempDir);
                        #endregion

                        return Json(new { result = true });
                    }
                    else
                        return Json(new { result = false, message = AppMessages.InvalidFileExtention });
                }
                else
                    return Json(new { result = false, message = "Please insert remarks and suggestion to continue." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDocRemarks", "SaveCourseDocRemarks", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        //public ActionResult ViewCourseDocumentRemarks(string dt, string s, string c, string d) //new
        //{
        //    try
        //    {
        //        if (s != null && c != null && d != null)
        //        {
        //            var fileName = UtilityHelper.Decrypt(s);
        //            var filePath = "~/Content/Temp/" + CurrentSession.UserId.ToString() + "/" + fileName;

        //            var courseId = UtilityHelper.Decrypt(c);
        //            var docId = UtilityHelper.Decrypt(d);

        //            var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordByDocId(Convert.ToInt32(docId)).ToModel();

        //            if (courseDocRemarksEntity == null)
        //            {
        //                courseDocRemarksEntity = new CourseDocRemarksModel();
        //                courseDocRemarksEntity.CourseId = Convert.ToInt32(courseId);
        //                courseDocRemarksEntity.DocId = Convert.ToInt32(docId);
        //            }

        //            courseDocRemarksEntity.DocName = filePath;

        //            var courseDocEntity = _ICourseDocumentService.GetRecordById(Convert.ToInt32(docId));
        //            ViewBag.DocName = courseDocEntity.DocName;
        //            ViewBag.DocType = dt;
        //            string ext = System.IO.Path.GetExtension(fileName);
        //            if (ext.ToLower() == ".pdf")
        //            {
        //                ViewBag.pdffullpath = "https://sstmtest.s3.ap-southeast-1.amazonaws.com/" + ViewBag.CourseId + "/" + fileName;
        //                ViewBag.fullpath = "";
        //            }
        //            else
        //            {
        //                ViewBag.pdffullpath = "";
        //                ViewBag.fullpath = "https://sstmtest.s3.ap-southeast-1.amazonaws.com/" + courseId + "/" + fileName;
        //            }

        //            return View("ReviewCourseDocument", courseDocRemarksEntity);
        //        }
        //        else
        //            return Content("<div class='alert alert-danger'>File not found.</div>");
        //    }
        //    catch (Exception ex)
        //    {
        //        _IExceptionLogService.SaveRecord(ex, "CourseDocRemarks", "ViewCourseDocumentRemarks", Request.Url.AbsoluteUri, CurrentSession.UserId);
        //        return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
        //    }
        //}
        public ActionResult ViewCourseDocumentRemarks(string dt, string s, string c, string d) //old
        {
            try
            {
                if (s != null && c != null && d != null)
                {
                    var fileName = UtilityHelper.Decrypt(s);
                    var filePath = "~/Content/Temp/" + CurrentSession.UserId.ToString() + "/" + fileName;

                    var courseId = UtilityHelper.Decrypt(c);
                    var docId = UtilityHelper.Decrypt(d);

                    var courseDocRemarksEntity = _ICourseDocRemarksService.GetRecordByDocId(Convert.ToInt32(docId)).ToModel();

                    if (courseDocRemarksEntity == null)
                    {
                        courseDocRemarksEntity = new CourseDocRemarksModel();
                        courseDocRemarksEntity.CourseId = Convert.ToInt32(courseId);
                        courseDocRemarksEntity.DocId = Convert.ToInt32(docId);
                    }

                    courseDocRemarksEntity.DocName = filePath;

                    var courseDocEntity = _ICourseDocumentService.GetRecordById(Convert.ToInt32(docId));
                    ViewBag.DocName = courseDocEntity.DocName;
                    ViewBag.DocType = dt;
                    string ext = System.IO.Path.GetExtension(fileName);
                   
                        ViewBag.pdffullpath = "";
                        ViewBag.fullpath = filePath;


                    return View("ReviewCourseDocument", courseDocRemarksEntity);
                }
                else
                    return Content("<div class='alert alert-danger'>File not found.</div>");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDocRemarks", "ViewCourseDocumentRemarks", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }
    }
}