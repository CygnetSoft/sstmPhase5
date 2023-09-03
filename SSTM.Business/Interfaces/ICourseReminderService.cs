using SSTM.Core.Course_Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public  interface ICourseReminderService
    {
        long SaveRecord(CourseReminder entity);
        void DeleteRecord(long Id);
        bool isCourseNameExists(long Id, string courseName);
        CourseReminder GetRecordById(long Id);
        IEnumerable<CourseReminder> GetAllRecord(bool MasterCourse, long MasterCourseId);
        IEnumerable<CourseReminder> GetAllRecordwithDeveloper(bool MasterCourse, long MasterCourseId, long developerid);
        CourseReminder GetRecordByLiId(long Id);
        IEnumerable<CourseReminder> GetPendingCourseList();
        IEnumerable<CourseReminder> GetRenewalCourseList();
        List<CourseReminder> GetAllRecord();

    }
}
