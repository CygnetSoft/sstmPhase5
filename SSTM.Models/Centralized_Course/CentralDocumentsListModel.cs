using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.Centralized_Course
{
   public class CentralDocumentsListModel
    {
        public int? CentralDocId { get; set; }
        public int? DocId { get; set; }
        public Int64? StatusId { get; set; }

        public string Document_File_Name { get; set; }
        public string Document_Type_Name { get; set; }

     
        public bool isTraining { get; set; }
        public bool isPrinting { get; set; }
        public bool isDeveloper { get; set; }
    }
}
