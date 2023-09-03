using SSTM.Core.CourseDocRemarks;
using SSTM.Models.CourseDocRemarks;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface ICourseDocRemarksService
    {
        IEnumerable<CourseDocRemarksGridModel> GetListByCourseId(long courseId);

        CourseDocRemarks GetRecordById(long Id);

        CourseDocRemarks GetRecordByDocId(long docId);

        void SaveRecord(CourseDocRemarks entity);

        void DeleteRecord(long docId);
        List<CourseDocRemarks> GetCourseremarkByList(long courseid);
    }
}