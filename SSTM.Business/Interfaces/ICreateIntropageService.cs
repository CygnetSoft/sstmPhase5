using SSTM.Core.IntroPage;
using SSTM.Models.IntroPage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICreateIntropageService
    {
        IEnumerable<StudentIntroPage> GetAllStudent(long courseId, string batchId, DateTime date);
        IEnumerable<StudentIntroPage> GetAllStudent(long courseId, string batchId);
        Task<string> GetAllTodayStudent(long courseId, long batchId);
       
        Task<TrainerSectionDetails> GetCurrentCourseAndBatch(string date, string trainerId);
        Task<TrainerDetails> GetTrainerDetails(long courseid, long batchid, long trainerId);
        dynamic SendNotification(List<TodayStudentDetails> todayStudentDetails, string link, string subject, string body, string trainerId);
        StudentIntroPage CreateIntropage(StudentIntroPage student);
        SendPushNotification SendNotificationFromEmail(string emailAddress, string link, string subject, string body);

        dynamic SendRANotification(List<TodayStudentDetails> todayStudentDetails, string link, string subject, string body, string trainerId);
    }
}
