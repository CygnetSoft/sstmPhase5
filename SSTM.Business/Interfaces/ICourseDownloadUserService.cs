using SSTM.Core.CourseDownload;
using SSTM.Models.CourseDownload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICourseDownloadUserService
    {
        IEnumerable<CourseDownloadUserDetailsModel> GetDownloadList(long UserId, bool MasterCourse, long MainCourseId);
        CourseDownloadUser GetRecordById(long Id);
        long SaveRecord(CourseDownloadUser entity);
        void DeleteRecord(long Id);
        bool isCourseNameExists(long CourseId, long Userid);
    }
}
