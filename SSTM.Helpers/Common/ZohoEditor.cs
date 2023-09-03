using RestSharp;
using System;
using System.Net;

namespace SSTM.Helpers.Common
{
    public class ZohoEditor
    {
        public static IRestResponse GetZohoEditor
            (string filePath, string Extension, string filename, string courseId, long docId, string docName, string UserId,string zohoApikey,string awsurl) // add new parameter awsurl 28-06-2021
        {
            
            try
            {
                var client = new RestClient();
                client.Timeout = -1;
                client.ReadWriteTimeout = -1;
                var request = new RestRequest(Method.POST);
                request.Timeout = -1;
                request.ReadWriteTimeout = -1;
                request.AlwaysMultipartFormData = true;

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //int document_id = Convert.ToInt32(System.IO.Path.GetFileNameWithoutExtension(filename).ToString().Substring(System.IO.Path.GetFileNameWithoutExtension(filename).Length - 4));

              
                if (Extension == ".doc" || Extension == ".docx")
                {
                    string url = UtilityHelper.SiteUrl + "home/PPTSaveDocument/?cId=" + courseId + "&dId=" + docId.ToString() + "&ext=" + Extension + "&UserId=" + UserId;

                    client = new RestClient("https://writer.zoho.in/writer/officeapi/v1/document");

                    request.AddParameter("apikey", zohoApikey);
                    request.AddParameter("callback_settings", "{'save_format':'" + Extension.Replace(".", "").Trim() + "','context_info':'" + filename + "','save_url':'" + url + "'}");

                    //request.AddFile("document", filePath);//old
                    request.AddParameter("url", awsurl);
                    request.AddParameter("editor_settings", "{'unit':'in','language':'en','view':'pageview'}");
                    request.AddParameter("permissions", "{'document.export':false,'document.print':false,'document.edit':true}");
                    request.AddParameter("document_info", "{'document_name':'" + docName + "', 'document_id':" + UtilityHelper.GenerateSixDigitOTP()+ "}");
                   
                }
                else if (Extension == ".ppt" || Extension == ".pptx")
                {
                    string url = UtilityHelper.SiteUrl + "home/PPTSaveDocument/?cId=" + courseId + "&dId=" + docId.ToString() + "&ext=" + Extension + "&UserId=" + UserId;

                    //request.AddFile("document", filePath); //old
                   
                    var client1 = new RestClient("https://show.zoho.in/show/officeapi/v1/presentation?apikey="+ zohoApikey);
                    client1.Timeout = -1;
                    client.ReadWriteTimeout = -1;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    request.AlwaysMultipartFormData = true;
                    var request1 = new RestRequest(Method.POST);
                     
                    request1.Timeout = -1;
                    request1.ReadWriteTimeout = -1;
                   // request.AddFile("document", filePath);//old
                    request1.AddParameter("url", awsurl);
                    request1.AddParameter("editor_settings", "{ 'language': 'en' }");
                    request1.AddParameter("permissions", "{ 'document.export': false, 'document.print': false, 'document.edit': true }");
                    request1.AddParameter("callback_settings", "{'save_format':'pptx','context_info':"+ filename + ",'save_url':'" + url + "'}");
                    request1.AddParameter("document_info", "{'document_name':'" + docName + "', 'document_id':"+ UtilityHelper.GenerateSixDigitOTP() + "}");
                    IRestResponse response11 = client1.Execute(request1);
                    return response11;
                }
                else if (Extension == ".xlsx" || Extension == ".xls")
                {
                    string url = UtilityHelper.SiteUrl + "home/PPTSaveDocument/?cId=" + courseId + "&dId=" + docId.ToString() + "&ext=" + Extension + "&UserId=" + UserId;
                    client = new RestClient("https://sheet.zoho.in/sheet/officeapi/v1/spreadsheet");
                    request.AddParameter("apikey", zohoApikey);
                    request.AddParameter("callback_settings", "{'save_format':'" + Extension.Replace(".", "").Trim() + "','context_info':'" + filename + "','save_url':'" + url + "'}");
                  //  request.AddFile("document", filePath);//old

                    request.AddParameter("url", awsurl);//new
                    request.AddParameter("editor_settings", "{'language':'en','country':'IN'}");
                    request.AddParameter("permissions", "{'document.export':false,'document.print':false,'document.edit':true}");
                    request.AddParameter("document_info", "{'document_name':'" + docName + "', 'document_id':" + UtilityHelper.GenerateSixDigitOTP() + "}");
                }

                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}