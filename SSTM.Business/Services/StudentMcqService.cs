using SSTM.Business.Interfaces;
using SSTM.Core.IntroPage;
using SSTM.Data.Infrastructure;
using SSTM.Models.IntroPage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class StudentMcqService : RepositoryBase<StudentQP_Written>, IStudentMcqService
    {
        public StudentMcqService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        public string SaveStudentMCQ(List<StudentQP_Written> students)
        {
            if (students.Any())
            {
                students.ForEach(entity =>
                {
                    entity.CreatedOn = DateTime.Now;
                    Add(entity);
                });
            }
            string result = DataContext.SqlQuery<string>("EXEC [dbo].[SP_Get_StudentExamPercentOnCourse] @p0,@p1,@p2,@p3", students.First().StudentNo, students.First().CourseId, students.First().ChapterId, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
            return result;
        }
        public IEnumerable<StudentQP_Written> GetAllTestCompleteStudent(long courseId, long batchId, DateTime date)
        {
            try
            {
                List<StudentQP_Written> getWritten = GetMany(x => x.CourseId == courseId && x.BatchId == batchId).ToList();
                getWritten = getWritten.Where(x => x.CreatedOn.Date.ToShortDateString() == date.Date.ToShortDateString()).ToList();
                return getWritten;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<StudentMarkList> GetAllStudentMarksList(string courseId, string chapterId)
        {
            List<StudentMarkList> result = new List<StudentMarkList>();
            try
            {
               result = DataContext.SqlQuery<StudentMarkList>("EXEC [dbo].[SP_Get_ExamMarkBasedOnCourse] @p0, @p1,@p2", courseId.ToString(), chapterId.ToString(), DateTime.Now.ToString("yyyy-MM-dd")).ToList();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return result;
        }
        public List<StudentMarkList> GetOverAllStudentMarksList(string courseId, string chapterId)
        {
            List<StudentMarkList> result = DataContext.SqlQuery<StudentMarkList>("EXEC [dbo].[SP_Get_OverallStudentMarksBasedOnCourse] @p0, @p1,@p2", courseId.ToString(), chapterId.ToString(), DateTime.Now.ToString("yyyy-MM-dd")).ToList();
            return result;
        }   
    }
}
