using System;
using System.Collections.Generic;
namespace SSTM.Models.IntroPage
{
    public class CourseList
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseShortname { get; set; }
        public string EntryRequirment { get; set; }
    }
    public class IntroStudentCount
    {
        public int TotalStudent { get; set; }
        public int IntroPending { get; set; }
        public int IntroCompleted { get; set; }
        public string TrainerId { get; set; }
    }

    public class StudentDetails
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string CourseId { get; set; }
        public string ChapterId { get; set; }

        public string BatchId { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string TrainerId { get; set; }

    }

    public class StudentAnswerDetails
    {
        public string StudentNo { get; set; }
        public string QuestionNo { get; set; }
        public string ChapterId { get; set; }
        public string Choice { get; set; }
    }


    public class TodayStudentDetails
    {
        public long StudentId { get; set; }
        public string Studentname { get; set; }
        public long Courseid { get; set; }
        public string ChapterId { get; set; }
        public long Batchid { get; set; }
        public string CourseName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
    public class StudentDeviceDetails
    {
        public string DeviceId { get; set; }
        public string DeviceDetail { get; set; }
        public string Studentname { get; set; }
    }
    public class StudentMarkList
    {
        public string StudentNo { get; set; }
        public string StudentName { get; set; }
        public string TotalCorrectMark { get; set; }
        public string PerQuestionMark { get; set; }
        public string TotalMark { get; set; }
        public string Photo { get; set; }
    }
    public class SendPushNotification
    {
        public string StudentName { get; set; }
        public string Message { get; set; }
        public string SendingType { get; set; }
        public bool IsSended { get; set; }
    }
    public class StudentNotification
    {
        public Guid NotificationId { get; set; }
        public long StudentId { get; set; }
        public string NotificationType { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsSend { get; set; }
        public bool? IsRecieved { get; set; }
        public TimeSpan? SessionStartTime { get; set; }
        public TimeSpan? SessionExpiryTime { get; set; }
        public string Link { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
    public class NotificationResponse
    {
        public Dictionary<long, SendPushNotification> Notification { get; set; }
        public List<StudentNotification> SaveNotification { get; set; }
    } 

    public class RADetails
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }        
        public int BatchId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public string CourseName { get; set; }
    }

}
