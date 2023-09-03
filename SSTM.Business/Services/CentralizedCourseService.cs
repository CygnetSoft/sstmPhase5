using SSTM.Business.Interfaces;
using SSTM.Core.Centralized_Course;
using SSTM.Data.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{

    public class CentralizedCourseService : RepositoryBase<Centralized_Course>, ICentralizedCourseService
    {
        public CentralizedCourseService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public Centralized_Course GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }
        public List<Centralized_Course> GetAllRecord()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.id).ToList();
        }
        public List<Centralized_Course> GetAllbyCenterRecord(long Central_Master_Id)
        {
            return Table.AsNoTracking().Where(a => a.center_master_id == Central_Master_Id).OrderByDescending(a => a.center_master_id).ToList();
        }
        public Centralized_Course GetRecordNewcourseById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.id == Id).FirstOrDefault();
        }

        public List<Centralized_Course> GetCourseReplacedataWithType(string type,int center_master_id)
        {
            return Table.AsNoTracking().Where(a => a.type == type && a.center_master_id== center_master_id).ToList();
        }

        public long SaveRecord(Centralized_Course entity)
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

        public void DeleteAllRecord(long Id)
        {
            var entity = Table.Where(a => a.center_master_id == Id).ToList();
            foreach (var item in entity)
            {
                var data = Table.Where(a => a.id == item.id).FirstOrDefault();
                if (data != null)
                    Delete(data);
            }
            
        }
    }
}
