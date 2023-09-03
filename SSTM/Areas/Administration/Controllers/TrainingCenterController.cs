using SSTM.Business.Interfaces;
using SSTM.Controllers;
using SSTM.Filters;
using SSTM.Helpers.App;
using SSTM.Helpers.AutoMapping;
using SSTM.Helpers.Common;
using SSTM.Models.JQueryDataTablesModel;
using SSTM.Models.TrainingCenter;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SSTM.Areas.Administration.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class TrainingCenterController : BaseController
    {
        #region Class Properties Declarations
        private readonly IExceptionLogService _IExceptionLogService;
        private readonly ITrainingCenterService _ITrainingCenterService;

        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        #region Class Properties Definitions
        public TrainingCenterController(IExceptionLogService exceptionLogService, ITrainingCenterService trainingCenterService)
        {
            _IExceptionLogService = exceptionLogService;
            _ITrainingCenterService = trainingCenterService;
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetTrainingCenterDataGridJsonResult(JQueryDataTablesModel jQueryDataTablesModel, int isActive)
        {
            int totalRecordCount;
            int searchRecordCount;

            var records =
                _ITrainingCenterService.GetTrainingCentersGrid
                    (startIndex: jQueryDataTablesModel.iDisplayStart, pageSize: jQueryDataTablesModel.iDisplayLength,
                    sortedColumns: jQueryDataTablesModel.GetSortedColumns(), totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount,
                    searchString: jQueryDataTablesModel.sSearch, isActive: isActive);

            return this.DataTablesJson
                (items: records, totalRecords: totalRecordCount, totalDisplayRecords: searchRecordCount, sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpGet]
        public ActionResult GetTrainingCentersList(int isActive)
        {
            try
            {
                var sBuilder = new StringBuilder();

                var list = _ITrainingCenterService.GetList(isActive).ToList();

                foreach (var item in list)
                {
                    var status = item.isActive ?
                        "<label class='badge badge-success'>Active</label>" : "<label class='badge badge-warning'>Inactive</label>";

                    var actions =
                        "<button type='button' title='Edit' class='btn btn-primary btn-sm btnEditTC'><i class='fa fa-pen'></i></button>&nbsp;" +
                        "<button type='button' title='Delete' class='btn btn-danger btn-sm btnDeleteTC'><i class='fa fa-trash'></i></button>";

                    sBuilder.Append(
                        "<tr id='" + item.Id + "'>" +
                            "<td>" + item.Name + "</td>" +
                            "<td>" + item.AddressLine1 + " " + item.AddressLine2 + "</td>" +
                            "<td>" + item.PostalCode + "</td>" +
                            "<td class='text-center'>" + status + "</td>" +
                            "<td class='text-center'>" + actions + "</td>" +
                        "</tr>");
                }

                return Json(new { result = true, content = sBuilder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainingCenter", "GetTrainingCentersList", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetTrainingCenterById(long Id)
        {
            try
            {
                var model = _ITrainingCenterService.GetRecordById(Id).ToModel();

                if (model == null)
                    model = new TrainingCenterModel();
                else
                    model.NetworkIP = UtilityHelper.Decrypt(model.NetworkIP);

                return PartialView("_AddOrUpdate", model);
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainingCenter", "GetTrainingCenterById", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Content("<div class='alert alert-danger'>" + AppMessages.Exception + "</div>");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveTrainingCenter(TrainingCenterModel model)
        {
            try
            {
                if (!UtilityHelper.ValidateIPv4(model.NetworkIP))
                    ModelState.AddModelError("InvalidIP", new Exception("Please insert valid Network IP."));

                var entity = _ITrainingCenterService.GetRecordById(model.Id).ToModel();
                if (entity != null)
                {
                    entity.UpdatedBy = CurrentSession.UserId;
                    entity.UpdatedOn = DateTime.Now;
                }
                else
                {
                    entity = new TrainingCenterModel();
                    model.CreatedBy = CurrentSession.UserId;
                    model.CreatedOn = DateTime.Now;
                }

                entity.Name = model.Name;
                entity.AddressLine1 = model.AddressLine1;
                entity.AddressLine2 = model.AddressLine2;
                entity.PostalCode = model.PostalCode;
                entity.NetworkIP = UtilityHelper.Encrypt(model.NetworkIP);
                entity.isActive = model.isActive;

                _ITrainingCenterService.SaveRecord(entity.ToEntity());

                return Json(new { result = true });
            }
            catch (System.Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainingCenter", "SaveTrainingCenter", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }

        [HttpPost]
        public ActionResult DeleteTrainingCenter(long Id)
        {
            try
            {
                _ITrainingCenterService.DeleteRecord(Id);
                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                _IExceptionLogService.SaveRecord(ex, "TrainingCenter", "DeleteTrainingCenter", Request.Url.AbsoluteUri, CurrentSession.UserId);
                return Json(new { result = false, message = AppMessages.Exception });
            }
        }
    }
}