using SSTM.Business.Interfaces;
using SSTM.Core.IntroPage;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class FeedbackService : RepositoryBase<StudentFeedback>, IFeedbackService
    {
        public FeedbackService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        /// <summary>
        /// CreateFeedback
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public string CreateFeedback(StudentFeedback feedback)
        {
            try
            {
                string response = string.Empty;
                feedback.CreatedOn = DateTime.Now;
                feedback.CreatedBy = feedback.StudentName;

                feedback.IsActive = true;
                Add(feedback);
                response = "Added";
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<StudentFeedback> GetAllFeedback(long courseId, string batchId, DateTime date)
        {
            try
            {
                long bat = Convert.ToInt64(batchId);

                List<StudentFeedback> getStudentIntroPage = GetMany(x => x.CourseId == courseId && x.BatchId == bat).ToList();
                getStudentIntroPage = getStudentIntroPage.Where(x => x.CreatedOn.Date == date.Date).ToList();
                return getStudentIntroPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<StudentFeedback> GetTodayAllFeedback(DateTime date)
        {
            try
            {
                
                List<StudentFeedback> getStudentIntroPage = GetMany(x => x.Rating=="1").ToList();
                getStudentIntroPage = getStudentIntroPage.Where(x => x.CreatedOn.Date.ToString("dd/mm/yyyy") == date.Date.ToString("dd/mm/yyyy")).ToList();
               return getStudentIntroPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
