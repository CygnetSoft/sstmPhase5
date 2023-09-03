using System.Web.Mvc;

namespace SSTM.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult AccessForbidden()
        {
            return View();
        }

        public ActionResult RedirectoToPreviousLink(string PrevURL)
        {
            if (PrevURL != null)
                return Redirect(PrevURL);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}