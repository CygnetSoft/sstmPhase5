using SSTM.Core.Assessment_Paper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface IAssessmentPaperService
    {
        long SaveRecord(AssessmentPaper entity);
        void DeleteRecord(long Id);
        AssessmentPaper GetFirstRecord();
        AssessmentPaper GetRecordById(long Id);
        List<AssessmentPaper> GetRecordlist();
        //AssessmentPaper GetCheckCourseExistById(long courseId);
        AssessmentPaper GetCheckCourseExistById(long courseid, long id, string trainer_id, int qty, decimal batchid);
        AssessmentPaper isexist_record(int courseid, decimal batchid, string fin);
    }
}
