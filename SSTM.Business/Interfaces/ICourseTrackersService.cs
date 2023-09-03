using SSTM.Core.CourseTrackers;
using SSTM.Models.CourseTrackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICourseTrackersService
    {
        CourseTrackers GetRecordById(long Id);
        CourseTrackers GetDocument(long Documentid);
        long SaveRecord(CourseTrackers entity);
        void DeleteRecord(long Id);
        CourseTrackingDataModel GetCoursesTrackWithData(long CourseId);
        NewCourseTrackingDataModel GetNewCoursesTrackWithData(long CourseId);
    }
}
