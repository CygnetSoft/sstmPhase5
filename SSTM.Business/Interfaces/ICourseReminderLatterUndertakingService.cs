using SSTM.Core.Course_Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICourseReminderLatterUndertakingService
    {
        IEnumerable<Course_Reminder_Latter_Undertaking> GetAllRecord();
        Course_Reminder_Latter_Undertaking GetRecordById(long Id);
        long SaveRecord(Course_Reminder_Latter_Undertaking entity);
        void DeleteRecord(long Id);
        Course_Reminder_Latter_Undertaking GetFirstRecord();
    }
}
