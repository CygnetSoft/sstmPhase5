using SSTM.Business.Interfaces;
using SSTM.Core.TrainnerMacAddress;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{

    public class TrainnerMacAddressService : RepositoryBase<TrainnerMacAddress>, ITrainnerMacAddressService
    {
        public TrainnerMacAddressService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        //public IEnumerable<CourseListModel> GetList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        //{
        //    return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        //}
        public TrainnerMacAddress GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }
        public long SaveRecord(TrainnerMacAddress entity)
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
        public IEnumerable<TrainnerMacAddress> GetAllMacAddress()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.Id).ToList();
        }

    }
}
