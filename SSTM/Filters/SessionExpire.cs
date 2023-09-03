using SSTM.Helpers.App;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SSTM.Filters
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = HttpContext.Current;

            // check if session is supported
            if (context.Session != null)
            {
                // check if a new session id was generated
                if (context.Session.IsNewSession)
                {
                    // If it says it is a new session, but an existing cookie exists, then it must
                    // have timed out
                    var sessionCookie = context.Request.Headers["Cookie"];
                    if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId", System.StringComparison.Ordinal) >= 0))
                    {
                        //var redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                        if (filterContext.HttpContext.Request.Url != null)
                        {
                            var redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                            var redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                            var loginUrl = FormsAuthentication.LoginUrl + redirectUrl;

                            var area = filterContext.RouteData.DataTokens["area"];
                            if ((string)area == "Administration")
                                loginUrl = "~/Administration/Login" + redirectUrl;
                            else
                                loginUrl = "~/UserLogin" + redirectUrl;

                            if (context.Request.IsAuthenticated)
                                FormsAuthentication.SignOut();

                            var rr = new RedirectResult(loginUrl);
                            filterContext.Result = rr;
                        }
                    }
                }
                else if (HttpContext.Current.Session["AppSession"] == null)
                {
                    var redirectOnSuccess = filterContext.HttpContext.Request.Url.PathAndQuery;
                    var redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                    var loginUrl = FormsAuthentication.LoginUrl + redirectUrl;

                    var area = filterContext.RouteData.DataTokens["area"];
                    if ((string)area == "Administration")
                        loginUrl = "~/Administration/Login" + redirectUrl;
                    else
                        loginUrl = "~/UserLogin" + redirectUrl;

                    if (context.Request.IsAuthenticated)
                        FormsAuthentication.SignOut();

                    var rr = new RedirectResult(loginUrl);
                    filterContext.Result = rr;
                }
                else
                {
                    var currentSession = (AppSession)HttpContext.Current.Session["AppSession"];
                    if (!currentSession.isLocationVerified)
                    {
                        if (context.Request.IsAuthenticated)
                            FormsAuthentication.SignOut();

                        var rr = new RedirectResult("~/Error/AccessForbidden");
                        filterContext.Result = rr;
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

                        if (context.Request.IsAuthenticated)
                            FormsAuthentication.SignOut();

                        var rr = new RedirectResult(loginUrl);
                        filterContext.Result = rr;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}