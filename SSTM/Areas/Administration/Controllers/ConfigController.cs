using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.Config;
using System;
using System.Web.Mvc;

namespace SSTM.Areas.Administration.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class ConfigController : Controller
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IConfigService _IConfigService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public ConfigController(IExceptionLogService exceptionLogService, IConfigService configService)
        {
            _IExceptionLogService = exceptionLogService;
            _IConfigService = configService;
        }
        #endregion

        public ActionResult Index()
        {
            var model = _IConfigService.GetFirstRecord().ToModel();
            if (model != null)
            {
                model.AWSProfileName = UtilityHelper.Decrypt(model.AWSProfileName);
                model.AWSAccessKey = UtilityHelper.Decrypt(model.AWSAccessKey);
                model.AWSSecretKey = UtilityHelper.Decrypt(model.AWSSecretKey);
                model.BucketName = UtilityHelper.Decrypt(model.BucketName);

                model.Email = UtilityHelper.Decrypt(model.Email);
                model.Pass = UtilityHelper.Decrypt(model.Pass);
                model.Port = UtilityHelper.Decrypt(model.Port);
                model.Host = UtilityHelper.Decrypt(model.Host);
                model.ZohoApiKey = UtilityHelper.Decrypt(model.ZohoApiKey);
                model.EnableSsl = UtilityHelper.Decrypt(model.EnableSsl);
            }
            else
                model = new ConfigModel();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveRecord(ConfigModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = _IConfigService.GetRecordById(model.Id);

                    if (entity == null)
                    {
                        entity = new ConfigModel().ToEntity();
                        model.EnableSsl = UtilityHelper.Encrypt("true");
                        model.CreatedBy = CurrentSession.UserId;
                        model.CreatedOn = DateTime.Now;
                    }

                    entity.AWSProfileName = UtilityHelper.Encrypt(model.AWSProfileName);
                    entity.AWSAccessKey = UtilityHelper.Encrypt(model.AWSAccessKey);
                    entity.AWSSecretKey = UtilityHelper.Encrypt(model.AWSSecretKey);
                    entity.BucketName = UtilityHelper.Encrypt(model.BucketName);
                    entity.ZohoApiKey = UtilityHelper.Encrypt(model.ZohoApiKey);

                    entity.Email = UtilityHelper.Encrypt(model.Email);
                    entity.Pass = UtilityHelper.Encrypt(model.Pass);
                    entity.Port = UtilityHelper.Encrypt(model.Port);
                    entity.Host = UtilityHelper.Encrypt(model.Host);

                    _IConfigService.SaveRecord(entity);

                    return Json(new { result = true });
                }
                else
                    return Json(new { result = false, message = "Please complete all the required fields." });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "Config", "SaveRecord", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
    }
}