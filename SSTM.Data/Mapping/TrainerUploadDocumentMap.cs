using SSTM.Core.TrainerUploadDocument;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
   public class TrainerUploadDocumentMap : EntityTypeConfiguration<TrainerUploadDocument>
    {
        public TrainerUploadDocumentMap()
        {
            ToTable("TrainerUploadDocument");
            HasKey(a => a.Id);
        }
    }
}
