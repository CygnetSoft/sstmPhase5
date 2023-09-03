using SSTM.Core.Config;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class ReportMap : EntityTypeConfiguration<Report>
    {
        public ReportMap()
        {
            ToTable("Report");
            HasKey(a => a.Id);
        }
    }
}