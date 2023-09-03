using SSTM.Core.CourseStatus;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface ICourseStatusService
    {
        CourseStatus GetRecordById(string Id);

        long GetRecordIdByName(string status);

        IEnumerable<CourseStatus> GetList();
    }
}