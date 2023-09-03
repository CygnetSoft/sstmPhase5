using SSTM.Core.ExceptionLog;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class ExceptionLogMap : EntityTypeConfiguration<ExceptionLog>
    {
        public ExceptionLogMap()
        {
            ToTable("ExceptionLog");
            HasKey(a => a.Id);
        }
    }
}