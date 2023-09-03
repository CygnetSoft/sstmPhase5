using SSTM.Core.Central_CourseSharing;
using SSTM.Models.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICentralCourseSharingService
    {
        Central_CourseSharing GetRecordById(long Id);
        List<Central_CourseSharing> GetAllRecord();
        List<Central_CourseSharing> GetAssignRecord(int CourseId);
        long SaveRecord(Central_CourseSharing entity);
        void DeleteRecord(long Id);
        IEnumerable<CentralDocumentsListModel> GetListofSharedCentralCourseDocs(int Central_master_Id);

        Central_CourseSharing GetRecordByCentraAndDocIds(int master_id, int docid);
    }
}
