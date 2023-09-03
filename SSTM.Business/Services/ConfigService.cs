using SSTM.Business.Interfaces;
using SSTM.Core.Config;
using SSTM.Data.Infrastructure;
using System.Linq;

namespace SSTM.Business.Services
{
    public class ConfigService : RepositoryBase<Config>, IConfigService
    {
        public ConfigService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public Config GetFirstRecord()
        {
            return Table.FirstOrDefault();
        }

        public Config GetRecordById(long Id)
        {
            return Table.Where(a => !a.IsDeleted && a.Id == Id).FirstOrDefault();
        }

        public void SaveRecord(Config entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }
       
    }
}