using SSTM.Business.Interfaces;
using SSTM.Core;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
  
    public class QPRequestService : RepositoryBase<QPRequest>, IQPRequestService
    {
        public QPRequestService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public QPRequest GetFirstRecord()
        {
            return Table.FirstOrDefault();
        }

        public QPRequest GetRecordById(long Id)
        {
            return Table.Where(a=>a.Id == Id).FirstOrDefault();
        }
        public List<QPRequest> GetAllRecord()
        {
            return Table.OrderByDescending(a => a.Id).ToList();
        }
        public void SaveRecord(QPRequest entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }
        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }
    }
}
