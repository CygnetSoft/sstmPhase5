using SSTM.Business.Interfaces;
using SSTM.Core.Assessment_Paper;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{

    public class AssessmentPaperService : RepositoryBase<AssessmentPaper>, IAssessmentPaperService
    {
        public AssessmentPaperService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public AssessmentPaper GetFirstRecord()
        {
            return Table.FirstOrDefault();
        }

        public List<AssessmentPaper> GetRecordlist()
        {
            return Table.ToList();
        }
        public AssessmentPaper GetRecordById(long Id)
        {
            return Table.Where(a => a.id == Id).FirstOrDefault();
        }
        public AssessmentPaper isexist_record(int courseid, decimal batchid, string fin)
        {
            return Table.Where(a => a.courseid == courseid && a.batchid == batchid && a.student_fin == fin).FirstOrDefault();
        }


        public AssessmentPaper GetCheckCourseExistById(long courseid,long id, string trainer_id, int qty, decimal batchid)
        {
           return Table.Where(a => a.courseid == courseid && a.id != id && a.trainer_id == trainer_id && a.qty == qty && a.batchid== batchid).FirstOrDefault();
        }

        public long SaveRecord(AssessmentPaper entity)
        {
            if (entity.id > 0)
                Update(entity);
            else
                Add(entity);

            return entity.id;
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }

    }
}
