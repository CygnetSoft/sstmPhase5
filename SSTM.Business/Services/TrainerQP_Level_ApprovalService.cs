using SSTM.Business.Interfaces;
using SSTM.Core.TrainerQPUpload;
using SSTM.Data.Infrastructure;
using SSTM.Models.TrainerQPUpload;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class TrainerQP_Level_ApprovalService : RepositoryBase<TrainerQP_Level_Approval>, ITrainerQP_Level_ApprovalService
    {
        public TrainerQP_Level_ApprovalService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        public IEnumerable<Trainer_qp_level_data_model> Trainer_QP_Level_Approval_data()
        {
            return DataContext.Database.SqlQuery<Trainer_qp_level_data_model>("EXEC sstmo.sp_get_QP_Approval_list");
        }
        public TrainerQP_Level_Approval GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).OrderByDescending(a => a.id).FirstOrDefault();
        }

        public TrainerQP_Level_Approval GetRecordByQPId(long Id)
        {
            return Table.AsNoTracking().Where(a => a.QP_Id == Id).OrderByDescending(a => a.id).FirstOrDefault();
        }


        public long SaveRecord(TrainerQP_Level_Approval entity)
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
