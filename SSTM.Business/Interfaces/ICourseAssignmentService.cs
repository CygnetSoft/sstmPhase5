using SSTM.Core.CourseAssignment;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface ICourseAssignmentService
    {
        CourseAssignment GetRecordByCourseId(long courseId);
        List<CourseAssignment>  GetRecordByCourseId_list();

        void SaveRecord(CourseAssignment entity);

        void DeleteRecordByCourseId(long courseId);
    }
}