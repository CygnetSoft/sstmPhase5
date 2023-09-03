using SSTM.Business.Interfaces;
using SSTM.Core.IntroPage;
using SSTM.Data.Infrastructure;
using SSTM.Models.IntroPage;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class RiskAssessmentDeclarationService : RepositoryBase<RiskAssessmentDeclaration>, IRiskAssessmentDeclarationService
    {
        public RiskAssessmentDeclarationService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public List<RADetails> GetRiskAssessmentDeclaration(long trainerId, int courseId, int batchId, long studentId, string filter)
        {
            //List<RiskAssessmentDeclaration> result = Table.Where(x => x.TrainerId == trainerId).ToList();

            //return result;

            List<RADetails> result = DataContext.SqlQuery<RADetails>("EXEC [sstmo].[SP_Get_RiskAssessmentDetails] @p0, @p1,@p2,@p3,@p4", trainerId, courseId, batchId, studentId, filter).ToList();
            return result;

        }
        

        public void SaveRecord(RiskAssessmentDeclaration entity)
        {
            Add(entity);
        }
    }
}
