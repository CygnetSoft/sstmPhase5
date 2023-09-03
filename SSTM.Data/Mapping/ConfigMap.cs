using SSTM.Core.Config;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class ConfigMap : EntityTypeConfiguration<Config>
    {
        public ConfigMap()
        {
            ToTable("Config");
            HasKey(a => a.Id);
        }
    }
}