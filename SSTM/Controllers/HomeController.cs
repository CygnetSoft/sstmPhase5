using LinqToExcel;
using RestSharp;
using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.ActivityLog;
using SSTM.Models.AWS;
using SSTM.Models.CourseDocument;
using SSTM.Models.CourseDocVersion;
using SSTM.Models.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    //[ErrorHandler]
    public class HomeController : Controller
    {
        #region Class Properties Declarations
        private readonly IUserService _IUserService;
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IConfigService _IConfigService;
        private readonly IActivityLogService _IActivityLogService;

        private readonly ICourseService _ICourseService;
        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ICourseDocVersionService _ICourseDocVersionService;
        private readonly ICentralizedMasterService _ICentralizedMasterService;
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definition
        public HomeController(IUserService userService, IExceptionLogService exceptionLogService, IConfigService configService,
            IActivityLogService activityLogService, ICourseService courseService, ICourseDocVersionService courseDocVersionService,
            ICourseDocumentService courseDocumentService, ICentralizedMasterService CentralizedMasterService)
        {
            _IUserService = userService;
            _IExceptionLogService = exceptionLogService;
            _IConfigService = configService;
            _IActivityLogService = activityLogService;

            _ICourseService = courseService;
            _ICourseDocumentService = courseDocumentService;
            _ICourseDocVersionService = courseDocVersionService;
            _ICentralizedMasterService = CentralizedMasterService;
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(ImportUsersModel importExcel)
        {
            if (ModelState.IsValid)
            {
                string path = Server.MapPath("~/Content/Upload/" + importExcel.file.FileName);
                importExcel.file.SaveAs(path);

                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                //Sheet Name
                excelConnection.Open();
                string tableName = excelConnection.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                excelConnection.Close();
                //End

                //  OleDbCommand cmd = new OleDbCommand("Select * from [" + tableName + "]", excelConnection);
                var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", excelConnection);

                excelConnection.Open();

                OleDbDataReader dReader;
                //  dReader = cmd.ExecuteReader();
                var ds = new DataSet();
                adapter.Fill(ds, "ExcelTable");
                DataTable dtable = ds.Tables["ExcelTable"];
                var excelFile = new ExcelQueryFactory(path);
                string sheetName = "Sheet1";
                var list = from a in excelFile.Worksheet<UserModel>(sheetName) select a;
                UserModel model = new UserModel();
                foreach (var a in list)
                {
                    var entity = new UserModel();
                    entity.CreatedBy = 0;
                    entity.CreatedOn = DateTime.Now;
                    entity.TrainingCenterId = 0;
                    entity.FirstName = a.FirstName;
                    entity.LastName = "Eversafe";
                    entity.Email = UtilityHelper.Encrypt(a.Email);
                    entity.Password = UtilityHelper.Encrypt(a.Password);
                    entity.Mobile = UtilityHelper.Encrypt(a.Mobile);
                    entity.MacAddress = "";
                    entity.MacAddress1 = "";
                    entity.RoleId = a.RoleId;
                    entity.isActive = a.isActive;
                    entity.isDeleted = false;
                    if (!string.IsNullOrEmpty(a.Trainer_AirLine_id.ToString()))
                        entity.Trainer_AirLine_id = a.Trainer_AirLine_id;
                    else
                        entity.Trainer_AirLine_id = 0;
                    _IUserService.SaveRecord(entity.ToEntity());

                }

                excelConnection.Close();

                ViewBag.Result = "Successfully Imported";
            }
            return View();
        }
        public ActionResult GetGeoLocation()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult ConfirmGeoLocation(string lat, string lng)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
        //        {
        //            var usersEntitiesList = _IUserService.GetDefaultList().Where(a => a.RoleId != 1 && a.RoleId != 4).ToList();

        //            if (usersEntitiesList.Count() > 0)
        //            {
        //                decimal latitude = Math.Round(Convert.ToDecimal(lat), 2);
        //                decimal longitude = Math.Round(Convert.ToDecimal(lng), 2);

        //                var userEntity = usersEntitiesList
        //                    .Where(a => Math.Round(Convert.ToDecimal(UtilityHelper.Decrypt(a.Latitude)), 2) == latitude &&
        //                    Math.Round(Convert.ToDecimal(UtilityHelper.Decrypt(a.Longitude)), 2) == longitude).Count();

        //                CurrentSession = new AppSession();

        //                if (userEntity > 0)
        //                {
        //                    CurrentSession.isLocationVerified = true;
        //                    return Json(new { result = true });
        //                }
        //                else
        //                {
        //                    CurrentSession.isLocationVerified = false;
        //                    return Json(new { result = false });
        //                }
        //            }
        //            else
        //                return Json(new { result = false });
        //        }
        //        else
        //            return Json(new { result = false });
        //    }
        //    catch (Exception ex)
        //    {
        //        _IExceptionLogService.SaveRecord(ex, "Home", "ConfirmGeoLocation", Request.Url.AbsoluteUri, 0);
        //        return Json(new { result = false });
        //    }
        //}

        public ActionResult GetHeaderNavbar()
        {
            return PartialView("_HeaderNavbar", CurrentSession);
        }

        public ActionResult GetMenuSidebar()
        {
            return PartialView("_Menusidebar", CurrentSession);
        }

        [HttpPost]
        public ActionResult GetSessionTimeout()
        {
            try
            {
                var conf = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                var section = (SessionStateSection)conf.GetSection("system.web/sessionState");

                int timeout = (int)section.Timeout.TotalMinutes * 1000 * 60;

                return Json(new { result = true, sessionTime = timeout });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Home", "GetSessionTimeout", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult ExtendSession()
        {
            try
            {
                var conf = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                var section = (SessionStateSection)conf.GetSection("system.web/sessionState");

                int timeout = (int)section.Timeout.TotalMinutes * 1000 * 60;

                Session.Timeout = timeout;

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Home", "ExtendSession", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, code = "Exception", message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public string saveDocument(string cId, string dId, string ext, string UserId)
        {
            string filePath = Path.Combine(Server.MapPath("~/Content/date.txt"));
            if (!System.IO.File.Exists(filePath))
                System.IO.File.Create(filePath).Close();

            StringBuilder filedata = new StringBuilder();
            try
            {
                filedata.Append("before name course id" + cId);
                System.IO.File.AppendAllText(filePath, "before name course id" + cId + Environment.NewLine);

                var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", UserId);
                if (!Directory.Exists(sourceDir))
                    Directory.CreateDirectory(sourceDir);

                #region New file 
                Random generator = new Random();
                string newFileName = generator.Next(0, 1000000).ToString("D10") + ext;
                string AwsSaveFileName = generator.Next(0, 1000000).ToString("D10") + ext;
                string NewDocxFile = Path.Combine(sourceDir, AwsSaveFileName);
                System.IO.File.Create(NewDocxFile).Close();
                #endregion

                var filename = Request.QueryString["filename"];
                if (filename == "")
                {
                    filename = newFileName;
                }

                System.IO.File.AppendAllText(filePath, "fname: " + filename + Environment.NewLine);
                filedata.Append("after name");
                System.IO.File.AppendAllText(filePath, "after name" + filename + Environment.NewLine);
                try
                {
                    Request.Files["content"].SaveAs(Path.Combine(sourceDir, filename));
                }
                catch (Exception)
                {
                    openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                }
                filedata.Append("Save File");
                System.IO.File.AppendAllText(filePath, "Save File" + Environment.NewLine);
                System.IO.File.AppendAllText(filePath, "Start convert File" + Environment.NewLine);
                #region  convert zdoc to docx
                if (filename != null)
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    var client = new RestClient("https://writer.zoho.com/writer/officeapi/v1/document/convert?apikey=" + UtilityHelper.zohoApikey);
                    client.Timeout = -1;

                    var request = new RestRequest(Method.POST);
                    request.AlwaysMultipartFormData = true;
                    request.AddFile("document", Path.Combine(sourceDir, filename));
                    request.AddParameter("output_options", "{'format':'docx','document_name':'new'}");

                    IRestResponse response = client.Execute(request);

                    //string filenamedata = filename == null ? "test" : Path.GetFileNameWithoutExtension(filename);
                    //string ExistmakeDocxFile = sourceDir + "\\" + filenamedata + ".docx";

                    filedata.Append("create docx file");

                    System.IO.File.WriteAllBytes(NewDocxFile, response.RawBytes);
                    System.IO.File.AppendAllText(filePath, "Convert end" + Environment.NewLine);
                    filedata.Append("write data to file docx");

                    Stream stream = new MemoryStream(response.RawBytes);
                    System.IO.File.AppendAllText(filePath, "delete file docx" + Environment.NewLine);
                    #region Upload & Delete  AWS File
                    filedata.Append("AWS Start ");
                    filedata.Append("start delete file ");

                    var configEntity = _IConfigService.GetFirstRecord();

                    filedata.Append("AWS delete file success");

                    System.IO.File.AppendAllText(filePath, "Aws Upload " + Environment.NewLine);
                    AWSModel awsModelUpload = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = cId.ToString(),
                        FileName = newFileName,
                        LocalFileStream = stream
                    };
                    try
                    {
                        var data = AWSHelper.UploadFile(awsModelUpload);
                        System.IO.File.AppendAllText(filePath, "Aws Upload end " + data + Environment.NewLine);
                    }
                    catch (Exception)
                    {
                        openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                    }
                    #endregion

                    try
                    {
                        System.IO.File.AppendAllText(filePath, "delete file" + Environment.NewLine);
                        try
                        {
                            if (System.IO.File.Exists(Path.Combine(sourceDir, filename)))
                                System.IO.File.Delete(Path.Combine(sourceDir, filename));
                        }
                        catch (Exception)
                        {
                            openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                        }

                        if (System.IO.File.Exists(Path.Combine(sourceDir, newFileName)))
                            System.IO.File.Delete(Path.Combine(sourceDir, newFileName));

                        System.IO.File.AppendAllText(filePath, "End delete file" + Environment.NewLine);
                        filedata.Append("File delete aws ");
                    }
                    catch (Exception exDel)
                    {
                        System.IO.File.AppendAllText(filePath, "File delete aws Exception:" + exDel.Message + Environment.NewLine);
                        openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                    }
                    try
                    {
                        System.IO.File.AppendAllText(filePath, "vesrion save" + Environment.NewLine);
                        saveDocumentWithVersion(newFileName, long.Parse(dId.ToString()), long.Parse(UserId));
                        System.IO.File.AppendAllText(filePath, "End vesrion save" + Environment.NewLine);

                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllText(filePath, ex.Message + Environment.NewLine);
                        openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                    }
                }
                #endregion
                System.IO.File.AppendAllText(filePath, "SUccess" + Environment.NewLine + Environment.NewLine);


                return "";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(filePath, ex.Message + Environment.NewLine);
                openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
            }
            return "";
        }

        [HttpPost]
        public string PPTSaveDocument(string cId, string dId, string ext, string UserId)
        {
            string filePath = Path.Combine(Server.MapPath("~/Content/pptdate.txt"));
            if (!System.IO.File.Exists(filePath))
                System.IO.File.Create(filePath).Close();

            StringBuilder sBuilderLogs = new StringBuilder();
            try
            {
                sBuilderLogs.Append("before name course id" + cId);
                System.IO.File.AppendAllText(filePath, "before name course id" + cId + Environment.NewLine);
                var filename = Request["filename"];
                sBuilderLogs.Append("after name");
                var courseDocEntity = _ICourseDocumentService.GetRecordById(Convert.ToInt64(dId));

                var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", UserId);
                #region New file 
                Random generator = new Random();
                //string newFileName = generator.Next(0, 1000000).ToString("D10") + ext;
                string courseDocVersion = "1";
                string newFileName = "";
                var courseDocVersionEntity = _ICourseDocVersionService.GetLatestRecordByDocId(Convert.ToInt64(dId));
                if (courseDocVersionEntity != null)
                {
                    courseDocVersion = courseDocVersionEntity.Version;
                    courseDocVersion = courseDocVersion + 1;
                    newFileName = courseDocEntity.DocName.Replace(" ", "") + "_V" + courseDocVersion + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ext;
                }
                else
                    newFileName = courseDocEntity.DocName.Replace(" ", "") + "_V" + courseDocVersion + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ext;

                string NewDocxFile = Path.Combine(sourceDir, newFileName);
                #endregion

                var fname = Request.QueryString["filename"];

                System.IO.File.AppendAllText(filePath, "before name old" + fname + Environment.NewLine);
                fname = newFileName;
                System.IO.File.AppendAllText(filePath, "before name new" + fname + Environment.NewLine);
                if (!Directory.Exists(sourceDir))
                    Directory.CreateDirectory(sourceDir);
                try
                {
                    //file save remove //md
                    //if (Request.Files["content"] != null)
                    //    Request.Files["content"].SaveAs(Path.Combine(sourceDir, fname));
                }
                catch (Exception)
                {
                    openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                }
                System.IO.File.AppendAllText(filePath, "file upload" + Environment.NewLine);
                #region  convert zdoc to pptx or xls
                if (filename != null)
                {
                    #region Upload & Delete  AWS File
                    sBuilderLogs.Append("AWS Start");
                    sBuilderLogs.Append("start delete file ");

                    var configEntity = _IConfigService.GetFirstRecord();
                    sBuilderLogs.Append("AWS delete file success");

                    System.IO.File.AppendAllText(filePath, "AWS Upload file" + Environment.NewLine);

                    AWSModel awsModelUpload = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = cId.ToString(),
                        FileName = fname,
                        LocalFileStream = Request.Files["content"].InputStream
                    };
                    try
                    {
                        AWSHelper.UploadFile(awsModelUpload);
                        System.IO.File.AppendAllText(filePath, "AWS done Upload file" + Environment.NewLine);
                        sBuilderLogs.Append("AWS Upload file" + filename);
                    }
                    catch (Exception)
                    {
                        openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                    }
                    #endregion
                }
                #endregion

                try
                {
                    if (System.IO.File.Exists(Path.Combine(sourceDir, fname)))
                        System.IO.File.Delete(Path.Combine(sourceDir, fname));

                    System.IO.File.AppendAllText(filePath, "delete file" + Environment.NewLine);
                }
                catch (Exception exDel)
                {
                    sBuilderLogs.Append("End with FIle delete " + exDel.Message);
                    System.IO.File.AppendAllText(filePath, exDel.Message + Environment.NewLine);
                    openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                }

                try
                {
                    sBuilderLogs.Append("Save version file ");
                    saveDocumentWithVersion(fname, long.Parse(dId.ToString()), long.Parse(UserId));
                    sBuilderLogs.Append("End Save version file ");
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(filePath, ex.Message + Environment.NewLine);
                    openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                }
                openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                return "";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(filePath, ex.Message + Environment.NewLine);
                openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
            }
            return "";
        }


        [HttpPost]
        public string CentralPPTSaveDocument(string Folderpath, string filename, string ext, string UserId)
        {
            StringBuilder sBuilderLogs = new StringBuilder();

            string filePath = Path.Combine(Server.MapPath("~/Content/Centralpptdate.txt"));
            if (!System.IO.File.Exists(filePath))
                System.IO.File.Create(filePath).Close();

            try
            {
                sBuilderLogs.Append("before name course path" + Folderpath);
                System.IO.File.AppendAllText(filePath, "before name course path" + Folderpath + Environment.NewLine);
                var filename1 = Request["filename"];
                System.IO.File.AppendAllText(filePath, "after filebane" + Environment.NewLine);
                sBuilderLogs.Append("after name");
                // var courseDocEntity = _ICourseDocumentService.GetRecordById(Convert.ToInt64(dId));

                var sourceDir = Path.Combine(Server.MapPath("~/Content"), "Temp", @"CentralCourse\CourseOutputFiles\" + Folderpath.Trim());
                #region New file 
                Random generator = new Random();
                //string newFileName = generator.Next(0, 1000000).ToString("D10") + ext;
                //string courseDocVersion = "1";
                //string newFileName = "";
                //var courseDocVersionEntity = _ICourseDocVersionService.GetLatestRecordByDocId(Convert.ToInt64(dId));
                //if (courseDocVersionEntity != null)
                //{
                //    courseDocVersion = courseDocVersionEntity.Version;
                //    courseDocVersion = courseDocVersion + 1;
                //    newFileName = courseDocEntity.DocName.Replace(" ", "") + "_V" + courseDocVersion + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ext;
                //}
                //else
                //    newFileName = courseDocEntity.DocName.Replace(" ", "") + "_V" + courseDocVersion + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ext;

                // string NewDocxFile = Path.Combine(sourceDir, Request.QueryString["filename"]);
                #endregion

                var fname = Request.QueryString["filename"];
                System.IO.File.AppendAllText(filePath, "filename" + fname + Environment.NewLine);
                //System.IO.File.AppendAllText(filePath, "before name old" + fname + Environment.NewLine);
                fname = filename;
                //System.IO.File.AppendAllText(filePath, "before name new" + fname + Environment.NewLine);
                if (!Directory.Exists(sourceDir))
                    Directory.CreateDirectory(sourceDir);
                try
                {
                    //file save remove //md
                    //if (Request.Files["content"] != null)
                    //    Request.Files["content"].SaveAs(Path.Combine(sourceDir, fname));
                }
                catch (Exception)
                {
                    //openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                }
                System.IO.File.AppendAllText(filePath, "file upload" + Environment.NewLine);
                #region  convert zdoc to pptx or xls
                if (filename != null)
                {
                    #region Upload & Delete  AWS File
                    sBuilderLogs.Append("AWS Start");
                    sBuilderLogs.Append("start delete file ");

                    var configEntity = _IConfigService.GetFirstRecord();
                    sBuilderLogs.Append("AWS delete file success");

                    System.IO.File.AppendAllText(filePath, "AWS Upload file" + Environment.NewLine);

                    AWSModel awsModelUpload = new AWSModel()
                    {
                        AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                        SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                        BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                        BucketDirectory = "CentralCourse/CourseOutputFiles/" + Folderpath.Trim(),
                        FileName = fname.Trim(),
                        LocalFileStream = Request.Files["content"].InputStream
                    };
                    try
                    {
                        AWSHelper.UploadFile(awsModelUpload);
                        try
                        {

                            if (fname.Trim() == "CentralizePPT.pptx")
                            {

                                var RawPPTPath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + Folderpath+ "/CentralizePPT.pptx");

                                if (System.IO.File.Exists(RawPPTPath))
                                    System.IO.File.Delete(RawPPTPath);

                                System.IO.File.Create(RawPPTPath).Close();

                                sBuilderLogs.Append("Central Start PPT");
                                AWSModel awsModel = new AWSModel()
                                {
                                    AccessKey = UtilityHelper.Decrypt(configEntity.AWSAccessKey),
                                    SecreteKey = UtilityHelper.Decrypt(configEntity.AWSSecretKey),
                                    BucketName = UtilityHelper.Decrypt(configEntity.BucketName),
                                    BucketDirectory = "CentralCourse/CourseOutputFiles/" + Folderpath,
                                    FileName = "CentralizePPT.pptx",
                                    FilePath = Server.MapPath("~/Content/CenteralizedCourseSamplePPT/CourseRawFiles/" + Folderpath + "/CentralizePPT.pptx")
                                };

                                bool status = AWSHelper.GetSingleFile(awsModel);
                                sBuilderLogs.Append("Get Letest Success full file End");
                            }
                        }
                        catch (Exception ex)
                        {
                            sBuilderLogs.Append("Get Letest File PPT : " + ex.Message);
                        }
                        System.IO.File.AppendAllText(filePath, "AWS done Upload file" + Environment.NewLine);
                        sBuilderLogs.Append("AWS Upload file" + filename);
                    }
                    catch (Exception)
                    {
                        //openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                    }
                    #endregion
                }
                #endregion

                try
                {
                    if (System.IO.File.Exists(Path.Combine(sourceDir, fname)))
                        System.IO.File.Delete(Path.Combine(sourceDir, fname));

                    System.IO.File.AppendAllText(filePath, "delete file" + Environment.NewLine);
                }
                catch (Exception exDel)
                {
                    sBuilderLogs.Append("End with FIle delete " + exDel.Message);
                    System.IO.File.AppendAllText(filePath, exDel.Message + Environment.NewLine);
                    // openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                }

                try
                {
                    sBuilderLogs.Append("Save version file ");
                    //saveDocumentWithVersion(fname, long.Parse(dId.ToString()), long.Parse(UserId));
                    sBuilderLogs.Append("End Save version file ");
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(filePath, ex.Message + Environment.NewLine);
                    //openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                }
                // openFileClose(long.Parse(dId.ToString()), long.Parse(UserId));
                return "";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(filePath, ex.Message + Environment.NewLine);
                openFileClose(0, long.Parse(UserId));
            }
            return "";
        }

        public void saveDocumentWithVersion(string fileName, long dId, long userid)
        {
            try
            {
                #region Save record into Course Document table
                var courseDocEntity = _ICourseDocumentService.GetRecordById(dId);

                courseDocEntity.Filename = fileName;
                courseDocEntity.isOpened = false;
                courseDocEntity.UpdatedBy = userid;
                courseDocEntity.UpdatedOn = DateTime.Now;

                _ICourseDocumentService.SaveRecord(courseDocEntity);
                #endregion

                #region Updating document versioning info
                string courseDocVersion = "1";

                var courseDocVersionEntity = _ICourseDocVersionService.GetLatestRecordByDocId(dId);
                if (courseDocVersionEntity != null)
                {
                    courseDocVersion = courseDocVersionEntity.Version;

                    courseDocVersionEntity.isActive = false;
                    courseDocVersionEntity.UpdatedBy = userid;
                    courseDocVersionEntity.UpdatedOn = DateTime.Now;
                    _ICourseDocVersionService.SaveRecord(courseDocVersionEntity);

                    courseDocVersion = courseDocVersion + "_Auto_" + courseDocVersionEntity.Id;
                }

                _ICourseDocVersionService.SaveRecord(new CourseDocVersionModel()
                {
                    AuthorId = userid,
                    DocId = courseDocEntity.Id,
                    FileName = fileName,
                    Version = courseDocVersion + "_Auto_" + courseDocVersionEntity.Id,
                    VersionDate = DateTime.Now,
                    isActive = true,
                    CreatedBy = userid,
                    CreatedOn = DateTime.Now
                }.ToEntity());
                #endregion
            }
            catch (Exception ex)
            { string s = ex.Message; }
        }

        public void openFileClose(long dId, long userid)
        {
            try
            {
                #region Save record into Course Document table
                var courseDocEntity = _ICourseDocumentService.GetRecordById(dId);
                courseDocEntity.isOpened = false;
                courseDocEntity.UpdatedBy = userid;
                courseDocEntity.UpdatedOn = DateTime.Now;
                _ICourseDocumentService.SaveRecord(courseDocEntity);
                #endregion
            }
            catch (Exception ex)
            { string e = ex.Message; }
        }
    }
}
public class ImportUsersModel
{
    [Required(ErrorMessage = "Please select file")]
    [FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
    public HttpPostedFileBase file { get; set; }
}