using SSTM.Core.Centralized_Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ICentralizedDocumentFilesService
    {
        Centralized_Document_files GetRecordById(long Id);
        List<Centralized_Document_files> GetAllRecord();
        Centralized_Document_files GetRecordNewcourseById(long Id);
        long SaveRecord(Centralized_Document_files entity);
        void DeleteRecord(long Id);
        List<Centralized_Document_files> GetAllbyCenterRecord(long Central_Master_Id);
        Centralized_Document_files GetRecordNewcourseByTypewithmasterid(long master_id, string type);
    }
}
