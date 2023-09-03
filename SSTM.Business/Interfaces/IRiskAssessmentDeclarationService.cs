using SSTM.Core.IntroPage;
using SSTM.Models.IntroPage;
using System.Collections.Generic;
namespace SSTM.Business.Interfaces
{
    public interface IRiskAssessmentDeclarationService
    {
        List<RADetails> GetRiskAssessmentDeclaration(long trainerId, int courseId, int batchId, long studentId, string filter);       

        void SaveRecord(RiskAssessmentDeclaration entity);
    }
}
