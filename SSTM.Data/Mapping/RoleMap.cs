using SSTM.Core.Role;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            ToTable("Role");
            HasKey(a => a.Id);
        }
    }
}