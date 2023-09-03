using GleamTech.DocumentUltimate;
using GleamTech.DocumentUltimate.AspNet;
using SSTM.Helpers.AutoMapping;
using SSTM.Models.JQueryDataTablesModel;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using GleamTech;
using GleamTech.AspNet;

namespace SSTM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTkxNjMyOUAzMjMxMmUzMjJlMzNaWTh2MlZyTlYxZTRqQmRTdFVZOGg0cU4rQjdBTkFybjloVDF4RmpPR044PQ==");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        
            //AutoMapper is an object-object mapper which allows you to solve issues with mapping 
            // the same properties from one object of one type to another object of another type.
            AutoMapperStartupTask.Execute();

            //We registerd IOC Config File 
            IoCConfig.RegisterDependencies();

            //configure Log File 
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));

            ModelBinders.Binders.Add(typeof(JQueryDataTablesModel), new JQueryDataTablesModelBinder());

             DocumentUltimateConfiguration.Current.LicenseKey = "QTCJAL6K96-CDXJUUFGMF-98XAB2LQ1J-CJSF9A1CMJ-NHAPC27QXK-2FU3HUS68K-E7NJSS69CX-JFVA6JCNTF-RA2JCNNS5V-JULNNJS68J-Q";

            var gleamTechConfig = Hosting.ResolvePhysicalPath("~/App_Data/GleamTech.config");
            if (File.Exists(gleamTechConfig))
                GleamTechConfiguration.Current.Load(gleamTechConfig);
         

            DocumentUltimateWebConfiguration.Current.CacheLocation = "~/App_Data/DocumentCache";
           
        }
        public class AllowCrossSiteAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "http://localhost:53913");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "*");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");

                base.OnActionExecuting(filterContext);
            }
        }
        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exception = Server.GetLastError();
        //    Server.ClearError();
        //    Response.Redirect("/Error/InternalServerError");
        //}
    }
}