using SSTM.Core.CourseDocVersion;
using SSTM.Models.CourseDocVersion;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface ICourseDocVersionService
    {
        CourseDocVersion GetRecordById(long Id);

        CourseDocVersion GetLatestRecordByDocId(long docId);

        IEnumerable<CourseDocVersionsListModel> GetListByDocId(long docId);

        void SaveRecord(CourseDocVersion entity);

        void DeleteRecord(long Id);

        void DeleteRecordsByDocId(long docId);

        void UpdateDocVersionStatus(long docVersionId, bool isActive, long userId);
        IEnumerable<CourseDocVersionsListModel> GetRecentDocumentList(long docId);
    }
}