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
    public class TrainerQPUploadService : RepositoryBase<TrainerQPUpload>, ITrainerQPUploadService
    {
        public TrainerQPUploadService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        public IEnumerable<TrainerQPUploadDataModel> GetAllQPUploadDocsList(long trainerId,long SmeId)
        {
            return DataContext.Database.SqlQuery<TrainerQPUploadDataModel>("EXEC sstmo.Get_trainer_QP_List @p0,@p1", trainerId, SmeId);
        }
        public TrainerQPUpload GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).OrderByDescending(a=>a.Id).FirstOrDefault();
        }

        //public TrainerQPUpload GetAllRecord(long trainerId)
        //{
        //    if(trainerId==0)
        //        return Table.AsNoTracking().Where(a => a.Status == 0).OrderByDescending(a => a.Id).FirstOrDefault();
        //    else
        //        return Table.AsNoTracking().Where(a => a.Status == 0 && a.TrainerId == trainerId).OrderByDescending(a => a.Id).FirstOrDefault();

        //}

        public long SaveRecord(TrainerQPUpload entity)
        {
            if (entity.Id > 0)
                Update(entity);
            else
                Add(entity);

            return entity.Id;
        }
        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }
    }
}
