using SSTM.Core.IntroPage;
using SSTM.Models.IntroPage;
using System;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface IStudentMcqService
    {
        string SaveStudentMCQ(List<StudentQP_Written> students);
        List<StudentMarkList> GetAllStudentMarksList(string courseId, string chapterId);
        List<StudentMarkList> GetOverAllStudentMarksList(string courseId, string chapterId);

        IEnumerable<StudentQP_Written> GetAllTestCompleteStudent(long courseId, long batchId, DateTime date);
    }
}
