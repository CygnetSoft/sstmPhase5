using SSTM.Business.Interfaces;
using SSTM.Core.TrainerQPUpload;
using SSTM.Data.Infrastructure;
using SSTM.Models.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class TrainerQPSharedStudentService : RepositoryBase<TrainerQP_Shared_Student>, ITrainerQPSharedStudentService
    {
        public TrainerQPSharedStudentService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        public List<TrainerStudentApiModel> GetQpToStundetList(string courseid, string batchid)
        {
            return DataContext.Database.SqlQuery<TrainerStudentApiModel>("EXEC sstmo.Get_QP_Course_and_Batch_id @p0, @p1", courseid, batchid).ToList();
        }

        public IEnumerable<TrainerQP_Shared_Student> GetRecordList(long id)
        {
            return Table.Where(a => a.QP_id == id).ToList();
        }

        public void SaveRecord(TrainerQP_Shared_Student entity)
        {
            if (entity.id == 0)
                Add(entity);
            else
                Update(entity);
        }
        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }
    }
}
