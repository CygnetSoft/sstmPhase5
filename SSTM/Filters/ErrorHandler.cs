using System;
using System.Web;
using System.Web.Mvc;

namespace SSTM.Filters
{
    public class ErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //Exception ex = filterContext.Exception;
            //filterContext.ExceptionHandled = true;

            //var model = new HandleErrorInfo(filterContext.Exception, (string)filterContext.RouteData.Values["controller"],
            //    (string)filterContext.RouteData.Values["action"]);

            //filterContext.Result = new ViewResult()
            //{
            //    ViewName = "InternalServerError",
            //    ViewData = new ViewDataDictionary(model)
            //};

            string controllerName = "", actionName = "";

            HandleErrorInfo model = null;

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
                return;

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
                return;

            // if the request is AJAX return JSON else view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                controllerName = (string)filterContext.RouteData.Values["controller"];
                actionName = (string)filterContext.RouteData.Values["action"];
                model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary(model),
                    TempData = filterContext.Controller.TempData
                };
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}