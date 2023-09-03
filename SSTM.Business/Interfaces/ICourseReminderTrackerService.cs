using SSTM.Core.CourseTrackers;
using SSTM.Models.CourseTrackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICourseReminderTrackerService
    {
        
        NewCourseTrackingData GetRecordById(long Id);
        NewCourseTrackingData GetDocument(long Documentid);
        long SaveRecord(NewCourseTrackingData entity);
        void DeleteRecord(long Id);
        NewCourseTrackingModel GetNewCoursesTrackWithData(long CourseId, int flag);
        void DeleteNewCourseRecord(long Id);
    }
}
