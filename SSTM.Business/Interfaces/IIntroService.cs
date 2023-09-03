using SSTM.Core.IntroPage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface IIntroService
    {
        Task<object> GetAllCourse();
        string AddStudentMultipleChoiceQp(List<StudentQP> student);
        Task<List<StudentQP>> GetStudentMCQ(long courseId, long chapterId);
    }
}
