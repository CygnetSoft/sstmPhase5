using SSTM.Helpers.App;
using System.Web.Mvc;
using System.Web.Security;

namespace SSTM.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["AppSession"] == null)
            {
                var redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                var redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                var loginUrl = FormsAuthentication.LoginUrl + redirectUrl;

                var area = filterContext.RouteData.DataTokens["area"];
                if ((string)area == "Administration")
                    loginUrl = "~/Administration/Login" + redirectUrl;
                else
                    loginUrl = "~/UserLogin" + redirectUrl;

                if (Request.IsAuthenticated)
                    FormsAuthentication.SignOut();

                filterContext.Result = new RedirectResult(loginUrl);
            }
            else
            {
                var currentSession = (AppSession)Session["AppSession"];
                if (!currentSession.isLocationVerified)
                {
                    if (Request.IsAuthenticated)
                        FormsAuthentication.SignOut();

                    filterContext.Result = new RedirectResult("~/Error/AccessForbidden");
                }
                else if (!currentSession.isOTPVerified)
                {
                    var redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                    var redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                    var loginUrl = FormsAuthentication.LoginUrl + redirectUrl;

                    var area = filterContext.RouteData.DataTokens["area"];
                    if ((string)area == "Administration")
                        loginUrl = "~/Administration/Login" + redirectUrl;
                    else
                        loginUrl = "~/UserLogin" + redirectUrl;

                    if (Request.IsAuthenticated)
                        FormsAuthentication.SignOut();

                    var rr = new RedirectResult(loginUrl);
                    filterContext.Result = rr;
                }


                if (filterContext.HttpContext.Session != null)
                {
                    if (filterContext.HttpContext.Session.IsNewSession)
                    {
                        string cookie = filterContext.HttpContext.Request.Headers["Cookie"];
                        if ((cookie != null) && (cookie.IndexOf("ASP.NET_SessionId") >= 0))
                        {
                            var redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                            var redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                            var loginUrl = FormsAuthentication.LoginUrl + redirectUrl;

                            var area = filterContext.RouteData.DataTokens["area"];
                            if ((string)area == "Administration")
                                loginUrl = "~/Administration/Login" + redirectUrl;
                            else
                                loginUrl = "~/UserLogin" + redirectUrl;
                            filterContext.Result = new RedirectResult(loginUrl);

                            return;

                        }

                    }

                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
