using SSTM.Core.IntroPage;
using System;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface IFeedbackService
    {
        string CreateFeedback(StudentFeedback feedback);
        IEnumerable<StudentFeedback> GetAllFeedback(long courseId, string batchId, DateTime date);
        List<StudentFeedback> GetTodayAllFeedback(DateTime date);

    }
}
