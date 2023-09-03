using SSTM.Business.Interfaces;
using SSTM.Core.IntroPage;
using SSTM.Data.Infrastructure;
using SSTM.Helpers.Common;
using SSTM.Models.IntroPage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SSTM.Business.Services;

namespace Business.Services
{
    public class CreateIntropageService : RepositoryBase<StudentIntroPage>, ICreateIntropageService
    {
        private readonly string ApiBaseUrl = ConfigurationManager.AppSettings["li_ApiServices"];
        private readonly string Exist_ApiBaseUrl = ConfigurationManager.AppSettings["li_Exist_ApiServices"];
        private readonly string Push_Noti_ApiBaseUrl = ConfigurationManager.AppSettings["Cloud_Notification_ApiServices"];
        private readonly string Sql_Connection = ConfigurationManager.ConnectionStrings["SSTMDbContext"].ConnectionString;
        private const string Singapore_Code = "+65";
        private readonly string Test_Mobile_Number = ConfigurationManager.AppSettings["TestMobileNumber"];
        private readonly string Test_Email = ConfigurationManager.AppSettings["TestEmail"];
        private readonly string Test_Device = ConfigurationManager.AppSettings["TestDevice"];

        public CreateIntropageService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        DateFormat dtformat = new DateFormat();
        /// <summary>
        /// CreateIntropage
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public StudentIntroPage CreateIntropage(StudentIntroPage student)
        {
            try
            {
                string response = string.Empty;
                student.CreatedOn = DateTime.Now;
                student.IsActive = true;
                StudentIntroPage getEntity = SaveRecord(student);
                response = "Added";
                return getEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public StudentIntroPage SaveRecord(StudentIntroPage entity)
        {
            if (entity.StudentIntroPageId > 0)
                Update(entity);
            else
                Add(entity);
            return entity;
        }
        /// <summary>
        /// GetAllStudent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StudentIntroPage> GetAllStudent(long courseId, string batchId, DateTime date)
        {
            try
            {
                List<StudentIntroPage> getStudentIntroPage = GetMany(x => x.CourseId == courseId && x.BatchId == batchId).ToList();
                getStudentIntroPage = getStudentIntroPage.Where(x => x.CreatedOn.Date.ToString() == dtformat.GetSingaporeTime().Date.ToString()).ToList();              

                return getStudentIntroPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<StudentIntroPage> GetAllStudent(long courseId, string batchId)
        {
            try
            {
                List<StudentIntroPage> getStudentIntroPage = GetMany(x => x.CourseId == courseId && x.BatchId == batchId).ToList();              

                return getStudentIntroPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetAllTodayStudent(long courseId, long batchId)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(string.Concat(ApiBaseUrl, "StudentdatawithcourseandbatchINTROPAGE?courseid=" + courseId + "&batchid=" + batchId))
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(string.Empty).Result;
                if (response.IsSuccessStatusCode)
                {
                    string dataObjects = await response.Content.ReadAsStringAsync();
                    string response_json = dataObjects.ToString();
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("[");
                    stringBuilder.Append(response_json);
                    stringBuilder.Append("]");
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(stringBuilder.ToString());
                    return stringBuilder.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public object SendNotification(List<TodayStudentDetails> todayStudentDetails, string link, string subject, string body, string trainerId)
        {
            try
            {
                var Sended_Notification = new Dictionary<long, SendPushNotification>();
                List<SSTM.Core.IntroPage.StudentNotification> save_noti = new List<SSTM.Core.IntroPage.StudentNotification>();
                if (todayStudentDetails.Any())
                {
                    //todayStudentDetails = todayStudentDetails.Take(3).ToList();
                    //todayStudentDetails.ToList().ForEach(x =>
                    //{
                    //    x.Mobile = "";
                    //    x.Email = "teversafe@gmail.com";
                    //});
                    string baseUrl = link;
                    //foreach (var send_noti in todayStudentDetails)
                    //{

                    List<TodayStudentDetails> lstStudentDetails = new List<TodayStudentDetails>();

                    if (todayStudentDetails.Any())
                    {
                        if (!string.IsNullOrEmpty(Test_Mobile_Number))
                        {
                            lstStudentDetails = todayStudentDetails.Take(1).ToList();

                            todayStudentDetails = lstStudentDetails;
                        }
                    }

                    todayStudentDetails.ToList().ForEach(send_noti =>
                    {
                        body = "<b>Dear " + send_noti.Studentname + "<b><br/><br/>Welcome to Eversafe's Instant performance reflection(IPR) session.<br/> Please click the below link to participant in the Intro Page, <br/>Instant Testing and Instant Feedback Link ";
                        link = "";
                        link = baseUrl;
                        StudentDetails student = new StudentDetails()
                        {
                            BatchId = send_noti.Batchid.ToString(),
                            StudentId = send_noti.StudentId.ToString(),
                            CourseId = send_noti.Courseid.ToString(),
                            ChapterId = send_noti.ChapterId,
                            StudentName = send_noti.Studentname,
                            Date = DateTime.Now,
                            TrainerId = trainerId
                        };
                        string jsonStudent = JsonConvert.SerializeObject(student);
                        jsonStudent = UtilityHelper.JsonSstmEncrypt(jsonStudent);
                        link = link + jsonStudent;
                        body = body + link;
                        //HttpClient client = new HttpClient
                        //{
                        //    BaseAddress = new Uri(string.Concat(ApiBaseUrl, "TodayStudemtidForDEviceINTROPAGE?studentid=" + send_noti.StudentId))
                        //};

                        SSTM.Business.CourseServiceMethod.SSTM service = new SSTM.Business.CourseServiceMethod.SSTM();
                        List<StudentDeviceDetails> TodayClass_data = new List<StudentDeviceDetails>();
                        
                        string devicedata = service.TodayStudemtidForDEviceINTROPAGE(Convert.ToInt32(send_noti.StudentId));
                        TodayClass_data = JsonConvert.DeserializeObject<List<StudentDeviceDetails>>(devicedata);
                       
                        StudentDeviceDetails deviceDetails = new StudentDeviceDetails();
                      

                        if (TodayClass_data.Count() == 0)
                        {
                            deviceDetails.DeviceId = string.Empty;
                            deviceDetails.DeviceDetail = string.Empty;
                            deviceDetails.Studentname = string.Empty;
                        }
                        else
                        {
                            try
                            {
                                int counter = 0;
                                foreach (var item in TodayClass_data)
                                {
                                    if (counter == 0)
                                    {
                                        deviceDetails.DeviceId = item.DeviceId;
                                        deviceDetails.DeviceDetail = item.DeviceDetail;
                                        deviceDetails.Studentname = item.Studentname;
                                    }
                                    counter++;
                                }

                            }
                            catch (Exception)
                            {
                                deviceDetails.DeviceId = "";
                                deviceDetails.DeviceDetail = "";
                                deviceDetails.Studentname = "";
                            }
                        }
                        SSTM.Core.IntroPage.StudentNotification notification = new SSTM.Core.IntroPage.StudentNotification();

                        //var notificationStatus = !string.IsNullOrEmpty(Test_Mobile_Number) ? SendNotificationFromSms(Test_Mobile_Number, link, subject, body) : SendNotificationFromSms(send_noti.Mobile, link, subject, body);

                        if (!string.IsNullOrEmpty(Test_Mobile_Number)) send_noti.Mobile = Test_Mobile_Number;

                        if (!string.IsNullOrEmpty(Test_Email)) send_noti.Email = Test_Email;

                        var notificationStatus =  SendNotificationFromSms(send_noti.Mobile, link, subject, body); //SendCloudPushNotification(deviceDetails.DeviceId, link, subject, body); //
                        if (!notificationStatus.IsSended)
                        {
                            if (IsValidEmail(send_noti.Email) && !string.IsNullOrEmpty(send_noti.Email))
                            {
                                var sendNotification = SendNotificationFromEmail(send_noti.Email, link, subject, body);
                                if (!sendNotification.IsSended)
                                {
                                    if (IsSGMobileNumber(send_noti.Mobile) && !string.IsNullOrEmpty(send_noti.Mobile))
                                    {
                                        var sendNotificationFromSms = SendNotificationFromSms(send_noti.Mobile, link, subject, body);
                                        sendNotificationFromSms.StudentName = send_noti.Studentname;
                                        Sended_Notification.Add(send_noti.StudentId, sendNotificationFromSms);
                                        notification.NotificationType = "mobile";
                                        notification.StudentId = send_noti.StudentId;
                                        notification.IsSend = true;
                                        notification.MobileNo = string.Concat(Singapore_Code, send_noti.Mobile);
                                        notification.SessionStartTime = dtformat.GetSingaporeTime().TimeOfDay;
                                        notification.SessionExpiryTime = TimeSpan.FromHours(3);
                                        notification.Message = body;
                                    }
                                }
                                else
                                {
                                    notification.NotificationType = "email";
                                    notification.IsSend = true;
                                    notification.StudentId = send_noti.StudentId;
                                    notification.ToAddress = send_noti.Email;
                                    notification.Body = body;
                                    notification.Subject = subject;
                                    notification.SessionStartTime = dtformat.GetSingaporeTime().TimeOfDay;
                                    notification.SessionExpiryTime = TimeSpan.FromHours(3);
                                    SendPushNotification sendPushNotification = new SendPushNotification()
                                    {
                                        StudentName = send_noti.Studentname,
                                        IsSended = notificationStatus.IsSended,
                                        SendingType = "Email",
                                        Message = body
                                    };
                                    Sended_Notification.Add(send_noti.StudentId, sendPushNotification);
                                }
                            }
                            else if (IsSGMobileNumber(send_noti.Mobile) && !string.IsNullOrEmpty(send_noti.Mobile))
                            {

                                var sendNotificationFromSms = SendNotificationFromSms(send_noti.Mobile, link, subject, body);
                                sendNotificationFromSms.StudentName = send_noti.Studentname;
                                notification.NotificationType = "mobile";
                                notification.IsSend = true;
                                notification.StudentId = send_noti.StudentId;
                                notification.MobileNo = string.Concat(Singapore_Code, send_noti.Mobile);
                                notification.SessionStartTime = dtformat.GetSingaporeTime().TimeOfDay;
                                notification.SessionExpiryTime = TimeSpan.FromHours(3);
                                notification.Message = body;
                                Sended_Notification.Add(send_noti.StudentId, sendNotificationFromSms);

                            }
                            else
                            {
                                notification.IsSend = false;
                                notification.DeviceType = string.Empty;
                                notification.Body = string.Empty;
                                notification.Message = string.Empty;
                                notification.Link = link;
                                notification.StudentId = send_noti.StudentId;
                                notification.NotificationType = string.Empty;
                                notification.SessionStartTime = null;
                                notification.SessionExpiryTime = null;
                                SendPushNotification sendPushNotification = new SendPushNotification()
                                {
                                    StudentName = send_noti.Studentname,
                                    IsSended = false,
                                    SendingType = "No Device",
                                    Message = "Couldn't Send Any Notification"
                                };
                                Sended_Notification.Add(send_noti.StudentId, sendPushNotification);
                            }
                        }
                        else
                        {
                            notification.IsSend = true;
                            notification.StudentId = send_noti.StudentId;
                            notification.NotificationType = "cloud-message";
                            notification.DeviceId = deviceDetails.DeviceId;
                            notification.DeviceType = deviceDetails.DeviceDetail;
                        }
                        notification.NotificationId = Guid.NewGuid();
                        notification.SessionStartTime = string.IsNullOrEmpty(notification.SessionStartTime.ToString()) ? TimeSpan.Zero : dtformat.GetSingaporeTime().TimeOfDay;
                        notification.SessionExpiryTime = string.IsNullOrEmpty(notification.SessionExpiryTime.ToString()) ? TimeSpan.Zero : TimeSpan.FromHours(3);
                        notification.Link = link;
                        notification.IsRecieved = false;
                        notification.CreatedOn = DateTime.Now;
                        notification.IsSend = notification.IsSend;
                        save_noti.Add(notification);
                    });
                       //});
                    }
                //}
                var data = new
                {
                    Sended_Notification = Sended_Notification,
                    SaveNotification = save_noti
                };
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private SendPushNotification SendNotificationFromSms(string mobileNumber, string link, string subject, string body)
        {
            SendPushNotification sendPushNotification;
            try
            {
                if (string.IsNullOrEmpty(mobileNumber))
                {
                    sendPushNotification = new SendPushNotification()
                    {
                        IsSended = false,
                        Message = "Send Not Notification",
                        SendingType = "Mobile"
                    };
                    return sendPushNotification;
                }
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Exist_ApiBaseUrl)
                };
                var data = new
                {
                    mobile = Singapore_Code + mobileNumber,
                    Message = body,
                };
                HttpResponseMessage response = client.GetAsync("sentSmsForcovid?mobile=" + data.mobile + "&Message=" + data.Message).Result;
                if (response.IsSuccessStatusCode)
                {
                    string dataObjects = response.Content.ReadAsStringAsync().Result;
                    JsonSerializer serializer = new JsonSerializer();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(dataObjects);
                    sendPushNotification = new SendPushNotification()
                    {
                        IsSended = true,
                        Message = doc.InnerText
                    };
                    return sendPushNotification;
                }
                sendPushNotification = new SendPushNotification()
                {
                    IsSended = false,
                    Message = "Send Notification",
                    SendingType = "Mobile",
                };
                return sendPushNotification;
            }
            catch (Exception)
            {
                sendPushNotification = new SendPushNotification()
                {
                    IsSended = false,
                    Message = "Couldn't Send Notification"
                };
                return sendPushNotification;
            }
        }


        public SendPushNotification SendNotificationFromEmail(string emailAddress, string link, string subject, string body)
        {
            SendPushNotification sendPushNotification;
            try
            {
                if (string.IsNullOrEmpty(emailAddress))
                {
                    sendPushNotification = new SendPushNotification()
                    {
                        IsSended = false,
                        Message = "Send Not Notification"
                    };
                    return sendPushNotification;
                }
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Exist_ApiBaseUrl)
                };
                var data = new
                {
                    to = emailAddress,
                    Message = body,
                    Subject = subject
                };
                HttpResponseMessage response = client.GetAsync("sentMailForcovid?to=" + emailAddress + "&Message=" + data.Message + "&Subject=" + data.Subject).Result;
                if (response.IsSuccessStatusCode)
                {
                    string dataObjects = response.Content.ReadAsStringAsync().Result;
                    JsonSerializer serializer = new JsonSerializer();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(dataObjects);
                    sendPushNotification = new SendPushNotification()
                    {
                        IsSended = true,
                        Message = doc.InnerText
                    };
                    return sendPushNotification;
                }
                sendPushNotification = new SendPushNotification()
                {
                    IsSended = false,
                    Message = "Send Not Notification"
                };
                return sendPushNotification;
            }
            catch (Exception)
            {
                sendPushNotification = new SendPushNotification()
                {
                    IsSended = false,
                    Message = "Not Send Notification"
                };
                return sendPushNotification;
            }
        }
        private SendPushNotification SendCloudPushNotification(string studentDeviceId, string link, string subject, string body)
        {
            if (!string.IsNullOrEmpty(Test_Device)) studentDeviceId = Test_Device;

            SendPushNotification sendPushNotification;
            try
            {
                if (string.IsNullOrEmpty(studentDeviceId))
                {
                    sendPushNotification = new SendPushNotification()
                    {
                        IsSended = false,
                        Message = "Send Not Notification"
                    };
                    return sendPushNotification;
                }
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Push_Noti_ApiBaseUrl)
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var data = new
                {
                    id = studentDeviceId,
                    title = subject,
                    message = body,
                    type = "text"
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(string.Empty, stringContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    string dataObjects = response.Content.ReadAsStringAsync().Result;
                    sendPushNotification = new SendPushNotification()
                    {
                        IsSended = true,
                        Message = dataObjects
                    };
                    return sendPushNotification;
                }
                sendPushNotification = new SendPushNotification()
                {
                    IsSended = false,
                    Message = "Send Not Notification"
                };
                return sendPushNotification;
            }
            catch (Exception ex)
            {
                sendPushNotification = new SendPushNotification()
                {
                    IsSended = false,
                    Message = ex.Message + " Not Send Notification"
                };
                return sendPushNotification;
            }
        }
        public async Task<TrainerSectionDetails> GetCurrentCourseAndBatch(string date, string trainerId)
        {
            try
            {
                TrainerSectionDetails trainerSectionDetails = new TrainerSectionDetails();

                string response_json = string.Empty;
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(string.Concat(ConfigurationManager.AppSettings["li_ApiServices"], "TodayCoursesWithSectionTainerid?date=" + date + "&trainerId=" + trainerId))
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(string.Empty).Result;
                if (response.IsSuccessStatusCode)
                {
                    string dataObjects = await response.Content.ReadAsStringAsync();
                    JsonSerializer serializer = new JsonSerializer();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(dataObjects);

                    List<TrainerSectionDetails> data = JsonConvert.DeserializeObject<List<TrainerSectionDetails>>(doc.InnerText);
                    List<DateTime> dates = new List<DateTime>();

                    foreach (TrainerSectionDetails item in data)
                    {
                        string to = "";
                        string from = (DateTime.Parse(item.section1SectionName.Split(' ')[0].Replace('.', ':'))).ToString("HH:mm");
                        if (item.section1SectionName.Split(' ')[2] != "")
                        {
                            //to = (DateTime.Parse(item.section1SectionName.Split(' ')[2].Replace('.', ':'))).ToString("HH:mm");

                            DateTime to1 = new DateTime();

                            var validTime = DateTime.TryParse(item.section1SectionName.Split(' ')[2].Replace('.', ':'), out to1);

                            if(!validTime)
                            {
                                to = (DateTime.Parse(item.section1SectionName.Split(' ')[3].Replace('.', ':'))).ToString("HH:mm");
                            }
                        }
                        else
                            to = "00:00";
                        DateTime currentTime = new DateTime();
                        currentTime = dtformat.GetSingaporeTime();

                        trainerSectionDetails.TrainerCourseid = item.Courseid;
                        trainerSectionDetails.TrainerBatchId = item.BatchId;

                        //if (Convert.ToDateTime(from).TimeOfDay <= dtformat.GetSingaporeTime().TimeOfDay && Convert.ToDateTime(to).TimeOfDay >= dtformat.GetSingaporeTime().TimeOfDay)
                        //{
                        //    //match found
                        //    trainerSectionDetails.Courseid = item.Courseid;
                        //    trainerSectionDetails.BatchId = item.BatchId;
                        //    break;
                        //}

                        trainerSectionDetails.Courseid = item.Courseid;
                        trainerSectionDetails.BatchId = item.BatchId;
                        break;
                    }
                    return trainerSectionDetails;
                }
                else
                {
                    return trainerSectionDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<TrainerDetails> GetTrainerDetails(long courseid, long batchid, long trainerId)
        {
            try
            {
                string response_json = string.Empty;
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(string.Concat(ConfigurationManager.AppSettings["li_ApiServices"], "TrainerDetailsINTROPAGE?courseid=" + courseid + "&batchid=" + batchid + "&Todaydate=" + DateTime.Now.ToString("yyyy/MM/dd")))
                };
                TrainerDetails trainerDetails = new TrainerDetails();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(string.Empty).Result;
                List<TrainerDetails> trainers = new List<TrainerDetails>();
                if (response.IsSuccessStatusCode)
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    string dataObjects = await response.Content.ReadAsStringAsync();
                    response_json = dataObjects.ToString();
                    stringBuilder.Append("[");
                    stringBuilder.Append(response_json);
                    stringBuilder.Append("]");

                    if (stringBuilder.ToString() != "[[]]")
                    {
                        trainers = JsonConvert.DeserializeObject<List<TrainerDetails>>(stringBuilder.ToString());
                        if (trainers.Any())
                        {
                            trainerDetails = trainers.Where(x => x.section1Trainer == trainerId).FirstOrDefault();
                        }
                        HttpClient client2 = new HttpClient
                        {
                            BaseAddress = new Uri(string.Concat(ConfigurationManager.AppSettings["li_ApiServices"], "TrainerIndustryEXPINTROPAGE?trainerid=" + trainerId))
                        };
                        client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response2 = client2.GetAsync(string.Empty).Result;
                        if (response2.IsSuccessStatusCode)
                        {
                            string dataObjects2 = await response2.Content.ReadAsStringAsync();
                            if (dataObjects2 != "[]")
                            {
                                TrainerExperience trainerexp = JsonConvert.DeserializeObject<TrainerExperience>(dataObjects2);
                                trainerDetails.Experince = trainerexp.Experince;
                                trainerDetails.industry = trainerexp.industry;
                                trainerDetails.Aboutme = trainerexp.Aboutme;
                            }
                        }
                        return trainerDetails;
                    }
                }
                return trainerDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool IsValidEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool IsSGMobileNumber(string mobileNumber)
        {
            try
            {
                string sg_mobile = @"(\+91)?(-)?\s*?(91)?\s*?(\d{3})-?\s*?(\d{3})-?\s*?(\d{4})";
                Regex isCheck = new Regex(sg_mobile);
                if (isCheck.IsMatch(mobileNumber))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public object SendRANotification(List<TodayStudentDetails> todayStudentDetails, string link, string subject, string body, string trainerId)
        {
            try
            {
                var Sended_Notification = new Dictionary<long, SendPushNotification>();
                List<SSTM.Core.IntroPage.StudentNotification> save_noti = new List<SSTM.Core.IntroPage.StudentNotification>();
                if (todayStudentDetails.Any())
                {
                    //todayStudentDetails = todayStudentDetails.Take(3).ToList();
                    //todayStudentDetails.ToList().ForEach(x =>
                    //{
                    //    x.Mobile = "";
                    //    x.Email = "teversafe@gmail.com";
                    //});
                    string baseUrl = link;
                    //foreach (var send_noti in todayStudentDetails)
                    //{

                    List<TodayStudentDetails> lstStudentDetails = new List<TodayStudentDetails>();


                    if (todayStudentDetails.Any())
                    {
                        if (!string.IsNullOrEmpty(Test_Mobile_Number))
                        {
                            lstStudentDetails = todayStudentDetails.Take(1).ToList();

                            todayStudentDetails = lstStudentDetails;
                        }
                    }


                    todayStudentDetails.ToList().ForEach(send_noti =>
                    {
                        body = "Dear " + send_noti.Studentname + ", Click the following link to acknowledge the Risk assessment and Safe Work Procedure Briefing. Please ignore if you are already submitted<br/>";
                        link = "";
                        link = baseUrl;
                        StudentDetails student = new StudentDetails()
                        {
                            BatchId = send_noti.Batchid.ToString(),
                            StudentId = send_noti.StudentId.ToString(),
                            CourseId = send_noti.Courseid.ToString(),
                            ChapterId = send_noti.ChapterId,
                            StudentName = send_noti.Studentname,
                            Date = DateTime.Now,
                            TrainerId = trainerId
                        };
                        string jsonStudent = JsonConvert.SerializeObject(student);
                        jsonStudent = UtilityHelper.JsonSstmEncrypt(jsonStudent);
                        link = link + jsonStudent;
                        body = body + link;
                        //HttpClient client = new HttpClient
                        //{
                        //    BaseAddress = new Uri(string.Concat(ApiBaseUrl, "TodayStudemtidForDEviceINTROPAGE?studentid=" + send_noti.StudentId))
                        //};

                        SSTM.Business.CourseServiceMethod.SSTM service = new SSTM.Business.CourseServiceMethod.SSTM();
                        List<StudentDeviceDetails> TodayClass_data = new List<StudentDeviceDetails>();

                        string devicedata = service.TodayStudemtidForDEviceINTROPAGE(Convert.ToInt32(send_noti.StudentId));
                        TodayClass_data = JsonConvert.DeserializeObject<List<StudentDeviceDetails>>(devicedata);

                        StudentDeviceDetails deviceDetails = new StudentDeviceDetails();


                        if (TodayClass_data.Count() == 0)
                        {
                            deviceDetails.DeviceId = string.Empty;
                            deviceDetails.DeviceDetail = string.Empty;
                            deviceDetails.Studentname = string.Empty;
                        }
                        else
                        {
                            try
                            {
                                int counter = 0;
                                foreach (var item in TodayClass_data)
                                {
                                    if (counter == 0)
                                    {
                                        deviceDetails.DeviceId = item.DeviceId;
                                        deviceDetails.DeviceDetail = item.DeviceDetail;
                                        deviceDetails.Studentname = item.Studentname;
                                    }
                                    counter++;
                                }

                            }
                            catch (Exception)
                            {
                                deviceDetails.DeviceId = "";
                                deviceDetails.DeviceDetail = "";
                                deviceDetails.Studentname = "";
                            }
                        }

                        if (!string.IsNullOrEmpty(Test_Mobile_Number)) send_noti.Mobile = Test_Mobile_Number;

                        if (!string.IsNullOrEmpty(Test_Email)) send_noti.Email = Test_Email;

                        SSTM.Core.IntroPage.StudentNotification notification = new SSTM.Core.IntroPage.StudentNotification();
                        var notificationStatus = SendNotificationFromSms(send_noti.Mobile, link, subject, body); //SendCloudPushNotification(deviceDetails.DeviceId, link, subject, body); //
                        if (!notificationStatus.IsSended)
                        {
                            if (IsValidEmail(send_noti.Email) && !string.IsNullOrEmpty(send_noti.Email))
                            {
                                var sendNotification = SendNotificationFromEmail(send_noti.Email, link, subject, body);
                                if (!sendNotification.IsSended)
                                {
                                    if (IsSGMobileNumber(send_noti.Mobile) && !string.IsNullOrEmpty(send_noti.Mobile))
                                    {
                                        var sendNotificationFromSms = SendNotificationFromSms(send_noti.Mobile, link, subject, body);
                                        sendNotificationFromSms.StudentName = send_noti.Studentname;
                                        Sended_Notification.Add(send_noti.StudentId, sendNotificationFromSms);
                                        notification.NotificationType = "mobile";
                                        notification.StudentId = send_noti.StudentId;
                                        notification.IsSend = true;
                                        notification.MobileNo = string.Concat(Singapore_Code, send_noti.Mobile);
                                        notification.SessionStartTime = dtformat.GetSingaporeTime().TimeOfDay;
                                        notification.SessionExpiryTime = TimeSpan.FromHours(3);
                                        notification.Message = body;
                                    }
                                }
                                else
                                {
                                    notification.NotificationType = "email";
                                    notification.IsSend = true;
                                    notification.StudentId = send_noti.StudentId;
                                    notification.ToAddress = send_noti.Email;
                                    notification.Body = body;
                                    notification.Subject = subject;
                                    notification.SessionStartTime = dtformat.GetSingaporeTime().TimeOfDay;
                                    notification.SessionExpiryTime = TimeSpan.FromHours(3);
                                    SendPushNotification sendPushNotification = new SendPushNotification()
                                    {
                                        StudentName = send_noti.Studentname,
                                        IsSended = notificationStatus.IsSended,
                                        SendingType = "Email",
                                        Message = body
                                    };
                                    Sended_Notification.Add(send_noti.StudentId, sendPushNotification);
                                }
                            }
                            else if (IsSGMobileNumber(send_noti.Mobile) && !string.IsNullOrEmpty(send_noti.Mobile))
                            {

                                var sendNotificationFromSms = SendNotificationFromSms(send_noti.Mobile, link, subject, body);
                                sendNotificationFromSms.StudentName = send_noti.Studentname;
                                notification.NotificationType = "mobile";
                                notification.IsSend = true;
                                notification.StudentId = send_noti.StudentId;
                                notification.MobileNo = string.Concat(Singapore_Code, send_noti.Mobile);
                                notification.SessionStartTime = dtformat.GetSingaporeTime().TimeOfDay;
                                notification.SessionExpiryTime = TimeSpan.FromHours(3);
                                notification.Message = body;
                                Sended_Notification.Add(send_noti.StudentId, sendNotificationFromSms);

                            }
                            else
                            {
                                notification.IsSend = false;
                                notification.DeviceType = string.Empty;
                                notification.Body = string.Empty;
                                notification.Message = string.Empty;
                                notification.Link = link;
                                notification.StudentId = send_noti.StudentId;
                                notification.NotificationType = string.Empty;
                                notification.SessionStartTime = null;
                                notification.SessionExpiryTime = null;
                                SendPushNotification sendPushNotification = new SendPushNotification()
                                {
                                    StudentName = send_noti.Studentname,
                                    IsSended = false,
                                    SendingType = "No Device",
                                    Message = "Couldn't Send Any Notification"
                                };
                                Sended_Notification.Add(send_noti.StudentId, sendPushNotification);
                            }
                        }
                        else
                        {
                            notification.IsSend = true;
                            notification.StudentId = send_noti.StudentId;
                            notification.NotificationType = "cloud-message";
                            notification.DeviceId = deviceDetails.DeviceId;
                            notification.DeviceType = deviceDetails.DeviceDetail;
                        }
                        notification.NotificationId = Guid.NewGuid();
                        notification.SessionStartTime = string.IsNullOrEmpty(notification.SessionStartTime.ToString()) ? TimeSpan.Zero : dtformat.GetSingaporeTime().TimeOfDay;
                        notification.SessionExpiryTime = string.IsNullOrEmpty(notification.SessionExpiryTime.ToString()) ? TimeSpan.Zero : TimeSpan.FromHours(3);
                        notification.Link = link;
                        notification.IsRecieved = false;
                        notification.CreatedOn = DateTime.Now;
                        notification.IsSend = notification.IsSend;
                        save_noti.Add(notification);
                    });
                    //});
                }
                //}
                var data = new
                {
                    Sended_Notification = Sended_Notification,
                    SaveNotification = save_noti
                };
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
