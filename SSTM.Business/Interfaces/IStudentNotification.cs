
using System.Collections.Generic;
using SSTM.Models.IntroPage;

namespace SSTM.Business.Interfaces
{
    public interface IStudentNotification
    {
        void SaveStudentNotification(List<StudentNotification> saveNotification);

        bool CheckStudentNotificationExists(long studentId);
    }
}
