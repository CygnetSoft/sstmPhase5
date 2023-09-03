using SSTM.Core.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class CentralizedCourseMap : EntityTypeConfiguration<Centralized_Course>
    {
        public CentralizedCourseMap()
        {
            ToTable("Centralized_Course");
            HasKey(a => a.id);
        }
    }
}
