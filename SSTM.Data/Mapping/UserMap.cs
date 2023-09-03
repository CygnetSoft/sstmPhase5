using SSTM.Core.User;
using System.Data.Entity.ModelConfiguration;

namespace SSTM.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("User");
            HasKey(a => a.Id);
        }
    }
}