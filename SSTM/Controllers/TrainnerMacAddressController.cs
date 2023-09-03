using SSTM.Business.Interfaces;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.TrainnerMacAddress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class TrainnerMacAddressController : Controller
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly IActivityLogService _IActivityLogService;
        private readonly IConfigService _IConfigService;
        private readonly IUserService _IUserService;
        private readonly IRoleService _IRoleService;
        private readonly ITrainnerMacAddressService _ITrainnerMacAddressService;
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion
        public TrainnerMacAddressController
          (IExceptionLogService exceptionLogService, IActivityLogService activityLogService, IConfigService configService,
          IUserService userService, IRoleService roleService, ITrainnerMacAddressService TrainnerMacAddressService)
        {
            _IExceptionLogService = exceptionLogService;
            _IActivityLogService = activityLogService;
            _IConfigService = configService;
            _IUserService = userService;
            _IRoleService = roleService;
            _ITrainnerMacAddressService = TrainnerMacAddressService;
        }

        // GET: TrainnerMacAddress
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MacAddressList()
        {
            var sBuilder = new StringBuilder();
            var list = _ITrainnerMacAddressService.GetAllMacAddress();
            foreach (var item in list)
            {
                sBuilder.Append(
                     "<tr id='" + item.Id + "'>" +
                         "<td>" + UtilityHelper.Decrypt(item.MacAddress) + "</td>" +
                          "<td class='text-center'>" +
                                "<a href='javascript: void(0)' id='btndelete' class='btn btn-danger btn-sm delete' onclick='DeleteMacAddress("+ item.Id+ ")'> Delete</a>" +
                            "</td>" +
                          "</tr>"
                         );
            }
            return Json(new { result = true, listdata = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveMacAddress(TrainnerMacAddressModel model)
        {
            try
            {
                var macaddr = new TrainnerMacAddressModel().ToEntity();
                macaddr.MacAddress = UtilityHelper.Encrypt(model.MacAddress);
                _ITrainnerMacAddressService.SaveRecord(macaddr);
                return Json(new { result = true, message = "Save Mac Address" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainnerMacAddress", "SaveMacAddress", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = "Error in Save Mac Address" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteMacAddress(long id)
        {
            try
            {
                _ITrainnerMacAddressService.DeleteRecord(id);
                return Json(new { result = true, message = "Successfull delete mac Address" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainnerMacAddress", "DeleteMacAddress", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }

        }
    }
}