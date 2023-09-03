using System.Data.Entity.ModelConfiguration;
using SSTM.Core.IntroPage;

namespace SSTM.Data.Mapping
{
    public class RiskAssessmentDeclarationMap : EntityTypeConfiguration<RiskAssessmentDeclaration>
    {
        public RiskAssessmentDeclarationMap()
        {
            ToTable("RiskAssessmentDeclaration");
            HasKey(a => a.Id);
        }
    }
}
