using SSTM.Core.CourseDocument;
using SSTM.Models.CourseDocument;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface ICourseDocumentService
    {
        IEnumerable<CourseDocumentsListModel> GetListByCourseId(long courseId, bool MasterCourse);
        IEnumerable<CourseDocumentsListModel> GetListVersionByCourseId(long courseId);
        CourseDocument GetRecordById(long Id);

        IEnumerable<CourseDocument> UpdateDocumentStatusByUserRecordById(long Userid);

        long SaveRecord(CourseDocument entity);

        void DeleteRecord(long Id);

        bool isExistsDocNameForCourse(string docName, long courseId, long docId);
        IEnumerable<CourseDocumentsListModel> GetListByOldCourse();
    }
}