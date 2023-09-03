using SSTM.Core.ActivityLog;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class ActivityLogMap : EntityTypeConfiguration<ActivityLog>
    {
        public ActivityLogMap()
        {
            ToTable("ActivityLog");
            HasKey(a => a.Id);
        }
    }
}