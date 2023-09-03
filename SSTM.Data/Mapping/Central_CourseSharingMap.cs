using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.ModelConfiguration;
using SSTM.Core.Central_CourseSharing;

namespace SSTM.Data.Mapping
{
  public  class Central_CourseSharingMap: EntityTypeConfiguration<Central_CourseSharing>
    {
        public Central_CourseSharingMap()
        {
            ToTable("Central_CourseSharing");
            HasKey(a => a.Id);
        }
    }
}
