using SSTM.Core.Course_Reminder;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class CourseRenewalMap : EntityTypeConfiguration<CourseRenewal>
    {
        public CourseRenewalMap()
        {
            ToTable("Course_Renewal");
            HasKey(a => a.Id);
        }
    }
}
