using SSTM.Controllers;
using SSTM.Filters;
using SSTM.Helpers.App;
using System.Web.Mvc;

namespace SSTM.Areas.Administration.Controllers
{
    [Authorize, SessionExpire, ErrorHandler]
    public class AdminHomeController : BaseController
    {
        #region Class Properties Declarations
        public AppSession CurrentSession
        {
            set { Session["AppSession"] = value; }
            get { return (Session["AppSession"] as AppSession); }
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetHeaderNavbar()
        {
            return PartialView("_HeaderNavbar", CurrentSession);
        }

        public ActionResult GetMenuSidebar()
        {
            return PartialView("_MenuSidebar", CurrentSession);
        }
    }
}