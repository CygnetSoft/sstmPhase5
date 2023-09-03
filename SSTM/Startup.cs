using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire.SqlServer;
using Hangfire;
using Hangfire.Dashboard;
using System.Configuration;
using SSTM.Controllers;
using System.Web.Mvc;
using System.Web;
using SSTM.Business.Interfaces;
using Autofac;
using IoC;
using SSTM.Helpers.Common;

[assembly: OwinStartup(typeof(SSTM.Startup))]

namespace SSTM
{
    public class Startup
    {
        string DevloperHostName = ConfigurationManager.AppSettings["DevloperHostName"];
        string DevloperUserID = ConfigurationManager.AppSettings["DevloperUserID"];
        string Devloperpassword = ConfigurationManager.AppSettings["Devloperpassword"];

        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            // ConfigureAuth(app);
            try
            {
                var PMS_DB_Hang = new SqlServerStorage(@"Data Source=" + DevloperHostName + ";Initial Catalog=SSTM_Hangfire; User ID=" + DevloperUserID + "; Password=" + Devloperpassword + ";");
                UtilityHelper helper = new UtilityHelper();
                GlobalConfiguration.Configuration.UseSqlServerStorage(@"Data Source=" + DevloperHostName + ";Initial Catalog=SSTM_Hangfire; User ID=" + DevloperUserID + "; Password=" + Devloperpassword + ";");
                // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });
                //app.UseHangfireServer(additionalProcesses: new[] { new ProcessMonitor(checkInterval: TimeSpan.FromSeconds(1)) });

                app.UseHangfireDashboard("/administration/SSTMHangFire", new DashboardOptions()
                {
                    Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
                }, PMS_DB_Hang);
                //app.UseHangfireDashboard("/administration/SSTMHangFire");
                app.UseHangfireServer();

                //Hangfire.RecurringJob.AddOrUpdate<UserLoginController>(job => job.AutoReminderMail(), cronExpression: "*/5 * * * *");
                //BackgroundJob.Enqueue<UserLoginController>(x => x.CreateRecurringJobs());
                try
                {
                    RecurringJob.AddOrUpdate("Daily_NewCourse", () => helper.expire_newcourse(), Cron.Daily(07, 00));

                }
                catch (Exception ex)
                {
                }

                try
                {
                    RecurringJob.AddOrUpdate("Daily_DeveloperMonitor", () => helper.GetDeveloperMonitorList(), Cron.Daily(19, 10));
                }
                catch (Exception ex)
                {
                }

                try
                {
                    RecurringJob.AddOrUpdate("Daily_CourseDownload", () => helper.DailyCourseDownload(), Cron.Daily(19, 00));
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
        }

        public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                // In case you need an OWIN context, use the next line, `OwinContext` class
                // is the part of the `Microsoft.Owin` package.
                var owinContext = new OwinContext(context.GetOwinEnvironment());

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return owinContext.Authentication.User.Identity.IsAuthenticated;
            }
        }

    }
}
