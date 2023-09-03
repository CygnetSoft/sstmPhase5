using SSTM.Core.TrainingCenter;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class TrainingCenterMap : EntityTypeConfiguration<TrainingCenter>
    {
        public TrainingCenterMap()
        {
            ToTable("TrainingCenter");
            HasKey(a => a.Id);
        }
    }
}