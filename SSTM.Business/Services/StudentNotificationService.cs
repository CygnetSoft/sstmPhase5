using SSTM.Business.Interfaces;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using SSTM.Models.IntroPage;
using SSTM.Helpers.Common;

namespace Business.Services
{
    public class StudentNotificationService : RepositoryBase<StudentNotification>, IStudentNotification
    {
        public StudentNotificationService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        public void SaveStudentNotification(List<StudentNotification> saveNotification)
        {
            if (saveNotification.Any())
            {
                try
                {
                    saveNotification.ForEach(entity =>
                    {
                        entity.CreatedOn = DateTime.Now;
                        Add(entity);
                    });
                }
                catch (Exception ex)
                {
                    saveNotification.ForEach(entity =>
                    {
                        entity.CreatedOn = DateTime.Now;
                        DataContext.Database.ExecuteSqlCommand("EXEC [sstmo].[InsertServiceNotification] @p0, @p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16",
                        entity.NotificationId, entity.StudentId, entity.NotificationType, entity.DeviceId,entity.DeviceType,entity.MobileNo,entity.Message,entity.ToAddress,entity.Subject,entity.Body
                        ,entity.IsSend,entity.IsRecieved,entity.SessionStartTime,entity.SessionExpiryTime,entity.Link,entity.CreatedOn,entity.UpdatedOn);
                    });
                }
            }
        }

        public bool CheckStudentNotificationExists(long studentId)
        {
            List<StudentNotification> result = DataContext.SqlQuery<StudentNotification>("EXEC [dbo].[SP_Get_ChapterwiseStudentNotification_Status] @p0, @p1",
                        studentId, DateTime.Now.ToString("yyyy-MM-dd")).ToList();

            return result.Any() ? true: false;
        }
    }
}
