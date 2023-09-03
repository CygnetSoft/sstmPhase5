using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SSTM.Helpers.Common
{
    public class UtilityHelper
    {
        public static readonly string SiteUrl = ConfigurationManager.AppSettings["LiveSiteUrl"].ToString();
        public static readonly string amazonlinkUrl = ConfigurationManager.AppSettings["amazonlink"].ToString();
        public static readonly string testamazonlinkUrl = ConfigurationManager.AppSettings["testamazonlink"].ToString();
        public static readonly string localUrl = ConfigurationManager.AppSettings["LocalUrl"].ToString();

        public static readonly string zohoApikey = "ad2c03c053a6a8acfba4ef58f3bd17be";

        public static long CurrentUserloginId { get; set; }
        public static long CurrentCourseId { get; set; }
        public static long CurrentbatchId { get; set; }

        public static string Encrypt(string content)
        {
            return SecurityMethods.StaticEncrypt(content);
        }

        public static string Decrypt(string content)
        {
            return SecurityMethods.StaticDecrypt(content);
        }

        public static string GenerateSixDigitOTP()
        {
            string strrandom = string.Empty;
            string numbers = "0123456789";

            Random objrandom = new Random();
            for (int i = 0; i < 6; i++)
            {
                int temp = objrandom.Next(0, numbers.Length);
                strrandom += temp;
            }

            return strrandom;
        }

        public static DateTime GetCurrentSGTDate()
        {
            TimeZoneInfo sgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, sgTimeZone);
        }

        public static DateTime ConvertToSGTDate(string stringDate)
        {
            DateTime date = DateTime.ParseExact(stringDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            TimeZoneInfo sgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local, sgTimeZone);
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public static string GetIPAddress()
        {
            if (HttpContext.Current.Request.IsLocal)
                return ConfigurationManager.AppSettings["DebuggingIP"].ToString();
            else
                return HttpContext.Current.Request.UserHostAddress.ToString();

            //string UserIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(UserIP))
            //{
            //    UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
        }

        public static StringBuilder GetEmailTemplate(string templateName)
        {
            StringBuilder emailBodyBuilder = new StringBuilder();

            string templatePath = Path.Combine(HttpContext.Current.Server.MapPath("~/EmailTemplates"), templateName);
            using (StreamReader streamReader = new StreamReader(templatePath))
            {
                emailBodyBuilder.Append(streamReader.ReadToEnd());
            }

            return emailBodyBuilder;
        }

        public static string GetMimeTypeFromExtension(string ext)
        {
            if (MIMETypesDictionary.ContainsKey(ext))
            {
                return MIMETypesDictionary[ext];
            }
            return "unknown/unknown";
        }

        private static IDictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".doc", "application/msword"},
            {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            { ".ppt", "application/vnd.ms-powerpoint"},
            {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
            {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            { ".xls", "application/vnd.ms-excel"},
            {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
            {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            { ".pdf", "application/pdf"}
        };

        public static string JsonSstmEncrypt(string content)
        {
            return SecurityMethods.JsonSstmEncrypt(content);
        }
        public static string JsonSstmDecrypt(string content)
        {
            return SecurityMethods.JsonSstmDecrypt(content);
        }
        public void expire_newcourse()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["LiveSiteUrl"].ToString();

                var client = new RestClient(url + "/QPAPI/AutoReminderMail");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                throw;
            }
        }


        public void CourseDocumentbackup()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["LiveSiteUrl"].ToString();

                var client = new RestClient(url + "/QPAPI/DriveBackupCourse");
                client.Timeout = -1;
                client.ReadWriteTimeout = -1;

                var request = new RestRequest(Method.POST);
                request.Timeout = -1;
                request.ReadWriteTimeout = -1;
                request.AlwaysMultipartFormData = true;
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                throw;
            }
        }

        public void GetDeveloperMonitorList()
        {
            try
            {
               //string url = "http://localhost:53913/";
                string url =  ConfigurationManager.AppSettings["LiveSiteUrl"].ToString();

                var client = new RestClient(url + "QPAPI/GetDeveloperMonitorList");
                client.Timeout = -1;
                client.ReadWriteTimeout = -1;

                var request = new RestRequest(Method.POST);
                request.Timeout = -1;
                request.ReadWriteTimeout = -1;
                request.AlwaysMultipartFormData = true;

                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                throw;
            }
        }

        public void DailyCourseDownload()
        {
            try
            {
                 //string url = "http://localhost:53913/";
                string url = ConfigurationManager.AppSettings["LiveSiteUrl"].ToString();

                var client = new RestClient(url + "QPAPI/DriveBackupCourse");
                client.Timeout = -1;
                client.ReadWriteTimeout = -1;

                var request = new RestRequest(Method.POST);
                request.Timeout = -1;
                request.ReadWriteTimeout = -1;
                request.AlwaysMultipartFormData = true;

                IRestResponse response = client.Execute(request);

                Console.WriteLine(response.Content);
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                throw;
            }
        }

    }
}