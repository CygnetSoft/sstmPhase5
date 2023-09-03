//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Presentation;
using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.DBHandlers;
using SSTM.Helpers.Helpers;
using SSTM.Helpers.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class BulkImportExportController : BaseController
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;

        private readonly ICourseService _ICourseService;

        private readonly ICourseDocumentService _ICourseDocumentService;
        private readonly ICourseDocVersionService _ICourseDocVersionService;
        private readonly ITrainerUploadDocumentService _ITrainerUploadDocumentService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public BulkImportExportController(IExceptionLogService exceptionLogService, ICourseService courseService,
            ICourseDocumentService courseDocumentService, 
            ICourseDocVersionService courseDocVersionService,
            ITrainerUploadDocumentService ITrainerUploadDocumentService)
        {
            _IExceptionLogService = exceptionLogService;

            _ICourseService = courseService;

            _ICourseDocumentService = courseDocumentService;
            _ICourseDocVersionService = courseDocVersionService;
            _ITrainerUploadDocumentService = ITrainerUploadDocumentService;
        }
        #endregion

        // GET: BulkImportExport
        public ActionResult Index()
        {   
            GetCoursesList();
            return View();
        }

        
        public ActionResult ExportCourse()
        {
            GetCoursesList();
            GetDocument();
            return View();
        }
        #region GetCoursesList
        public void GetCoursesList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "Select Course" });

            var reviewdCoursesList = _ICourseService.GetCoursesWithotStatus(1, 5).ToList().Where(d => d.CourseType == "other" && d.MasterCourse == true && d.MasterCoursId == 0).ToList();//show all course in dropdown.

            reviewdCoursesList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.CourseName });
            });


            list.GroupBy(p => p.Value);

            TempData["CoursesList"] = new SelectList(list.OrderBy(o => o.Value), "Value", "Text");
        }

        public void GetDocument()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "Select Document Name" });

            var reviewdCoursesList = _ITrainerUploadDocumentService.GetCommonUploadDocsList(1, true, 0).ToList();

            reviewdCoursesList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.DocumentName });
            });


            list.GroupBy(p => p.Value);

            TempData["DocumentList"] = new SelectList(list.OrderBy(o => o.Value), "Value", "Text");
        }

        public JsonResult Getmst_CoursesListwithjson(string type)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "Select Course" });

            var reviewdCoursesList = _ICourseService.GetCoursesWithotStatus(1, 5).ToList().Where(d => d.CourseType == type && d.MasterCourse == true && d.MasterCoursId == 0).ToList();//show all course in dropdown.
            return Json(new { result = true, content = reviewdCoursesList }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region DownloadDocumentFolder

        string mastercoursename = "";
        public JsonResult DownloadDocumentFolder(int mainid,string type)
        {
            mastercoursename = "";
            string path = "";
            Utility.WriteToFile("{0}: SSTMDownloads Service start.");
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
                        var awsModel = new AWSModel()
                        {
                            AccessKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSAccessKey"])),
                            SecreteKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSSecretKey"])),
                            BucketName = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["BucketName"]))
                        };
                        objDbLibrary.MasterCourse = true; //Main Course
                        objDbLibrary.MasterCoursId = mainid;//master course
                        objDbLibrary.type = type;//Course Type
                        var dtMainCourses = objDbLibrary.GetDataTable("sstmo.GetCourses_First_Download", objDbLibrary.GrvFilldataCoursewithCourseandSubCourse);
                        Utility.WriteToFile("{0}: SP RUn.");
                        if (dtMainCourses != null && dtMainCourses.Rows.Count > 0)
                        {
                            foreach (DataRow courseRowMain in dtMainCourses.Rows)
                            {
                                try
                                {
                                    System.IO.DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Content/downloadCourseFile/"));

                                    foreach (FileInfo file in dir.GetFiles())
                                    {
                                        if (file.Extension == ".zip")
                                            file.Delete();
                                    }
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                                //main start
                                var destinationDirMain = Path.Combine(Server.MapPath("~/Content/downloadCourseFile/"), Convert.ToString(courseRowMain["CourseName"]));

                                if (Directory.Exists(destinationDirMain))
                                    Directory.Delete(destinationDirMain, true);

                                mastercoursename = Convert.ToString(courseRowMain["CourseName"]);
                                //if (Directory.Exists(destinationDirMain))
                                //    Directory.Delete(destinationDirMain, true);

                                if (!Directory.Exists(destinationDirMain))
                                    Directory.CreateDirectory(destinationDirMain);

                                //if (Convert.ToBoolean(courseRowMain["isTraining"]))
                                //{
                                Utility.WriteToFile("{0}: SP RUn.");
                                downloadDocument(destinationDirMain, Convert.ToString(courseRowMain["CourseName"]), long.Parse(courseRowMain["CourseId"].ToString()), awsModel);
                                //}
                                //main end

                                //Sub course 1 start
                                var objDbLibrarysub1 = new DbLibrary();
                                objDbLibrarysub1.MasterCourse = false; //sub Course
                                objDbLibrarysub1.MasterCoursId = long.Parse(Convert.ToString(courseRowMain["CourseId"])); //sub Course
                                objDbLibrary.type = type;//Course Type

                                var dtSub1 = objDbLibrarysub1.GetDataTable("sstmo.GetListOfCourses_download", objDbLibrarysub1.GrvFilldataCoursewithCourseandSubCourse);
                                if (dtSub1 != null && dtSub1.Rows.Count > 0)
                                {
                                    foreach (DataRow courseRowsub in dtSub1.Rows)
                                    {
                                        var destinationDirsub1 = Path.Combine(Server.MapPath("~/Content/downloadCourseFile/") + courseRowMain["CourseName"], Convert.ToString(courseRowsub["CourseName"]));

                                        //if (Directory.Exists(destinationDirsub1))
                                        //    Directory.Delete(destinationDirsub1, true);

                                        if (!Directory.Exists(destinationDirsub1))
                                            Directory.CreateDirectory(destinationDirsub1);
                                        //if (Convert.ToBoolean(courseRowsub["isTraining"]))
                                        //{
                                        downloadDocument(destinationDirsub1, Convert.ToString(courseRowsub["CourseName"]), long.Parse(courseRowsub["CourseId"].ToString()), awsModel);
                                        //}

                                        //start sub 2
                                        var objDbLibrarysub2 = new DbLibrary();
                                        objDbLibrarysub2.MasterCourse = false; //sub Course
                                        objDbLibrarysub2.MasterCoursId = long.Parse(Convert.ToString(courseRowsub["CourseId"])); //sub Course
                                        objDbLibrary.type = type;//Course Type

                                        var dtSub2 = objDbLibrarysub2.GetDataTable("sstmo.GetListOfCourses_download", objDbLibrarysub2.GrvFilldataCoursewithCourseandSubCourse);
                                        if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                        {
                                            foreach (DataRow courseRowsub2 in dtSub2.Rows)
                                            {
                                                var destinationDirsub2 = Path.Combine(Server.MapPath("~/Content/downloadCourseFile/") + courseRowMain["CourseName"] + "\\" + Convert.ToString(courseRowsub["CourseName"]), Convert.ToString(courseRowsub2["CourseName"]));

                                                //if (Directory.Exists(destinationDirsub2))
                                                //    Directory.Delete(destinationDirsub2, true);

                                                if (!Directory.Exists(destinationDirsub2))
                                                    Directory.CreateDirectory(destinationDirsub2);
                                                //if (Convert.ToBoolean(courseRowsub2["isTraining"]))
                                                //{
                                                downloadDocument(destinationDirsub2, Convert.ToString(courseRowsub2["CourseName"]), long.Parse(courseRowsub2["CourseId"].ToString()), awsModel);
                                                //}

                                                //start sub folder 3 

                                                var objDbLibrarysub3 = new DbLibrary();
                                                objDbLibrarysub3.MasterCourse = false; //sub Course
                                                objDbLibrarysub3.MasterCoursId = long.Parse(Convert.ToString(courseRowsub2["CourseId"])); //sub Course
                                                objDbLibrary.type = type;//Course Type

                                                var dtSub3 = objDbLibrarysub3.GetDataTable("sstmo.GetListOfCourses_download", objDbLibrarysub3.GrvFilldataCoursewithCourseandSubCourse);
                                                if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                                {
                                                    foreach (DataRow courseRowsub3 in dtSub3.Rows)
                                                    {
                                                        var destinationDirsub3 = Path.Combine(Server.MapPath("~/Content/downloadCourseFile/") + courseRowMain["CourseName"] + "\\" + Convert.ToString(courseRowsub["CourseName"]) + "\\" + Convert.ToString(courseRowsub2["CourseName"]), Convert.ToString(courseRowsub3["CourseName"]));

                                                        //if (Directory.Exists(destinationDirsub3))
                                                        //    Directory.Delete(destinationDirsub3, true);

                                                        if (!Directory.Exists(destinationDirsub3))
                                                            Directory.CreateDirectory(destinationDirsub3);
                                                        //if (Convert.ToBoolean(courseRowsub3["isTraining"]))
                                                        //{
                                                        downloadDocument(destinationDirsub3, Convert.ToString(courseRowsub3["CourseName"]), long.Parse(courseRowsub3["CourseId"].ToString()), awsModel);
                                                        //}

                                                        //start sub folder 3 


                                                        //end 
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

                 path = Download();
                return Json(new { result = true, message = path });
            }
            catch (Exception ex)
            {
                Utility.WriteToFile("{0}: Exception :: " + ex.Message + ex.StackTrace);
                Utility.WriteToFile("{0}: ************************************************************************");
                return Json(new { result = false, message = "Error" });
            }
            return Json(new { result = true, message = path });

            Utility.WriteToFile("{0}: SSTMDownloads Service end.");
        }

        #endregion

        #region download zip file

        public string Download()
        {
            try
            {
                if(string.IsNullOrEmpty(mastercoursename))
                    mastercoursename = mastercoursenameother;

                string path = Path.Combine(Server.MapPath("~/Content/downloadCourseFile/"), mastercoursename.Replace(" ", "") + ".zip");
                using (ZipFile zip = new ZipFile())
                {
                    //add directory, give it a name
                    zip.AddDirectory(Path.Combine(Server.MapPath("~/Content/downloadCourseFile/"), mastercoursename));
                    zip.Save(path);
                }
                string s = Request.Url.Authority;
                
                return s + "/Content/downloadCourseFile/" + mastercoursename.Replace(" ", "") + ".zip";
            }
            catch (Exception)
            {
                return ""; ;
            }
        }

        public string OtherDownload()
        {
            try
            {
                if (string.IsNullOrEmpty(mastercoursename))
                    mastercoursename = mastercoursenameother;

                string path = Path.Combine(Server.MapPath("~/Content/downloadTrainer/"), mastercoursename.Replace(" ", "") + ".zip");
                using (ZipFile zip = new ZipFile())
                {
                    //add directory, give it a name
                    zip.AddDirectory(Path.Combine(Server.MapPath("~/Content/downloadTrainer/"), mastercoursename));
                    zip.Save(path);
                }
                string s = Request.Url.Authority;

                return s + "/Content/downloadTrainer/" + mastercoursename.Replace(" ", "") + ".zip";
            }
            catch (Exception)
            {
                return ""; ;
            }
        }

        #endregion

        #region Download File from aws server


        public void downloadDocument(string destinationDir, string CourseName, long CourseId, AWSModel awsModel)
        {
            Utility.WriteToFile("{0}: Checking for Course: " + Convert.ToString(CourseName));
            var objDbLibrary = new DbLibrary();
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            awsModel.BucketDirectory = Convert.ToString(CourseId);

            var parameters = new SqlParameter[1] { new SqlParameter("@CourseId", Convert.ToInt32(CourseId)) };

            var sharedCoursesDocs = objDbLibrary.GetDataTable("sstmo.GetListofSharedCourseDocs_download", parameters);
            if (sharedCoursesDocs != null && sharedCoursesDocs.Rows.Count > 0)
            {
                foreach (DataRow sharedCoursesDocRow in sharedCoursesDocs.Rows)
                {
                    try
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

                        AWS.GetSingleFile(awsModel);

                        Utility.WriteToFile("{0}: Course document updated: " + Convert.ToString(sharedCoursesDocRow["DocName"]) + ".");
                      
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
                Utility.WriteToFile("{0}: No documents found for " + Convert.ToString(CourseName) + ".");

        }
        #endregion

        public JsonResult DriveBackupCourse()
        {
            DriveInfo drive = DriveInfo.GetDrives().Where(x => x.Name == @"D:\").FirstOrDefault();
            if(drive==null)
            {
                return Json(new { result = true, message ="Drive not found" });
            }

            Utility.WriteToFile("{0}: SSTMDownloads Service start.");
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
                        var awsModel = new AWSModel()
                        {
                            AccessKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSAccessKey"])),
                            SecreteKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSSecretKey"])),
                            BucketName = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["BucketName"]))
                        };
                        objDbLibrary.MasterCourse = true; //Main Course
                        objDbLibrary.MasterCoursId = 0;//master course
                        var dtMainCourses = objDbLibrary.GetDataTable("sstmo.GetListOfCourses_download", objDbLibrary.GrvFilldataCoursewithCourseandSubCourse);
                        Utility.WriteToFile("{0}: SP RUn.");
                        if (dtMainCourses != null && dtMainCourses.Rows.Count > 0)
                        {
                            foreach (DataRow courseRowMain in dtMainCourses.Rows)
                            {
                                //main start
                                var destinationDirMain = Path.Combine(@"d:\SSTMBackup\", Convert.ToString(courseRowMain["CourseName"]));

                                try
                                {


                                    if (Directory.Exists(destinationDirMain))
                                        Directory.Delete(destinationDirMain, true);
                                }
                                catch (Exception)
                                {

                                }
                                //if (Directory.Exists(destinationDirMain))
                                //    Directory.Delete(destinationDirMain, true);

                                if (!Directory.Exists(destinationDirMain))
                                    Directory.CreateDirectory(destinationDirMain);

                                //if (Convert.ToBoolean(courseRowMain["isTraining"]))
                                //{
                                Utility.WriteToFile("{0}: SP RUn.");
                                downloadDocument(destinationDirMain, Convert.ToString(courseRowMain["CourseName"]), long.Parse(courseRowMain["CourseId"].ToString()), awsModel);
                                //}
                                //main end

                                //Sub course 1 start
                                var objDbLibrarysub1 = new DbLibrary();
                                objDbLibrarysub1.MasterCourse = false; //sub Course
                                objDbLibrarysub1.MasterCoursId = long.Parse(Convert.ToString(courseRowMain["CourseId"])); //sub Course

                                var dtSub1 = objDbLibrarysub1.GetDataTable("sstmo.GetListOfSharedCourses", objDbLibrarysub1.GrvFilldataCoursewithCourseandSubCourse);
                                if (dtSub1 != null && dtSub1.Rows.Count > 0)
                                {
                                    foreach (DataRow courseRowsub in dtSub1.Rows)
                                    {
                                        var destinationDirsub1 = Path.Combine(@"d:\SSTMBackup\" + courseRowMain["CourseName"], Convert.ToString(courseRowsub["CourseName"]));

                                        //if (Directory.Exists(destinationDirsub1))
                                        //    Directory.Delete(destinationDirsub1, true);

                                        if (!Directory.Exists(destinationDirsub1))
                                            Directory.CreateDirectory(destinationDirsub1);
                                        //if (Convert.ToBoolean(courseRowsub["isTraining"]))
                                        //{
                                        downloadDocument(destinationDirsub1, Convert.ToString(courseRowsub["CourseName"]), long.Parse(courseRowsub["CourseId"].ToString()), awsModel);
                                        //}

                                        //start sub 2
                                        var objDbLibrarysub2 = new DbLibrary();
                                        objDbLibrarysub2.MasterCourse = false; //sub Course
                                        objDbLibrarysub2.MasterCoursId = long.Parse(Convert.ToString(courseRowsub["CourseId"])); //sub Course

                                        var dtSub2 = objDbLibrarysub2.GetDataTable("sstmo.GetListOfSharedCourses", objDbLibrarysub2.GrvFilldataCoursewithCourseandSubCourse);
                                        if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                        {
                                            foreach (DataRow courseRowsub2 in dtSub2.Rows)
                                            {
                                                var destinationDirsub2 = Path.Combine(@"d:\SSTMBackup\" + courseRowMain["CourseName"] + "\\" + Convert.ToString(courseRowsub["CourseName"]), Convert.ToString(courseRowsub2["CourseName"]));

                                                //if (Directory.Exists(destinationDirsub2))
                                                //    Directory.Delete(destinationDirsub2, true);

                                                if (!Directory.Exists(destinationDirsub2))
                                                    Directory.CreateDirectory(destinationDirsub2);
                                                //if (Convert.ToBoolean(courseRowsub2["isTraining"]))
                                                //{
                                                downloadDocument(destinationDirsub2, Convert.ToString(courseRowsub2["CourseName"]), long.Parse(courseRowsub2["CourseId"].ToString()), awsModel);
                                                //}

                                                //start sub folder 3 

                                                var objDbLibrarysub3 = new DbLibrary();
                                                objDbLibrarysub3.MasterCourse = false; //sub Course
                                                objDbLibrarysub3.MasterCoursId = long.Parse(Convert.ToString(courseRowsub2["CourseId"])); //sub Course

                                                var dtSub3 = objDbLibrarysub3.GetDataTable("sstmo.GetListOfSharedCourses", objDbLibrarysub3.GrvFilldataCoursewithCourseandSubCourse);
                                                if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                                {
                                                    foreach (DataRow courseRowsub3 in dtSub3.Rows)
                                                    {
                                                        var destinationDirsub3 = Path.Combine(@"d:\SSTMBackup\" + courseRowMain["CourseName"] + "\\" + Convert.ToString(courseRowsub["CourseName"]) + "\\" + Convert.ToString(courseRowsub2["CourseName"]), Convert.ToString(courseRowsub3["CourseName"]));

                                                        //if (Directory.Exists(destinationDirsub3))
                                                        //    Directory.Delete(destinationDirsub3, true);

                                                        if (!Directory.Exists(destinationDirsub3))
                                                            Directory.CreateDirectory(destinationDirsub3);

                                                        downloadDocument(destinationDirsub3, Convert.ToString(courseRowsub3["CourseName"]), long.Parse(courseRowsub3["CourseId"].ToString()), awsModel);


                                                        //start sub folder 3 


                                                        //end 
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

                //Stop the Windows Service.
                //using (ServiceController serviceController = new ServiceController("SSTMDownloads"))
                //{
                //    serviceController.Stop();
                //}
            }
            string path = Download();
            return Json(new { result = true, message = path });
            Utility.WriteToFile("{0}: SSTMDownloads Service end.");
        }


        public ActionResult uploadVideoFiles()
        {
           
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "", Text = "Select Document Name" });

            var reviewdCoursesList = _ITrainerUploadDocumentService.GetCommonUploadDocsList(1, true, 0).ToList();

            reviewdCoursesList.ForEach(a =>
            {
                list.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.DocumentName });
            });


            list.GroupBy(p => p.Value);

            TempData["DocumentList"] = new SelectList(list.OrderBy(o => o.Value), "Value", "Text");
            return View();
        }

        public ActionResult VideoFiles(int CourseType,bool? MasterDoc, long? MasterDocId)
        {
            if (MasterDoc == null)
                MasterDoc = true;
            if (MasterDocId == null)
                MasterDocId = 0;

            ViewBag.MasterDoc = MasterDoc.ToString();
            ViewBag.MasterDocId = MasterDocId;

            var list = _ITrainerUploadDocumentService.GetCommonUploadDocsList(CourseType, MasterDoc, MasterDocId).ToList();
           
            return Json(new { result = true, content = list }, JsonRequestBehavior.AllowGet);
        }

        #region Download Trainer Document Folder

        string mastercoursenameother = "";
        public JsonResult DownloadDocumentotherDocumentFolder(int mainid, string type)
        {
            mastercoursenameother = "";
            string path = "";
            Utility.WriteToFile("{0}: SSTMDownloads Service start.");
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
                        var awsModel = new AWSModel()
                        {
                            AccessKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSAccessKey"])),
                            SecreteKey = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["AWSSecretKey"])),
                            BucketName = Utility.StaticDecrypt(Convert.ToString(configAWS.Rows[0]["BucketName"]))
                        };

                        string TrainerDocumentdir = "TrainerDocument";
                        if (Convert.ToInt32(type) == 0)
                            TrainerDocumentdir = "TrainerUploadDocuments";
                        else if (Convert.ToInt32(type) == 1)
                            TrainerDocumentdir = "TrainerUploadvideo";
                        else if (Convert.ToInt32(type) == 3)
                            TrainerDocumentdir = "ConfindentialDocument";
                        else
                            TrainerDocumentdir = "TrainerUploadCommonDoc";

                        awsModel.BucketDirectory = TrainerDocumentdir;

                        objDbLibrary.MasterCourse = true; //Main Course
                        objDbLibrary.MasterCoursId = mainid;//master course
                        objDbLibrary.type = type;//Course Type
                        var dtMainCourses = objDbLibrary.GetDataTable("sstmo.TrainerDocument_Download", objDbLibrary.GrvFilldataCoursewithCourseandSubCourse);
                        Utility.WriteToFile("{0}: SP RUn.");
                        if (dtMainCourses != null && dtMainCourses.Rows.Count > 0)
                        {
                            foreach (DataRow courseRowMain in dtMainCourses.Rows)
                            {
                                try
                                {
                                    System.IO.DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Content/downloadTrainer"));

                                    foreach (FileInfo file in dir.GetFiles())
                                    {
                                        if (file.Extension == ".zip")
                                            file.Delete();
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                //main start
                                var destinationDirMain = Path.Combine(Server.MapPath("~/Content/downloadTrainer/"), Convert.ToString(courseRowMain["DocumentName"]));

                                if (Directory.Exists(destinationDirMain))
                                    Directory.Delete(destinationDirMain, true);

                                mastercoursenameother = Convert.ToString(courseRowMain["DocumentName"]);
                                //if (Directory.Exists(destinationDirMain))
                                //    Directory.Delete(destinationDirMain, true);

                                if (!Directory.Exists(destinationDirMain))
                                    Directory.CreateDirectory(destinationDirMain);

                                //if (Convert.ToBoolean(courseRowMain["isTraining"]))
                                //{
                                Utility.WriteToFile("{0}: SP RUn.");
                                downloadTrainerDocument(destinationDirMain, Convert.ToString(courseRowMain["DocumentName"]), long.Parse(courseRowMain["Id"].ToString()), awsModel,true);
                                //}
                                //main end

                                //Sub course 1 start
                                var objDbLibrarysub1 = new DbLibrary();
                                objDbLibrarysub1.MasterCourse = false; //sub Course
                                objDbLibrarysub1.MasterCoursId = long.Parse(Convert.ToString(courseRowMain["Id"])); //sub Course
                                objDbLibrarysub1.type = type;//Course Type

                                var dtSub1 = objDbLibrarysub1.GetDataTable("sstmo.TrainerDocument_Download", objDbLibrarysub1.GrvFilldataCoursewithCourseandSubCourse);
                                if (dtSub1 != null && dtSub1.Rows.Count > 0)
                                {
                                    foreach (DataRow courseRowsub in dtSub1.Rows)
                                    {
                                        var destinationDirsub1 = Path.Combine(Server.MapPath("~/Content/downloadTrainer/") + courseRowMain["DocumentName"], Convert.ToString(courseRowsub["DocumentName"]));

                                        //if (Directory.Exists(destinationDirsub1))
                                        //    Directory.Delete(destinationDirsub1, true);

                                        if (!Directory.Exists(destinationDirsub1))
                                            Directory.CreateDirectory(destinationDirsub1);
                                        //if (Convert.ToBoolean(courseRowsub["isTraining"]))
                                        //{
                                        downloadTrainerDocument(destinationDirsub1, Convert.ToString(courseRowsub["DocumentName"]), long.Parse(Convert.ToString(courseRowMain["Id"])), awsModel,false);
                                        //}

                                        //start sub 2
                                        var objDbLibrarysub2 = new DbLibrary();
                                        objDbLibrarysub2.MasterCourse = false; //sub Course
                                        objDbLibrarysub2.MasterCoursId = long.Parse(Convert.ToString(courseRowsub["Id"])); //sub Course
                                        objDbLibrarysub2.type = type;//Course Type

                                        var dtSub2 = objDbLibrarysub2.GetDataTable("sstmo.TrainerDocument_Download", objDbLibrarysub2.GrvFilldataCoursewithCourseandSubCourse);
                                        if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                        {
                                            foreach (DataRow courseRowsub2 in dtSub2.Rows)
                                            {
                                                var destinationDirsub2 = Path.Combine(Server.MapPath("~/Content/downloadTrainer/") + courseRowMain["DocumentName"] + "\\" + Convert.ToString(courseRowsub["DocumentName"]), Convert.ToString(courseRowsub2["DocumentName"]));

                                                //if (Directory.Exists(destinationDirsub2))
                                                //    Directory.Delete(destinationDirsub2, true);

                                                if (!Directory.Exists(destinationDirsub2))
                                                    Directory.CreateDirectory(destinationDirsub2);
                                                //if (Convert.ToBoolean(courseRowsub2["isTraining"]))
                                                //{
                                                downloadTrainerDocument(destinationDirsub2, Convert.ToString(courseRowsub2["DocumentName"]), long.Parse(Convert.ToString(courseRowsub["Id"])), awsModel,false);
                                                //}

                                                //start sub folder 3 

                                                var objDbLibrarysub3 = new DbLibrary();
                                                objDbLibrarysub3.MasterCourse = false; //sub Course
                                                objDbLibrarysub3.MasterCoursId = long.Parse(Convert.ToString(courseRowsub2["Id"])); //sub Course
                                                objDbLibrary.type = type;//Course Type

                                                var dtSub3 = objDbLibrarysub3.GetDataTable("sstmo.TrainerDocument_Download", objDbLibrarysub3.GrvFilldataCoursewithCourseandSubCourse);
                                                if (dtSub2 != null && dtSub2.Rows.Count > 0)
                                                {
                                                    foreach (DataRow courseRowsub3 in dtSub3.Rows)
                                                    {
                                                        var destinationDirsub3 = Path.Combine(Server.MapPath("~/Content/downloadCourseFile/") + courseRowMain["DocumentName"] + "\\" + Convert.ToString(courseRowsub["DocumentName"]) + "\\" + Convert.ToString(courseRowsub2["DocumentName"]), Convert.ToString(courseRowsub3["DocumentName"]));

                                                        //if (Directory.Exists(destinationDirsub3))
                                                        //    Directory.Delete(destinationDirsub3, true);

                                                        if (!Directory.Exists(destinationDirsub3))
                                                            Directory.CreateDirectory(destinationDirsub3);
                                                        //if (Convert.ToBoolean(courseRowsub3["isTraining"]))
                                                        //{
                                                        downloadTrainerDocument(destinationDirsub3, Convert.ToString(courseRowsub3["DocumentName"]), long.Parse(courseRowsub2["Id"].ToString()), awsModel,false);
                                                        //}

                                                        //start sub folder 3 


                                                        //end 
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

                path = OtherDownload();
                return Json(new { result = true, message = path });
            }
            catch (Exception ex)
            {
                Utility.WriteToFile("{0}: Exception :: " + ex.Message + ex.StackTrace);
                Utility.WriteToFile("{0}: ************************************************************************");
                return Json(new { result = false, message = "Error" });
            }
            return Json(new { result = true, message = path });

            Utility.WriteToFile("{0}: SSTMDownloads Service end.");
        }

        #endregion

        #region Download File from aws server


        public void downloadTrainerDocument(string destinationDir, string CourseName, long CourseId, AWSModel awsModel,bool ismaster)
        {
            Utility.WriteToFile("{0}: Checking for Course: " + Convert.ToString(CourseName));
            var objDbLibrary = new DbLibrary();
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

         

            var parameters = new SqlParameter[2] { new SqlParameter("@CourseId", Convert.ToInt32(CourseId)), new SqlParameter("@ismaster", ismaster) };
  
            var sharedCoursesDocs = objDbLibrary.GetDataTable("sstmo.TrainerDocumentSharedCourseDocs_download", parameters);
            if (sharedCoursesDocs != null && sharedCoursesDocs.Rows.Count > 0)
            {
                foreach (DataRow sharedCoursesDocRow in sharedCoursesDocs.Rows)
                {
                    try
                    {
                        
                        
                        Utility.WriteToFile("{0}: Checking for Course Document: " + Convert.ToString(sharedCoursesDocRow["DocumentName"]));

                        awsModel.FileName = Convert.ToString(sharedCoursesDocRow["DocumentPath"]);
                        string s = sharedCoursesDocRow["CreatedOn"].ToString().Replace(".", "");
                        string s1 = s.ToString().Replace("-", "");
                        string s3 = s1.ToString().Replace(":", "");
                        string s2 = s3.ToString().Replace(" ", "");

                        string Fname = Convert.ToString(sharedCoursesDocRow["DocumentName"]) + "_" + s2.Replace("/", "").Replace("am", "").Replace("pm", "");

                        var destinationFile = Path.Combine(destinationDir, Fname) + Path.GetExtension(awsModel.FileName);

                        Utility.WriteToFile("{0}: path : " + destinationFile);
                        string[] values = Fname.Split('_');
                        string names = values[0];

                        DirectoryInfo d = new DirectoryInfo(destinationDir);
                        FileInfo[] Files = d.GetFiles("*");

                        Utility.WriteToFile("{0}:Treainer documen: " + Convert.ToString(sharedCoursesDocRow["DocumentName"]));

                       
                        if (!string.IsNullOrEmpty(awsModel.FileName))
                        {
                            string[] getfilename = awsModel.FileName.Split('_');
                            if (CourseName.Replace(" ", "").Trim() == getfilename[0])
                            {
                                awsModel.FilePath = destinationFile;

                                AWS.GetSingleFile(awsModel);
                            }
                        }
                        
                       

                        Utility.WriteToFile("{0}: DocumentName  document updated: " + Convert.ToString(sharedCoursesDocRow["DocumentName"]) + ".");

                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
                Utility.WriteToFile("{0}: No documents found for " + Convert.ToString(CourseName) + ".");

        }
        #endregion
    }
    public class FileModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsSelected { get; set; }
    }
}