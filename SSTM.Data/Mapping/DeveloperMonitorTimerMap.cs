using SSTM.Core.Developer_Monitor_Timer;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class DeveloperMonitorTimerMap: EntityTypeConfiguration<DeveloperMonitorTimer>
    {
        public DeveloperMonitorTimerMap()
        {
            ToTable("DeveloperMonitorTimer");
            HasKey(a => a.id);
        }
    }
}
