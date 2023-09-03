using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SSTM.Filters;
using SSTM.Helpers.Common;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using PowerPointTemplates;
using DocumentFormat.OpenXml.Drawing;
using FindAndReplace;
using System.Diagnostics;
using OpenXmlPowerTools;
using System.IO.Packaging;
using System.Web.Script.Serialization;
using SSTM.Business.Interfaces;
using SSTM.Helpers.App;
using SSTM.Models.Centralized_Course;
using SSTM.Helpers.AutoMapping;
using System.Text;
using DocumentFormat.OpenXml.Office.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using System.Drawing;
using SSTM.Core.Centralized_Course;
using SSTM.Core.User;
using SSTM.Core.Central_CourseSharing;
using SSTM.Models.Central_CourseSharing;
using SSTM.Helpers.Model;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class CentralizedCourseController : BaseController
    {

        string imagepath = "~/Content/CenteralizedCourseSamplePPT/image/";
        #region Class Properties Declarations


        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IActivityLogService _IActivityLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        private readonly IRoleService _IRoleService;

        private readonly ICentralizedCourseService _ICentralizedCourseService;
        private readonly ICentralizedDocumentFilesService _ICentralizedDocumentFilesService;
        private readonly ICentralizedHistoryService _ICentralizedHistoryService;
        private readonly ICentralizedMasterService _ICentralizedMasterService;
        private readonly ICourseStatusService _ICourseStatusService;
        private readonly ICentralCourseSharingService _ICentralCourseSharingService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        public CentralizedCourseController(IExceptionLogService exceptionLogService, IActivityLogService activityLogService, IConfigService configService,
            IUserService userService, IRoleService roleService,
            ICentralizedCourseService centralizedCourseService, ICentralizedDocumentFilesService centralizedDocumentFilesService,
            ICentralizedHistoryService centralizedHistoryService, ICentralizedMasterService centralizedMasterService, ICourseStatusService courseStatusService,
            ICentralCourseSharingService CentralCourseSharingService)
        {


            _IExceptionLogService = exceptionLogService;
            _IActivityLogService = activityLogService;
            _IConfigService = configService;
            _IUserService = userService;
            _IRoleService = roleService;

            _ICentralizedCourseService = centralizedCourseService;
            _ICentralizedDocumentFilesService = centralizedDocumentFilesService;
            _ICentralizedHistoryService = centralizedHistoryService;
            _ICentralizedMasterService = centralizedMasterService;
            _ICourseStatusService = courseStatusService;

            _ICentralCourseSharingService = CentralCourseSharingService;
        }

        List<SelectListItem> list = new List<SelectListItem>();
        // GET: CentralizedCourse
        public ActionResult Index()
        {
            foreach (string name in Enum.GetNames(typeof(CenterCourseDocType)))
            {
                list.Add(new SelectListItem { Value = name.ToString(), Text = name });
            }
            TempData["type"] = new SelectList(list, "Value", "Text");
            return View();
        }
        public ActionResult central_list()
        {
            GetDocsStatusList();
            return View(CurrentSession);
        }

        public void GetDocsStatusList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "All" });
            var statusList = _ICourseStatusService.GetList().ToList();
            if (/*CurrentSession.UserRole == "Developer" ||*/ CurrentSession.UserRole == "Staff")
            {
                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Pending").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Pending").FirstOrDefault().Status.ToString()
                });

                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Submitted").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Submitted").FirstOrDefault().Status.ToString()
                });

            }
            else if (CurrentSession.UserRole == "SME")
            {
                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Under Review").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Under Review").FirstOrDefault().Status.ToString()
                });

                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Under Improvement").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Under Improvement").FirstOrDefault().Status.ToString()
                });

                list.Add(new SelectListItem
                {
                    Value = statusList.Where(a => a.Status == "Released").FirstOrDefault().Id.ToString(),
                    Text = statusList.Where(a => a.Status == "Released").FirstOrDefault().Status.ToString()
                });
            }
            else
            {
                statusList.ForEach(a =>
                {
                    list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.Status });
                });
            }

            TempData["CourseStatusList"] = new SelectList(list, "Value", "Text");
        }


        #region Central list


        [HttpGet]
        public ActionResult get_central_list(long statusId)
        {
            try
            {
                var sBuilder = new StringBuilder();
                List<Centralized_List_MasterModel> list = new List<Centralized_List_MasterModel>();
                if (CurrentSession.UserRole == "SME")
                {
                    list = _ICentralizedMasterService.GetAllSMERecord(CurrentSession.UserId).ToList();
                }
                else
                {
                    if (CurrentSession.UserRole == "Trainer")
                        list = _ICentralizedMasterService.GetAllTrainerStatusRecord(0).ToList();
                    else
                        list = _ICentralizedMasterService.GetAllStatusRecord(0).ToList();
                }

                foreach (var item in list)
                {
                    var SharedCourseList = _ICentralCourseSharingService.GetAssignRecord((int)item.id);
                    var courseNameTrack = item.CourseStatus;

                    if (courseNameTrack == "Pending")
                        courseNameTrack = "Draft (By Developer)";
                    if (courseNameTrack == "Submitted")
                        courseNameTrack = "Submitted (By Developer)";
                    if (courseNameTrack == "Under Review")
                        courseNameTrack = "SME Comments";
                    if (courseNameTrack == "Under Improvement")
                        courseNameTrack = "Improvement (By Developer)";

                    string sharedcourse = "";
                    if (SharedCourseList.Count() != 0)
                        courseNameTrack = "Shared";


                    if (courseNameTrack == "Released" || courseNameTrack == "Shared")
                        sharedcourse = "<button type='button' title='Share course documents' class='btn btn-success btn-sm btnShareCourse'>" +
                            "<i class='fas fa-share-alt'></i></button>";

                    var actions = "<button type='button' title='Edit' class='btn btn-primary btn-sm btnEditCourse'><i class='fa fa-pen'></i></button>&nbsp;";
                    var PlaceholderReplacement = "<button type='button' title='Placeholder' class='btn btn-primary btn-sm btnPlaceholder'><i class='fa fa-gear'></i></button>&nbsp;";

                    sBuilder.Append(
                       "<tr id='" + item.id + "' data-status='" + item.CourseStatus + "'>" +
                           "<td>" + item.CentralDocumentName + "</td>" +
                           "<td>" + item.document_type + "</td>" +
                            "<td>" + item.Developer + "</td>" +
                            "<td>" + item.SME + "</td>" +
                           "<td class='text-center'>" +
                                "<button type='button' title='Documents' class='btn btn-info btn-sm btnCourseDocs'>" +
                                    "<i class='fa fa-files-o'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Remarks' class='btn btn-primary btn-sm btnCourseDocsRemarks'>" +
                                    "<i class='fa fa-comments'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Assign' class='btn btn-warning btn-sm btnAssignCourse'>" +
                                    "<i class='fa fa-user-cog'></i></button>" +
                            "</td>" +
                             "<td class='text-center'>" +
                                "<button type='button' title='Assign' class='btn btn-warning btn-sm btnSMEDocComment'>" +
                                    "<i class='fa fa-comments'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Approve course documents' class='btn btn-primary btn-sm btnApproveCourse'>" +
                                    "<i class='fas fa-clipboard-check'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                "<button type='button' title='Release' class='btn btn-success btn-sm btnReleaseCourse'>" +
                                    "<i class='fa fa-broadcast-tower'></i></button>" +
                            "</td>" +
                            "<td class='text-center'>" +
                                    sharedcourse +
                            "</td>" +
                              "<td class='text-center'>" +
                                "<button type='button' title='Traking' class='btn btn-warning btn-sm btnTraking'>" + courseNameTrack + "</button>" +
                            "</td>" +
                            "<td class='text-center'>" + actions + "</td>" +
                             "<td class='text-center'>" + PlaceholderReplacement + "</td>" +
                        "</tr>");
                }
                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "GetCoursesList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region History


        public ActionResult Centralize_history()
        {
            return View();
        }

        public ActionResult Get_Centralize_history()
        {
            var sBuilder = new StringBuilder();
            List<Centralized_HistoryWithDataModel> data = _ICentralizedHistoryService.GetHistory_data();
            int counter = 1;
            foreach (var item in data)
            {

                sBuilder.Append(
                      "<tr id='" + item.id + "'>" +
                        "<td>" + counter + "</td>" +
                        "<td>" + item.type + "</td>" +
                        "<td>" + item.master_text + "</td>" +
                        "<td>" + item.replace_text + "</td>" +
                        "<td></td>" +
                        "<td >" + item.UpdatedOn + "</td>" +
                        "<td >" + item.UserName + "</td></tr>");
                counter++;
            }
            return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region save master central course

        public ActionResult Master_centeral_doc_save(string data)
        {
            Centralized_MasterModel data_final = new JavaScriptSerializer().Deserialize<Centralized_MasterModel>(data);
            string Rawpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/RAW_DOC/" + data_final.choose_type.Trim());
            string RawPPTpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/RAW_DOC/" + data_final.choose_type.Trim() + "/CentralizePPT.pptx");


            if(!Directory.Exists(Rawpath))
                Directory.CreateDirectory(Rawpath);

            DirectoryInfo di = new DirectoryInfo(Rawpath);
            try
            {
                FileInfo[] docFile = di.GetFiles("*.doc");
                FileInfo[] docxFile = di.GetFiles("*.docx");
                FileInfo[] pptFile = di.GetFiles("*.ppt");
                FileInfo[] pptxFile = di.GetFiles("*.pptx");
                if (docFile.Length == 0 && docxFile.Length == 0 )
                    return Json(new { result = false, masterid = 0, message = "Raw Document folder " + data_final.choose_type+ " inside Doc file not exist" }, JsonRequestBehavior.AllowGet);
                if(pptFile.Length == 0 && pptxFile.Length == 0)
                    return Json(new { result = false, masterid = 0, message = "Raw Document folder " + data_final.choose_type + " inside PPT file not exist" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { result = false, masterid = 0, message = "Folder Not Exist In Path " + Rawpath + " So,Please First add data after Create course" }, JsonRequestBehavior.AllowGet);
            }

            var entity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(data_final.id));
            if (entity != null)
            {
                entity.UpdatedBy = Convert.ToInt32(CurrentSession.UserId);
                entity.UpdatedOn = DateTime.Now;
                entity.sme_assign_id = entity.sme_assign_id;
                entity.Statusid = entity.Statusid;

            }
            else
            {
                entity = new Centralized_MasterModel().ToEntity();
                entity.CreatedBy = Convert.ToInt32(CurrentSession.UserId);
                entity.CreatedOn = DateTime.Now;
                entity.sme_assign_id = 0;
                entity.Statusid = _ICourseStatusService.GetRecordIdByName("Pending");

                #region Make File Folder
                entity.FolderNameInput = data_final.CentralDocumentName.Trim().Replace(" ","_") + "_" + data_final.language.Trim().Replace(" ", "_") + "_" + data_final.choose_type.Trim().Replace(" ", "_");
                entity.FolderNameOutput = data_final.CentralDocumentName.Trim().Replace(" ", "_") + "_" + data_final.language.Trim().Replace(" ", "_") + "_" + data_final.choose_type.Trim().Replace(" ", "_");

                string inputpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + entity.FolderNameInput);
                string outputpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseOutputFiles/" + entity.FolderNameInput);
                //string Rawpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/RAW_DOC/");
                //string RawPPTpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/RAW_DOC/CentralizePPT.pptx");

                try
                {
                    if (!Directory.Exists(inputpath))
                        Directory.CreateDirectory(inputpath);
                    if (!Directory.Exists(outputpath))
                        Directory.CreateDirectory(outputpath);

                }
                catch (Exception)
                {
                }
                #endregion

                #region file raw folder to course Input Folder file copy
                RawFileCopytoCourseInputFolderCopy(Rawpath, inputpath, "CentralizePPT.pptx");

                string[] values = data_final.document_type.Split(',');

                foreach (string filename in values)
                {
                    RawFileCopytoCourseInputFolderCopy(Rawpath, inputpath, filename + "_TEMPLATE.docx");
                }
                #endregion
            }
            entity.language = data_final.language.Trim();
            entity.CentralDocumentName = data_final.CentralDocumentName.Trim();
            entity.document_type = data_final.document_type.Trim();
            entity.isDeleted = false;
            entity.choose_type = data_final.choose_type.Trim();
            entity.AirLineCourseId = data_final.AirLineCourseId;

            if (CurrentSession.UserRole == "Director")
            {
                entity.DirectorId = Convert.ToInt32(CurrentSession.UserId);
                entity.developer_id = 0;
            }
            else
            {
                entity.DirectorId = 0;
                entity.developer_id = Convert.ToInt32(CurrentSession.UserId);
            }

            var MainId = _ICentralizedMasterService.SaveRecord(entity);

            return Json(new { result = true, masterid = MainId }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public void RawFileCopytoCourseInputFolderCopy(string rawPath, string CourseInput, string filename)
        {
            string fileName = filename;
            string destFile = filename;
            if (System.IO.Directory.Exists(rawPath))
            {
                string[] files = System.IO.Directory.GetFiles(rawPath);

                // Copy the files and overwrite destination files if they already exist. 
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    if (filename == System.IO.Path.GetFileName(s))
                    {
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(CourseInput, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
        }

        [HttpPost]
        public ActionResult CentralNewCourse(int id)
        {
            try
            {
                try
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem { Value = "0", Text = "--- Select  ----" });
                    CourseService.SSTM service = new SSTM.CourseService.SSTM();
                    string Coursedata = service.AllCourse();
                    List<SSTM.Models.Course.AirlineCourseModel> AirlineCourseModel = (new JavaScriptSerializer()).Deserialize<List<SSTM.Models.Course.AirlineCourseModel>>(Coursedata);

                    AirlineCourseModel.ForEach(a =>
                    {
                        list.Add(new SelectListItem { Value = Convert.ToString(a.CourseId), Text = a.CourseName });
                    });
                    TempData["AirlineCourse"] = new SelectList(list, "Value", "Text");

                }
                catch (Exception)
                {
                    var list = new List<SelectListItem>();
                    TempData["AirlineCourse"] = new SelectList(list, "Value", "Text");
                }

                Centralized_Master data = _ICentralizedMasterService.GetRecordById(id);
                if (data == null)
                {
                    data = new Centralized_Master();
                    data.id = 0;
                }
                foreach (string name in Enum.GetNames(typeof(CenterCourseDocType)))
                {
                    list.Add(new SelectListItem { Value = name.ToString(), Text = name });
                }
                TempData["type"] = new SelectList(list, "Value", "Text");

                return PartialView("_addCourse", data);
            }

            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Centrail Course", "CentralNewCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }


        [HttpPost]
        public ActionResult PlaceholderReplacementDataForm(int id)
        {
            try
            {
                Centralized_Master data = new Centralized_Master();
                data.id = id;
                return PartialView("_PlaceholderReplacement", data);
            }

            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "PlaceholderReplacementDataForm", "CentralNewCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }


        #region Document Replace text save

        public ActionResult Centralized_Course_save(string data, int master_id)
        {
            List<Central_Manadatory> data_final = new JavaScriptSerializer().Deserialize<List<Central_Manadatory>>(data);
            var getcentral_replaceData = _ICentralizedCourseService.GetAllbyCenterRecord(master_id);
            if (data_final.Count == getcentral_replaceData.Count)
            {
                return Json(new { result = true, masterid = getcentral_replaceData.Count != 0 ? getcentral_replaceData[0].center_master_id : 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    _ICentralizedCourseService.DeleteAllRecord(master_id);
                }
                catch (Exception)
                {
                }
                //try
                //{
                //    _ICentralizedHistoryService.DeleteAllRecord(master_id);
                //}
                //catch (Exception)
                //{
                //}
            }
            foreach (var item in data_final)
            {
                var entity = _ICentralizedCourseService.GetRecordById(Convert.ToInt32(item.id));
                if (entity != null)
                {
                    entity.UpdatedBy = Convert.ToInt32(CurrentSession.UserId);
                    entity.UpdatedOn = DateTime.Now;
                }
                else
                {
                    entity = new Centralized_CourseModel().ToEntity();
                    entity.CreatedBy = Convert.ToInt32(CurrentSession.UserId);
                    entity.CreatedOn = DateTime.Now;
                }
                entity.center_master_id = master_id;
                entity.master_text = item.lablename.Trim();
                entity.replace_text = item.name.Trim();
                entity.type = item.type.Trim();
                entity.textimage = item.repeate.Trim();
                entity.isDeleted = false;

                var MainId = _ICentralizedCourseService.SaveRecord(entity);


                //save history table data 
                //List<Centralized_History> GetAllbyCenterRecord = _ICentralizedHistoryService.GetAllbyCenterRecord(MainId);
                //Centralized_History GetfirstbyCenterRecord = GetAllbyCenterRecord.OrderByDescending(a => a.id).FirstOrDefault();

                #region Save history Data
                var entity_history = new Centralized_HistoryModel().ToEntity();
                entity_history.center_master_id = master_id;
                entity_history.master_text = item.lablename.Trim();
                entity_history.replace_text = item.name.Trim();
                entity_history.type = item.repeate.Trim();
                entity_history.textimage = item.type.Trim();
                entity_history.isDeleted = false;
                entity_history.CreatedBy = Convert.ToInt32(CurrentSession.UserId);
                entity_history.CreatedOn = DateTime.Now;
                entity_history.UpdatedBy = Convert.ToInt32(CurrentSession.UserId);
                entity_history.UpdatedOn = DateTime.Now;
                entity_history.Version = 1;
                entity_history.VersionDate = DateTime.Now;
                _ICentralizedHistoryService.SaveRecord(entity_history);
                #endregion
                //end history table data save
            }
            return Json(new { result = true, masterid = master_id }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Generate ppt

        public ActionResult CentralizePPTGenerate(string data, int master_id)
        {
            List<textreplacedata> data_final = new JavaScriptSerializer().Deserialize<List<textreplacedata>>(data);
            List<textreplacedata> data_final_list = data_final.Where(a => a.type == "PPT" || a.type == "All").ToList();

            var Masterentity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(master_id));

            string filename = "ppt_" + master_id + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pptx";
            var templatePath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" +  Masterentity.FolderNameInput.Trim() + "/CentralizePPT.pptx");

            string opath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseOutputFiles/" + Masterentity.FolderNameInput.Trim() + "/");
            string inputpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + Masterentity.FolderNameInput.Trim());

            try
            {
                if (!Directory.Exists(opath))
                    Directory.CreateDirectory(opath);

            }
            catch (Exception)
            {
                return Json(new { result = false, message = "Error ppt " });
            }

            List<Centralized_Document_files> files = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(master_id);


            var entity = _ICentralizedDocumentFilesService.GetRecordNewcourseByTypewithmasterid(master_id, "PPT");
            // string existDeletefile = "";
            //foreach (var item in files)
            //{
            //    if (item.Document_Type_Name.Trim() == "PPT")
            //    {
            //        templatePath = opath + item.Document_File_Name;
            //        existDeletefile = templatePath;
            //        filename = item.Document_File_Name;
            //    }
            //}

            //#region Check data same then new file not generate

            //var centralReplacedata = _ICentralizedCourseService.GetCourseReplacedataWithType("PPT", master_id);
            //if (data_final_list.Count == centralReplacedata.Count)
            //    return Json(new { result = true, message = "Already Generated PPT Click on Save Button" });

            //#endregion
            var outputPath = opath + filename;


            Syncfusion.Presentation.IPresentation pptxDoc = Syncfusion.Presentation.Presentation.Open(templatePath);

            List<Syncfusion.Presentation.ISlide> slides = pptxDoc.Slides.ToList();
            int s = 0;
            foreach (Syncfusion.Presentation.ISlide slide in slides)
            {
                Syncfusion.Presentation.ISlide i_slide = slides[s];
                foreach (var item in data_final_list)
                {
                    if (item.repeate != "image")
                    {
                        Syncfusion.Presentation.ITextSelection textSelections = null;
                        if (item.name.Contains("http"))
                        {
                            //below set hyper link
                            textSelections = i_slide.Find(item.lablename, false, false);
                            if (textSelections != null)
                            {

                                Syncfusion.Presentation.ITextPart textPart = textSelections.GetAsOneTextPart();
                                textPart.SetHyperlink(item.name);
                            }
                        }
                        else
                        {
                            //below set text value
                            textSelections = i_slide.Find(item.lablename, false, false);
                            if (textSelections != null)
                            {
                                Syncfusion.Presentation.ITextPart textPart = textSelections.GetAsOneTextPart();
                                textPart.Text = item.name;
                            }
                        }
                    }
                    else
                    {
                        foreach (Syncfusion.Presentation.IPicture pic in i_slide.Pictures)
                        {
                            if (pic.Title == item.lablename)
                            {
                                string path = Server.MapPath(imagepath) + item.name; //Server.MapPath($"~/Content/CenteralizedCourseSamplePPT/temp/{item.name}");
                                byte[] imageReplace = System.IO.File.ReadAllBytes(path);
                                pic.ImageData = imageReplace;
                            }
                        }
                    }
                }
                s++;
            }
            pptxDoc.Save(outputPath);


            if (entity == null)
            {
                Centralized_Document_filesModel model = new Centralized_Document_filesModel();
                model.Central_Master_Id = master_id;
                model.Document_File_Name = filename.Trim();
                model.Document_Type_Name = "PPT";
                saveDocFiles(model);
            }
            else
            {
                entity.Central_Master_Id = master_id;
                entity.Document_File_Name = filename.Trim();
                entity.Document_Type_Name = "PPT";
                _ICentralizedDocumentFilesService.SaveRecord(entity);
            }

            try
            {
                Stream reader = new FileStream(outputPath, FileMode.Open);
                var configEntity = _IConfigService.GetFirstRecord();
                SSTM.Models.AWS.AWSModel awsModel = new SSTM.Models.AWS.AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = "CentralCourse/CourseOutputFiles/" + Masterentity.FolderNameInput,
                    FileName = filename,
                    LocalFileStream = reader,
                };

                AWSHelper.UploadFile(awsModel);
                reader.Close();
            }
            catch (Exception ex)
            {
            }
            try
            {
                Generate_documentfile(data, master_id, "");
            }
            catch (Exception ex)
            {
            }
            try
            {
                if (System.IO.File.Exists(outputPath))
                    System.IO.File.Delete(outputPath);
            }
            catch (Exception ex)
            {
                try
                {
                    _IExceptionLogService.SaveRecord(ex, "Central Document", "Save document file", Request.Url.AbsoluteUri, CurrentSession.UserId);
                }
                catch (Exception)
                {
                }
            }

            return Json(new { result = true, message = "Successfully Save Data and Generated PPT" });
        }

        #endregion

        public ActionResult FileUpload_folder(string filename1)
        {
            if (!Directory.Exists(Server.MapPath(imagepath)))
                Directory.CreateDirectory(Server.MapPath(imagepath));

            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            string fileName = file.FileName;
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            //fileName =  DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + System.IO.Path.GetExtension(fileName);
            fileName = filename1 + System.IO.Path.GetExtension(fileName);
            string path = Server.MapPath(imagepath);
            file.SaveAs(path + fileName);
            return Json(new { result = true, FileName = fileName });
        }


        #region Generate Document files

        public void Generate_documentfile(string data, int master_id, string type)
        {
            var Masterentity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(master_id));
            string[] values = Masterentity.document_type.Split(',');

            foreach (string gettype in values)
            {
                type = gettype;
                //  CenterCourseDocType status = (CenterCourseDocType)int.Parse(type);

                string filename = type.ToString() + "_" + master_id + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".docx";

                #region file path

                string path = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseOutputFiles/" + Masterentity.FolderNameInput.Trim() + "/");

                string inputFile = "";
                string inputpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + Masterentity.FolderNameInput.Trim() + "/");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!Directory.Exists(inputpath))
                    Directory.CreateDirectory(inputpath);
                #endregion

                List<Centralized_Document_files> files = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(master_id);

                #region One Time Pickup File

                if (inputFile == "")
                {
                    if (type.ToString() == "LG")
                        inputFile = inputpath + "LG_TEMPLATE.docx";
                    if (type.ToString() == "FG")
                        inputFile = inputpath + "FG_TEMPLATE.docx";
                    if (type.ToString() == "CRM")
                        inputFile = inputpath + "CRM_TEMPLATE.docx";
                    if (type.ToString() == "LP")
                        inputFile = inputpath + "LP_TEMPLATE.docx";
                    if (type.ToString() == "QA")
                        inputFile = inputpath + "QA_TEMPLATE.docx";
                    if (type.ToString() == "Qwithoutanswer")
                        inputFile = inputpath + "Qwithoutanswer_TEMPLATE.docx";
                    if (type.ToString() == "AP")
                        inputFile = inputpath + "AP_TEMPLATE.docx";
                    if (type.ToString() == "AR")
                        inputFile = inputpath + "AR_TEMPLATE.docx";
                    if (type.ToString() == "Assessmentchecklist")
                        inputFile = inputpath + "Assessmentchecklist_TEMPLATE.docx";
                    if (type.ToString() == "RA")
                        inputFile = inputpath + "RA_TEMPLATE.docx";
                    if (type.ToString() == "SWP")
                        inputFile = inputpath + "SWP_TEMPLATE.docx";
                }
                #endregion

                var outputFile = path + filename;

                #region If type and master id with data exist then remove

                var entity = _ICentralizedDocumentFilesService.GetRecordNewcourseByTypewithmasterid(master_id, type.ToString().Trim());

                List<textreplacedata> data_final = new JavaScriptSerializer().Deserialize<List<textreplacedata>>(data);
                List<textreplacedata> data_final_list = data_final.Where(a => a.type == type.ToString().Trim() || a.type == "All").ToList();

                #endregion
                if (data_final_list.Count() != 0)
                {
                    //FileStream fileStream = new FileStream(inputFile, FileMode.Open, FileAccess.ReadWrite);
                    Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(inputFile);
                    Syncfusion.DocIO.DLS.TextBodyPart textBodyPart = new Syncfusion.DocIO.DLS.TextBodyPart(document);
                    foreach (var dbItem in data_final_list)
                    {
                        if (dbItem.repeate != "image")
                        {
                            Syncfusion.DocIO.DLS.TextSelection textSelection = null;
                            if (dbItem.name.Contains("http"))
                            {
                                textSelection = document.Find(dbItem.lablename, false, true);
                                if (textSelection != null)
                                {
                                    Syncfusion.DocIO.DLS.WTextRange textRange = textSelection.GetAsOneRange();
                                    textRange.Text = dbItem.name;
                                }

                            }
                            else
                            {
                                textSelection = document.Find(dbItem.lablename, false, true);
                                if (textSelection != null)
                                {
                                    Syncfusion.DocIO.DLS.WTextRange textRange = textSelection.GetAsOneRange();
                                    textRange.Text = dbItem.name;
                                }
                            }
                        }
                        else
                        {
                            Syncfusion.DocIO.DLS.WPicture picture = document.FindItemByProperty(Syncfusion.DocIO.DLS.EntityType.Picture, "AlternativeText", dbItem.lablename) as Syncfusion.DocIO.DLS.WPicture;
                            if (picture != null)
                            {
                                float width = picture.Width;
                                float height = picture.Height;

                                string replaceImage = Server.MapPath(imagepath) + dbItem.name; // Server.MapPath($"~/Content/CenteralizedCourseSamplePPT/temp/{dbItem.name}");
                                picture.LoadImage(System.IO.File.ReadAllBytes(replaceImage));

                                picture.Width = width;
                                picture.Height = height;
                            }
                        }
                    }

                    MemoryStream stream = new MemoryStream();
                    document.Save(outputFile, Syncfusion.DocIO.FormatType.Docx);
                    document.Close();

                    #region Save Document file

                    if (entity == null)
                    {
                        Centralized_Document_filesModel model = new Centralized_Document_filesModel();
                        model.Central_Master_Id = master_id;
                        model.Document_File_Name = filename.Trim();
                        model.Document_Type_Name = type.ToString().Trim();
                        saveDocFiles(model);
                    }
                    else
                    {
                        entity.Central_Master_Id = master_id;
                        entity.Document_File_Name = filename.Trim();
                        entity.Document_Type_Name = type.ToString().Trim();
                        _ICentralizedDocumentFilesService.SaveRecord(entity);
                    }
                }
                #endregion
                //var outputFile = path + filename;
                Stream reader = new FileStream(outputFile, FileMode.Open);
                var configEntity = _IConfigService.GetFirstRecord();
                SSTM.Models.AWS.AWSModel awsModel = new SSTM.Models.AWS.AWSModel()
                {
                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                    BucketDirectory = "CentralCourse/CourseOutputFiles/" + Masterentity.FolderNameInput.Trim(),
                    FileName = filename,
                    LocalFileStream = reader,
                };
                AWSHelper.UploadFile(awsModel);
                reader.Close();

                try
                {
                    if (System.IO.File.Exists(outputFile))
                        System.IO.File.Delete(outputFile);
                }
                catch (Exception ex)
                {
                    try
                    {
                        _IExceptionLogService.SaveRecord(ex, "Central Document", "Save document file", Request.Url.AbsoluteUri, CurrentSession.UserId);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

        }

        public ActionResult General_document_Generate(string data, int master_id, string type)
        {
            var Masterentity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(master_id));
            string[] values = Masterentity.document_type.Split(',');

            foreach (string gettype in values)
            {
                type = gettype;
                CenterCourseDocType status = (CenterCourseDocType)int.Parse(type);

                string filename = status.ToString() + "_" + master_id + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".docx";

                #region file path

                string path = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseOutputFiles/" + Masterentity.FolderNameInput.Trim());

                string inputFile = "";
                string inputpath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + Masterentity.FolderNameInput.Trim());

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!Directory.Exists(inputpath))
                    Directory.CreateDirectory(inputpath);
                #endregion

                List<Centralized_Document_files> files = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(master_id);

                #region replace file comment code


                //foreach (var item in files)
                //{
                #region  LG

                //if (item.Document_Type_Name.Trim() == "LG")
                //{
                //    if (status.ToString().Trim() == "LG")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "LG")
                //        if (status.ToString().Trim() == "LG")
                //            inputFile = inputpath + "LG_TEMPLATE.docx";
                //}
                #endregion

                #region FG

                //if (item.Document_Type_Name.Trim() == "FG")
                //{
                //    if (status.ToString().Trim() == "FG")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "FG")
                //        if (status.ToString().Trim() == "FG")
                //            inputFile = inputpath + "FG_TEMPLATE.docx";
                //}
                #endregion

                #region CRM

                //if (item.Document_Type_Name.Trim() == "CRM")
                //{
                //    if (status.ToString().Trim() == "CRM")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "CRM")
                //        if (status.ToString().Trim() == "CRM")
                //            inputFile = inputpath + "CRM_TEMPLATE.docx";
                //}
                #endregion

                #region LP

                //if (item.Document_Type_Name.Trim() == "LP")
                //{
                //    if (status.ToString().Trim() == "LP")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "LP")
                //        if (status.ToString().Trim() == "LP")
                //            inputFile = inputpath + "LP_TEMPLATE.docx";
                //}
                #endregion

                #region QA

                //if (item.Document_Type_Name.Trim() == "QA")
                //{
                //    if (status.ToString().Trim() == "QA")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "QA")
                //        if (status.ToString().Trim() == "QA")
                //            inputFile = inputpath + "QA_TEMPLATE.docx";
                //}
                #endregion

                #region Qwithoutanswer

                //if (item.Document_Type_Name.Trim() == "Qwithoutanswer")
                //{
                //    if (status.ToString().Trim() == "Qwithoutanswer")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "Qwithoutanswer")
                //        if (status.ToString().Trim() == "Qwithoutanswer")
                //            inputFile = inputpath + "Qwithoutanswer_TEMPLATE.docx";
                //}
                #endregion

                #region AP

                //if (item.Document_Type_Name.Trim() == "AP")
                //{
                //    if (status.ToString().Trim() == "AP")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "AP")
                //        if (status.ToString().Trim() == "AP")
                //            inputFile = inputpath + "AP_TEMPLATE.docx";
                //}
                #endregion

                #region AR

                //if (item.Document_Type_Name.Trim() == "AR")
                //{
                //    if (status.ToString().Trim() == "AR")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "AR")
                //        if (status.ToString().Trim() == "AR")
                //            inputFile = inputpath + "AR_TEMPLATE.docx";
                //}
                #endregion

                #region Assessmentchecklist

                //if (item.Document_Type_Name.Trim() == "Assessmentchecklist")
                //{
                //    if (status.ToString().Trim() == "Assessmentchecklist")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "Assessmentchecklist")
                //        if (status.ToString().Trim() == "Assessmentchecklist")
                //            inputFile = inputpath + "Assessmentchecklist_TEMPLATE.docx";
                //}
                #endregion

                #region RA

                //if (item.Document_Type_Name.Trim() == "RA")
                //{
                //    if (status.ToString().Trim() == "RA")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "RA")
                //        if (status.ToString().Trim() == "RA")
                //            inputFile = inputpath + "RA_TEMPLATE.docx";
                //}
                #endregion

                #region SWP

                //if (item.Document_Type_Name.Trim() == "SWP")
                //{
                //    if (status.ToString().Trim() == "SWP")
                //    {
                //        inputFile = path + item.Document_File_Name;
                //        filename = item.Document_File_Name;
                //        break;
                //    }
                //}
                //else
                //{
                //    if (item.Document_Type_Name.Trim() == "SWP")
                //        if (status.ToString().Trim() == "SWP")
                //            inputFile = inputpath + "SWP_TEMPLATE.docx";
                //}
                #endregion
                // }
                #endregion

                #region One Time Pickup File

                if (inputFile == "")
                {
                    if (status.ToString() == "LG")
                        inputFile = inputpath + "LG_TEMPLATE.docx";
                    if (status.ToString() == "FG")
                        inputFile = inputpath + "FG_TEMPLATE.docx";
                    if (status.ToString() == "CRM")
                        inputFile = inputpath + "CRM_TEMPLATE.docx";
                    if (status.ToString() == "LP")
                        inputFile = inputpath + "LP_TEMPLATE.docx";
                    if (status.ToString() == "QA")
                        inputFile = inputpath + "QA_TEMPLATE.docx";
                    if (status.ToString() == "Qwithoutanswer")
                        inputFile = inputpath + "Qwithoutanswer_TEMPLATE.docx";
                    if (status.ToString() == "AP")
                        inputFile = inputpath + "AP_TEMPLATE.docx";
                    if (status.ToString() == "AR")
                        inputFile = inputpath + "AR_TEMPLATE.docx";
                    if (status.ToString() == "Assessmentchecklist")
                        inputFile = inputpath + "Assessmentchecklist_TEMPLATE.docx";
                    if (status.ToString() == "RA")
                        inputFile = inputpath + "RA_TEMPLATE.docx";
                    if (status.ToString() == "SWP")
                        inputFile = inputpath + "SWP_TEMPLATE.docx";
                }
                #endregion

                if (!System.IO.File.Exists(inputFile))
                    return Json(new { result = false, message = "Sample File not Exist Please check  !" });

                var outputFile = path + filename;

                #region If type and master id with data exist then remove

                var entity = _ICentralizedDocumentFilesService.GetRecordNewcourseByTypewithmasterid(master_id, status.ToString().Trim());

                List<textreplacedata> data_final = new JavaScriptSerializer().Deserialize<List<textreplacedata>>(data);
                List<textreplacedata> data_final_list = data_final.Where(a => a.type == status.ToString().Trim() || a.type == "All").ToList();

                //#region Check data same then new file not generate

                //var centralReplacedata = _ICentralizedCourseService.GetCourseReplacedataWithType(status.ToString().Trim(), master_id);
                //if (data_final_list.Count == centralReplacedata.Count)
                //    return Json(new { result = true, message = "Already Generated " + status.ToString().Trim() });

                //#endregion

                if (data_final_list.Count() == 0)
                    return Json(new { result = false, message = "Replace Data Not Exist !" });

                #endregion
                //FileStream fileStream = new FileStream(inputFile, FileMode.Open, FileAccess.ReadWrite);
                Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(inputFile);
                Syncfusion.DocIO.DLS.TextBodyPart textBodyPart = new Syncfusion.DocIO.DLS.TextBodyPart(document);
                foreach (var dbItem in data_final_list)
                {
                    if (dbItem.repeate != "image")
                    {
                        Syncfusion.DocIO.DLS.TextSelection textSelection = null;
                        if (dbItem.name.Contains("http"))
                        {
                            //textSelection = document.Find(dbItem.lablename, false, true);
                            //if (textSelection != null)
                            //{
                            //    Syncfusion.DocIO.DLS.IWSection section = document.AddSection();

                            //    Syncfusion.DocIO.DLS.WTextRange textRange = textSelection.GetAsOneRange();
                            //    Syncfusion.DocIO.DLS.IWParagraph paragraph = section.AddParagraph();
                            //    textBodyPart.BodyItems.Add(paragraph);
                            //    paragraph.AppendHyperlink(dbItem.name, dbItem.lablename, Syncfusion.DocIO.DLS.HyperlinkType.WebLink);
                            //    document.Replace(dbItem.lablename, textBodyPart, false, true);

                            //}
                            textSelection = document.Find(dbItem.lablename, false, true);
                            if (textSelection != null)
                            {
                                Syncfusion.DocIO.DLS.WTextRange textRange = textSelection.GetAsOneRange();
                                textRange.Text = dbItem.name;
                            }

                        }
                        else
                        {
                            textSelection = document.Find(dbItem.lablename, false, true);
                            if (textSelection != null)
                            {
                                Syncfusion.DocIO.DLS.WTextRange textRange = textSelection.GetAsOneRange();
                                textRange.Text = dbItem.name;
                            }
                        }
                    }
                    else
                    {
                        Syncfusion.DocIO.DLS.WPicture picture = document.FindItemByProperty(Syncfusion.DocIO.DLS.EntityType.Picture, "AlternativeText", dbItem.lablename) as Syncfusion.DocIO.DLS.WPicture;
                        if (picture != null)
                        {
                            float width = picture.Width;
                            float height = picture.Height;

                            string replaceImage = Server.MapPath(imagepath) + dbItem.name; // Server.MapPath($"~/Content/CenteralizedCourseSamplePPT/temp/{dbItem.name}");
                            picture.LoadImage(System.IO.File.ReadAllBytes(replaceImage));

                            picture.Width = width;
                            picture.Height = height;
                        }
                    }
                }

                MemoryStream stream = new MemoryStream();
                document.Save(outputFile, Syncfusion.DocIO.FormatType.Docx);
                document.Close();


                #region Save Document file

                if (entity == null)
                {
                    Centralized_Document_filesModel model = new Centralized_Document_filesModel();
                    model.Central_Master_Id = master_id;
                    model.Document_File_Name = filename.Trim();
                    model.Document_Type_Name = status.ToString().Trim();
                    saveDocFiles(model);
                }
                else
                {
                    entity.Central_Master_Id = master_id;
                    entity.Document_File_Name = filename.Trim();
                    entity.Document_Type_Name = status.ToString().Trim();
                    _ICentralizedDocumentFilesService.SaveRecord(entity);
                }

                #endregion
            }
            return Json(new { result = true, message = " Successfully Save Data and Generated Document File" });
        }

        #endregion


        #region Save Document File

        public void saveDocFiles(Centralized_Document_filesModel model)
        {
            var entity = new Centralized_Document_filesModel().ToEntity();
            entity.Central_Master_Id = model.Central_Master_Id;
            entity.Document_File_Name = model.Document_File_Name.Trim();
            entity.Document_Type_Name = model.Document_Type_Name.Trim();
            entity.CreatedOn = DateTime.Now;
            _ICentralizedDocumentFilesService.SaveRecord(entity);
        }
        #endregion

        public ActionResult Get_Centrail_data(int document_id)
        {
            var master = _ICentralizedMasterService.GetRecordById(document_id);
            List<Centralized_Document_files> centraldocfiles = new List<Centralized_Document_files>();
            List<Centralized_Document_files> filtercentraldocfiles = new List<Centralized_Document_files>();
            if (CurrentSession.UserRole == "Trainer" || CurrentSession.UserRole == "Print Incharge")
            {
                centraldocfiles = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(document_id);
                foreach (var item in centraldocfiles)
                {
                    if (CurrentSession.UserRole == "Trainer")
                    {
                        var data = _ICentralCourseSharingService.GetRecordByCentraAndDocIds(master.id, item.id);
                        if (data != null)
                        {
                            if (data.isTraining == true)
                            {
                                filtercentraldocfiles.Add(item);
                            }
                        }
                    }
                    if (CurrentSession.UserRole == "Print Incharge")
                    {
                        var data = _ICentralCourseSharingService.GetRecordByCentraAndDocIds(master.id, item.id);
                        if (data != null)
                        {
                            if (data.isPrinting == true)
                            {
                                filtercentraldocfiles.Add(item);
                            }
                        }
                    }
                }
                centraldocfiles = filtercentraldocfiles;
            }
            else
            {
                centraldocfiles = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(document_id);
            }

            var replace_text = _ICentralizedCourseService.GetAllbyCenterRecord(document_id);

            return Json(new { Master = master, Document = centraldocfiles, replace_data = replace_text, result = true, message = " Successfully Get" });
        }

        #region SME

        
        [HttpPost]
        public ActionResult OpenSMEList()
        {
            try
            {
                GetSMEList();

                return PartialView("_SMEListModal");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Central Document", "OpenSMEList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }
        public void GetSMEList()
        {
            var list = _IUserService.GetList(1).Where(a => a.Role.Trim() == "SME").ToList();

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = "0", Text = "Select" });

            list.ForEach(a => { selectList.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.FirstName + " " + a.LastName }); });

            TempData["SMEList"] = new SelectList(selectList, "Value", "Text");
        }

        public JsonResult AssignSME(long courseId)
        {
            var entity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(courseId));
            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AssignCourseToSME(long courseId, long SMEId)
        {
            try
            {
                var entity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(courseId));
                var smeEntity = _IUserService.GetRecordById(SMEId);

                if (entity != null && smeEntity != null)
                {
                    long? statusId = _ICourseStatusService.GetRecordIdByName("Under Review");

                    if (statusId > 0)
                    {
                        #region Save record into course assignment table 

                        entity.sme_assign_id = smeEntity.Id;
                        entity.Statusid = _ICourseStatusService.GetRecordIdByName("Under Review");
                        var MainId = _ICentralizedMasterService.SaveRecord(entity);

                        #endregion

                        #region Send notification to selected SME
                        var configEntity = _IConfigService.GetFirstRecord();
                        if (configEntity != null)
                        {
                            var developerEntity = _IUserService.GetRecordById(Convert.ToInt64(entity.CreatedBy));

                            var developerName = string.Empty;
                            if (developerEntity != null)
                                developerName = developerEntity.FirstName + " " + developerEntity.LastName;

                            var emailBody = UtilityHelper.GetEmailTemplate("NotificationToSMEAndDeveloper.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", smeEntity.FirstName + " " + smeEntity.LastName)
                                .Replace("@CourseName@", entity.CentralDocumentName)
                                .Replace("@Documents@", entity.document_type)
                                .Replace("@DeveloperName@", developerName);

                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "meetmayur87@gmail.com" : UtilityHelper.Decrypt(smeEntity.Email),
                                Subject = "SSTM Central COURSE ASSIGNMENTS UPDATES (" + entity.CentralDocumentName + ")",
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
                            return Json(new { result = false, message = "Central Course is assigned successfully to seleted SME. Notification Mail is not sent due to email settings not found." });
                        #endregion


                    }
                    else
                        return Json(new { result = false, message = AppMessages.Exception });
                }
                else
                    return Json(new { result = false, message = AppMessages.Exception });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Central Document", "AssignCourseToSME", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult openSMEComment()
        {
            try
            {
                return PartialView("_SME_developer_comment");
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "SME Comment Document", "OpenSMEList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }
        #endregion

        [HttpPost]
        public ActionResult openDocuments(int id)
        {
            try
            {
                var master = _ICentralizedMasterService.GetRecordById(id);
                List<Centralized_Document_files> centraldocfiles = new List<Centralized_Document_files>();
                List<Centralized_Document_files> filtercentraldocfiles = new List<Centralized_Document_files>();
                if (CurrentSession.UserRole == "Trainer" || CurrentSession.UserRole == "Print Incharge")
                {
                    centraldocfiles = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(id);
                    foreach (var item in centraldocfiles)
                    {
                        if (CurrentSession.UserRole == "Trainer")
                        {
                            var data = _ICentralCourseSharingService.GetRecordByCentraAndDocIds(master.id, item.id);
                            if (data != null)
                            {
                                if (data.isTraining == true)
                                {
                                    filtercentraldocfiles.Add(item);
                                }
                            }
                        }
                        if (CurrentSession.UserRole == "Print Incharge")
                        {
                            var data = _ICentralCourseSharingService.GetRecordByCentraAndDocIds(master.id, item.id);
                            if (data != null)
                            {
                                if (data.isPrinting == true)
                                {
                                    filtercentraldocfiles.Add(item);
                                }
                            }
                        }
                    }
                    centraldocfiles = filtercentraldocfiles;
                }
                else
                {
                    centraldocfiles = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(id);
                }
                return PartialView("_CentralizeDocumentsList", centraldocfiles);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "SME Comment Document", "OpenSMEList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>Exception: " + ex.Message + ".</div>");
            }
        }

        [HttpPost]
        public ActionResult Comment_SME_And_developer(long courseId, string SMEcomment, string developer)
        {
            try
            {
                var entity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(courseId));
                string statusString = "";
                if (CurrentSession.UserRole == "SME")
                    statusString = SMEcomment != "N/A" ? "Under Improvement" : "Released";//new
                else
                    statusString = "Under Review";//new

                if (SMEcomment == "N/A" && developer == "N/A")
                    statusString = "Released";//new

                entity.Statusid = _ICourseStatusService.GetRecordIdByName(statusString);
                entity.UpdatedBy = CurrentSession.UserId;
                entity.UpdatedOn = DateTime.Now;
                entity.sme_comment = SMEcomment;
                entity.developer_sme_comment_reply = developer;
                _ICentralizedMasterService.SaveRecord(entity);


                #region Send notification to director and developer for remarks if SME reviewed all the docs

                var configEntity = _IConfigService.GetFirstRecord();
                if (configEntity != null)
                {

                    var directorEntity = _IUserService.GetRecordById(Convert.ToInt64(entity.DirectorId));
                    User developerEntity = new User();
                    if (CurrentSession.UserRole == "SME")
                        developerEntity = _IUserService.GetRecordById(Convert.ToInt64(entity.CreatedBy));
                    else
                        developerEntity = _IUserService.GetRecordById(Convert.ToInt64(entity.sme_assign_id));

                    if (directorEntity != null || developerEntity != null)
                    {
                        var emailBody = UtilityHelper.GetEmailTemplate("RemarksSummaryNotificationToDirectorAndDeveloper.html").ToString();

                        if (SMEcomment != "N/A")
                            emailBody = emailBody
                                .Replace("@CourseStatusMessage@", "Kindly notice that the below course is approved by SME and there is no any remarks or suggestion as you can view in below table.<br /><br />");
                        else
                            emailBody = emailBody
                                .Replace("@CourseStatusMessage@", "Kindly notice that the below course is under improvement and you can view remarks and / or suggestion in below table. Developer needs to attend following remarks and / or suggestion and re-submitting the updated documents.<br /><br />");

                        emailBody = emailBody
                            .Replace("@DeveloperName@", developerEntity != null ? developerEntity.FirstName + " " + developerEntity.LastName : "")
                            .Replace("@SMEName@", CurrentSession.UserName)
                            .Replace("@CourseName@", entity.CentralDocumentName)
                            .Replace("@DocumentsRemarksSummary@", CurrentSession.UserRole != "SME" ? SMEcomment : developer);

                        try
                        {
                            EmailHelper.SendMail(new Models.EmailModel.EmailModel
                            {
                                From = configEntity.Email,
                                To = Request.IsLocal ? "meetmayur87@gmail.com" : directorEntity != null ? UtilityHelper.Decrypt(directorEntity.Email) + ";" + UtilityHelper.Decrypt(developerEntity.Email) : developerEntity != null ? UtilityHelper.Decrypt(developerEntity.Email) : "",
                                Subject = "SSTM COURSE ASSIGNMENTS UPDATES (" + entity.CentralDocumentName + ")",
                                Message = emailBody,
                                SMTPHost = configEntity.Host,
                                SMTPPort = configEntity.Port,
                                SMTPEmail = configEntity.Email,
                                SMTPPassword = configEntity.Pass,
                                EnableSsl = configEntity.EnableSsl
                            });
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                #endregion

                return Json(new { result = true, message = "Central Documents are submitted successfully. Course status is updated to " + statusString + "." });

            }
            catch (Exception ex)
            {
                return Json(new { result = false });
            }
            return Json(new { result = true });
        }
        [HttpPost]
        public ActionResult OpenCourseDocsSharing(int docId)
        {
            try
            {
                var list = _ICentralCourseSharingService.GetListofSharedCentralCourseDocs(docId).ToList();

                //foreach (var item in list.ToList())
                //{
                //    if (item.isDeleted)
                //        list.Remove(item);
                //}

                return PartialView("_CourseDocsSharing", list);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "CourseDoc", "OpenCourseDocsSharing", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }




        [HttpPost]
        public ActionResult ReleaseCourse(long CentralMaster_id)
        {
            try
            {
                var entity = _ICentralizedMasterService.GetRecordById(Convert.ToInt32(CentralMaster_id));

                if (entity != null)
                {
                    if (entity.Statusid == _ICourseStatusService.GetRecordIdByName("Released"))
                    {
                        return Json(new { result = false, message = "Selected central course already Released" });
                    }
                    //if (CurrentSession.UserRole != "Administration" || CurrentSession.UserRole != "Director")
                    //{
                    if (entity.sme_comment == "" && entity.developer_sme_comment_reply == "")
                    {
                        return Json(new { result = false, message = "Before release comment SME and Development comment in N/A Add !" });
                    }

                    if (entity.sme_comment != "N/A" && entity.developer_sme_comment_reply != "N/A")
                    {
                        return Json(new { result = false, message = "Please check comment inside developer and sme added comment so, replace Added N/A after release other wise wait developer or SME Approval." });
                    }

                    //}
                    #region Update status in Central Master table

                    string statusString = "Released";
                    entity.Statusid = _ICourseStatusService.GetRecordIdByName(statusString);
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                    _ICentralizedMasterService.SaveRecord(entity);
                    #endregion

                    return Json(new { result = true, message = "Succesfully relase selected central document" });
                }
                else
                    return Json(new { result = false, message = "No data found." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "ReleaseCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }



        [HttpPost]
        public ActionResult ShareCourse(CentralDocumentsListModel[] paramsList)
        {
            try
            {
                long docid = 0;
                if (paramsList.Length > 0)
                {
                    #region Save records into Course sharing table for training and printing
                    foreach (var item in paramsList)
                    {
                        docid = (int)item.DocId;
                        Central_CourseSharing courseSharingEntity = _ICentralCourseSharingService.GetRecordByCentraAndDocIds((int)item.CentralDocId, (int)item.DocId);
                        if (courseSharingEntity != null)
                        {
                            courseSharingEntity.UpdatedBy = CurrentSession.UserId;
                            courseSharingEntity.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            courseSharingEntity = new Central_CourseSharingModel().ToEntity();
                            courseSharingEntity.CourseId = Convert.ToInt64(item.CentralDocId);
                            courseSharingEntity.DocId = (int)item.DocId;
                            courseSharingEntity.CreatedBy = CurrentSession.UserId;
                            courseSharingEntity.CreatedOn = DateTime.Now;
                        }

                        courseSharingEntity.isTraining = item.isTraining;
                        courseSharingEntity.isPrinting = item.isPrinting;
                        courseSharingEntity.isDeveloper = item.isDeveloper;

                        _ICentralCourseSharingService.SaveRecord(courseSharingEntity);
                    }


                    #endregion

                    var sharedCourseDocsList = _ICentralCourseSharingService.GetListofSharedCentralCourseDocs((int)paramsList[0].CentralDocId).ToList();

                    var configEntity = _IConfigService.GetFirstRecord();

                    #region Send notification to Trainer 
                    var courseDocumentsList = sharedCourseDocsList.Where(a => a.isTraining).ToList();
                    if (courseDocumentsList.Count() > 0)
                    {
                        var documents = "";
                        courseDocumentsList.ForEach(a => { documents += System.IO.Path.GetFileNameWithoutExtension(a.Document_File_Name) != "N/A" ? System.IO.Path.GetFileNameWithoutExtension(a.Document_File_Name) + ", " : ""; });

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToTrainer.html").ToString();
                        emailBody = emailBody.Replace("@CourseName@", System.IO.Path.GetFileNameWithoutExtension(sharedCourseDocsList[0].Document_File_Name))
                            .Replace("@Documents@", documents.Trim().TrimEnd(','));

                        var trainersEmails = string.Empty;
                        var trainersList = _IUserService.GetList(1).Where(a => a.Role == "Trainer").ToList();
                        trainersList.ForEach(a => { trainersEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "meetmayur87@gmail.com" : trainersEmails.Trim().TrimEnd(';'),
                            Subject = "SSTM CENTRAL DOCUMENTS RELEASED (" + System.IO.Path.GetFileNameWithoutExtension(sharedCourseDocsList[0].Document_File_Name) + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                    }
                    #endregion

                    #region Send notification to Print Incharge
                    courseDocumentsList = sharedCourseDocsList.Where(a => a.isPrinting).ToList();
                    if (courseDocumentsList.Count() > 0)
                    {
                        var documents = "";
                        courseDocumentsList.ForEach(a => { documents += a.Document_File_Name != "N/A" ? a.Document_File_Name + ", " : ""; });

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToPrintIncharge.html").ToString();
                        emailBody = emailBody.Replace("@CourseName@", sharedCourseDocsList[0].Document_File_Name)
                            .Replace("@Documents@", documents.Trim().TrimEnd(','));

                        var printInchargesEmails = string.Empty;
                        var printInchargesList = _IUserService.GetList(1).Where(a => a.Role == "Print Incharge").ToList();
                        printInchargesList.ForEach(a => { printInchargesEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "meetmayur87@gmail.com" : printInchargesEmails.Trim().TrimEnd(';'),
                            Subject = "SSTM COURSE DOCUMENTS RELEASED (" + sharedCourseDocsList[0].Document_File_Name + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                    }
                    #endregion

                    #region Send notification to Developer 
                    var courseDocumentsDeveloperList = sharedCourseDocsList.Where(a => a.isDeveloper).ToList();
                    if (courseDocumentsDeveloperList.Count() > 0)
                    {
                        var documents = "";
                        courseDocumentsDeveloperList.ForEach(a => { documents += a.Document_File_Name != "N/A" ? a.Document_File_Name + ", " : ""; });

                        var emailBody = UtilityHelper.GetEmailTemplate("NotificationToTrainer.html").ToString();
                        emailBody = emailBody.Replace("@CourseName@", sharedCourseDocsList[0].Document_File_Name)
                            .Replace("@Documents@", documents.Trim().TrimEnd(','));

                        var developerEmails = string.Empty;
                        var trainersList = _IUserService.GetList(1).Where(a => a.Role == "Developer").ToList();
                        trainersList.ForEach(a => { developerEmails += UtilityHelper.Decrypt(a.Email) + ";"; });

                        EmailHelper.SendMail(new Models.EmailModel.EmailModel
                        {
                            From = configEntity.Email,
                            To = Request.IsLocal ? "meetmayur87@gmail.com" : developerEmails.Trim().TrimEnd(';'),
                            Subject = "SSTM COURSE DOCUMENTS RELEASED (" + sharedCourseDocsList[0].Document_File_Name + ")",
                            Message = emailBody,
                            SMTPHost = configEntity.Host,
                            SMTPPort = configEntity.Port,
                            SMTPEmail = configEntity.Email,
                            SMTPPassword = configEntity.Pass,
                            EnableSsl = configEntity.EnableSsl
                        });
                    }
                    #endregion
                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "No data found for sharing." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Course", "ShareCourse", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SubmitCourseDocuments(int CourseId)
        {
            try
            {
                var courseStatusList = _ICourseStatusService.GetList().ToList();
                var courseEntity = _ICentralizedMasterService.GetRecordById(CourseId);
                var courseDocsList = _ICentralizedDocumentFilesService.GetAllbyCenterRecord(CourseId).ToList();

                var isProceed = true;

                if (CurrentSession.UserRole == "Developer")
                {
                    if (courseEntity.sme_comment == "" || courseEntity.developer_sme_comment_reply == "")
                        isProceed = false;
                }

                if (CurrentSession.UserRole == "SME")
                {
                    if (courseEntity.sme_comment == "")
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

                    _ICentralizedMasterService.SaveRecord(courseEntity);
                    #endregion

                    #region Save record into course assignment table
                    long directorId = 0;
                    long smeId = 0;
                    if (CurrentSession.UserRole == "Developer" || CurrentSession.UserRole == "Administration" || CurrentSession.UserRole == "Staff" || CurrentSession.UserRole == "Director" || CurrentSession.UserRole == "Aassociate Developer")
                    {
                        //var courseAssignmentEntity = _ICourseAssignmentService.GetRecordByCourseId(CourseId);
                        //if (courseAssignmentEntity != null)
                        //{
                        directorId = Convert.ToInt64(courseEntity.DirectorId);

                        smeId = Convert.ToInt64(courseEntity.sme_assign_id);
                        courseEntity.developer_id = CurrentSession.UserId;
                        courseEntity.UpdatedBy = CurrentSession.UserId;
                        courseEntity.UpdatedOn = DateTime.Now;
                        _ICentralizedMasterService.SaveRecord(courseEntity);
                        //}
                    }

                    #endregion

                    var configEntity = _IConfigService.GetFirstRecord();
                    if (configEntity != null)
                    {
                        var smeEntity = _IUserService.GetRecordById(smeId);
                        directorId = Convert.ToInt64(courseEntity.DirectorId);
                        #region Send email notification to director
                        var directorEntity = _IUserService.GetRecordById(directorId);
                        if (directorEntity == null)
                        {
                            var directorEntity1 = _IUserService.GetDefaultList().ToList().Where(a => a.RoleId == Convert.ToInt64(4));
                            if (directorEntity1.Count() != 0)
                            {
                                directorEntity = directorEntity1.FirstOrDefault();
                            }
                        }
                        if (directorEntity != null)
                        {
                            var toEmails = UtilityHelper.Decrypt(directorEntity.Email);

                            var emailBody = UtilityHelper.GetEmailTemplate("NewDocNotificationToDirector.html").ToString();
                            emailBody = emailBody.Replace("@DearName@", directorEntity.FirstName + " " + directorEntity.LastName)
                                .Replace("@DeveloperName@", CurrentSession.UserName)
                                .Replace("@CourseName@", courseEntity.CentralDocumentName)
                                .Replace("@Documents@", courseEntity.document_type);

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
                                To = Request.IsLocal ? "meetmayur87@gmail.com" : toEmails,
                                Subject = "SSTM CENTRAL COURSE DOCUMENTS UPDATES (" + courseEntity.CentralDocumentName + ")",
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

                    return Json(new { result = true, message = "All documents for " + courseEntity.CentralDocumentName + " submitted to the Director and notification sent successfully." });
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
    }
    public class textreplacedata
    {
        public string lablename { get; set; }
        public string name { get; set; }
        public string repeate { get; set; }
        public string type { get; set; }
        public HttpPostedFileBase[] files { get; set; }
    }
}