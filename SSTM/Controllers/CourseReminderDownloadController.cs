using OpenHtmlToPdf;
using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.Course_Reminder;
using SSTM.Models.CourseReminderDownload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class CourseReminderDownloadController : Controller
    {
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        private readonly ICourseReminderService _ICourseReminderService;
        private readonly ICourseRenewalService _ICourseRenewalService;
        private readonly ICourseReminderLatterUndertakingService _ICourseReminderLatterUndertakingService;
        private readonly ICourseReminderTrackerService _ICourseReminderTrackerService;
        public CourseReminderDownloadController
                  (IConfigService configService,
            IUserService userService, ICourseReminderService ICourseReminderService,
                   ICourseRenewalService ICourseRenewalService, ICourseReminderLatterUndertakingService ICourseReminderLatterUndertakingService,
                   ICourseReminderTrackerService ICourseReminderTrackerService)
        {
            _IConfigService = configService;
            _IUserService = userService;
            _ICourseReminderService = ICourseReminderService;

            _ICourseRenewalService = ICourseRenewalService;
            _ICourseReminderLatterUndertakingService = ICourseReminderLatterUndertakingService;
            _ICourseReminderTrackerService = ICourseReminderTrackerService;
        }

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        // GET: CourseReminderDownload
        public ActionResult Letterofundertaking(long id)
        {

            var configEntity = _IConfigService.GetFirstRecord();
            var model = _ICourseReminderService.GetRecordById(id);

            model.latter_signature = !string.IsNullOrEmpty(model.latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.latter_signature : "";
            model.director_latter_signature = !string.IsNullOrEmpty(model.director_latter_signature) ? "https://" + UtilityHelper.Decrypt(configEntity.BucketName) + ".s3.ap-southeast-1.amazonaws.com/NewCourseReminder/Latter/" + model.director_latter_signature : "";
            var entity = _ICourseReminderLatterUndertakingService.GetFirstRecord();
            var userEntity = _IUserService.GetRecordById(model.DeveloperId);
            //StringBuilder latter_data =new StringBuilder();
            var latter_data = entity.latter_content.ToString();
            if (userEntity != null)
            {
                try
                {
                    var Li_trainer = Li_trainer_detail(userEntity.Trainer_AirLine_id);
                    latter_data = latter_data.Replace("@@name@@", userEntity.FirstName + " " + userEntity.LastName).Replace("@@icno@@", Li_trainer.fin);
                }
                catch (Exception)
                {
                    latter_data = latter_data.Replace("@@name@@", "Developer not assing Li Id").Replace("@@icno@@", "0");
                }
            }
            ViewBag.newcourseid = id;
            ViewBag.latter = latter_data;
            return View(model);
        }
        public LITrainerModel Li_trainer_detail(long? trainerid)
        {
            LITrainerModel LitrinModel = new LITrainerModel();
            CourseService.SSTM service = new SSTM.CourseService.SSTM();
            string data = service.AllTrainer();

            List<LITrainerModel> Li_trainer_list = (new JavaScriptSerializer()).Deserialize<List<LITrainerModel>>(data);
            LitrinModel = Li_trainer_list.Where(a => a.TrainerID == trainerid).FirstOrDefault();

            return LitrinModel;
        }
        public ActionResult LetterofundertakingList()
        {
            var model = _ICourseReminderService.GetAllRecord();
            return View(model);
        }

        [ValidateInput(false)] // <-- not able to hit the action method without this
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GeneratePdf(string element, string singnature)
        {
            StringBuilder sdata = new StringBuilder();

            string dir = Server.MapPath("~/Content/latter_of_undertaking/");
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                else
                {
                    string[] files = Directory.GetFiles(dir);

                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        fi.Delete();
                    }
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception)
            {
                Directory.CreateDirectory(dir);
            }

            Random _random = new Random();
            element += singnature;
            string filepath = "";
            try
            {
                var randomNumber1 = _random.Next(1, 1000);
                var pdf = Pdf
                     .From(element)
                     .OfSize(PaperSize.A4)
                     .WithTitle("Title")
                     .WithoutOutline()
                     .WithMargins(2.Centimeters())
                     .Portrait()
                     .Comressed()
                     .Content();

                byte[] content = pdf;
                System.IO.File.WriteAllBytes(Server.MapPath("~") + "\\Content\\latter_of_undertaking\\" + randomNumber1 + "latter_of_undertaking.pdf", content);
                filepath = randomNumber1 + "latter_of_undertaking.pdf";
                //end
            }
            catch (Exception ex)
            {
            }
            return Json(new { result = true, FileName = filepath.ToString() });

        }
        public FileResult Download(string filepath)
        {

            filepath = filepath.Trim('"');
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~") + "\\Content\\latter_of_undertaking\\" + filepath);
            string fileName = "latter_of_undertaking.pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

        public ActionResult AebApproved()
        {
            CourseService.SSTM service = new CourseService.SSTM();
            List<AebcourseApproved> TodayClass_data = new List<AebcourseApproved>();

            string data = service.AEBAPPROVED();
            List<AebcourseApproved> model = (new JavaScriptSerializer()).Deserialize<List<AebcourseApproved>>(data);
            var remindermodel = _ICourseReminderService.GetAllRecord();
            var LicourseId = remindermodel.Select(x => x.li_course_id);

            TodayClass_data = model.Where(x => LicourseId.Contains(x.courseid)).ToList();

            return View(TodayClass_data);
        }

        public FileResult AebDownload(int id)
        {
            CourseService.SSTM service = new CourseService.SSTM();
            string data = service.AEBDocument(id);
            List<Aebpdf> model = (new JavaScriptSerializer()).Deserialize<List<Aebpdf>>(data);

            byte[] byteInfo = Convert.FromBase64String(model[0].pdf);
            return File(byteInfo, System.Net.Mime.MediaTypeNames.Application.Pdf, "AebApproval.pdf");

        }
        public ActionResult CourseProposalDownload()
        {
            CourseService.SSTM service = new CourseService.SSTM();
            List<CourseProposal> TodayClass_data = new List<CourseProposal>();

            string data = service.CourseProposal();
            List<CourseProposal> model = (new JavaScriptSerializer()).Deserialize<List<CourseProposal>>(data);
            var remindermodel = _ICourseReminderService.GetAllRecord();
            var LicourseId = remindermodel.Select(x => x.li_course_id);

            TodayClass_data = model.Where(x => LicourseId.Contains(x.courseid)).ToList();

            return View(TodayClass_data);
        }
        public ActionResult NeedAnalysisDownload()
        {
            CourseService.SSTM service = new CourseService.SSTM();
            List<CourseProposal> TodayClass_data = new List<CourseProposal>();

            string data = service.NeedAnalysis();
            List<CourseProposal> model = (new JavaScriptSerializer()).Deserialize<List<CourseProposal>>(data);
            var remindermodel = _ICourseReminderService.GetAllRecord();
            var LicourseId = remindermodel.Select(x => x.li_course_id);

            TodayClass_data = model.Where(x => LicourseId.Contains(x.courseid)).ToList();

            return View(TodayClass_data);
        }
        public ActionResult CourseReviewlist()
        {
            CourseService.SSTM service = new CourseService.SSTM();

            string data = service.GetDCR();
            List<GetDCRModel> model = (new JavaScriptSerializer()).Deserialize<List<GetDCRModel>>(data);
            return View(model);
        }
        public FileResult CourseReviewDownload(int id)
        {
            CourseService.SSTM service = new CourseService.SSTM();
            string data = service.DCRDocument(id);
            List<Aebpdf> model = (new JavaScriptSerializer()).Deserialize<List<Aebpdf>>(data);

            byte[] byteInfo = Convert.FromBase64String(model[0].pdf);
            return File(byteInfo, System.Net.Mime.MediaTypeNames.Application.Pdf, "CourseReview.pdf");

        }
        public ActionResult DesignChangeRequestlist()
        {
            CourseService.SSTM service = new CourseService.SSTM();
            string data = service.DesignValidation();
            List<DesignValidationModel> model = (new JavaScriptSerializer()).Deserialize<List<DesignValidationModel>>(data);
            return View(model);
        }
        public FileResult DesignChangeRequestDownload(int id,string title)
        {
            CourseService.SSTM service = new CourseService.SSTM();
            string data = service.Designvalidationform(id, title);
            List<Aebpdf> model = (new JavaScriptSerializer()).Deserialize<List<Aebpdf>>(data);

            byte[] byteInfo = Convert.FromBase64String(model[0].pdf);
            return File(byteInfo, System.Net.Mime.MediaTypeNames.Application.Pdf, "DesignChangeRequest.pdf");

        }
    }
}

public class Aebpdf
{
    public string pdf { get; set; }

}


