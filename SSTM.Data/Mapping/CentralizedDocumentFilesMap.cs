using SSTM.Core.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Data.Mapping
{
    public class CentralizedDocumentFilesMap : EntityTypeConfiguration<Centralized_Document_files>
    {
        public CentralizedDocumentFilesMap()
        {
            ToTable("Centralized_Document_files");
            HasKey(a => a.id);
        }
    }
}
