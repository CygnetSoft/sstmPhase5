using Quartz;
using RestSharp;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
//http://www.cronmaker.com/;jsessionid=node0nv7eync8guoj15dwim23d74i43203017.node0?0
namespace SSTM.BackgroundWorker
{
    public class ExecuteTaskServiceCallJob : IJob
    {
        public static readonly string SchedulingStatus = ConfigurationManager.AppSettings["ExecuteTaskServiceCallSchedulingStatus"];
        public  Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (SchedulingStatus.Equals("ON"))
                {
                    try
                    {
                        //HttpClient client = new HttpClient();
                        string Baseurl = "localhost:53913";
                        //HttpResponseMessage response = client.PostAsync(,null).Result;
                        var request = new RestRequest(Method.POST);
                        request.Timeout = -1;
                        request.ReadWriteTimeout = -1;
                        request.AlwaysMultipartFormData = true;

                        var client = new RestClient(Baseurl + "/QPAPI/AutoReminderMail");
                        client.Timeout = -1;
                        client.ReadWriteTimeout = -1;
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        IRestResponse response11 = client.Execute(request);

                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
            return task;
        }
    }
}