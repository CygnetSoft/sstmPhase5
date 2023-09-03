using SSTM.Business.Interfaces;
using SSTM.Core.Config;
using SSTM.Data.Infrastructure;
using System.Linq;

namespace SSTM.Business.Services
{
    public class ReportService : RepositoryBase<Report>, IReportService
    {
        public ReportService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public Report GetFirstRecord()
        {
            return Table.FirstOrDefault();
        }

        public Report GetRecordById(long Id)
        {
            return Table.Where(a => !a.IsActive && a.Id == Id).FirstOrDefault();
        }

        public void SaveRecord(Report entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }
       
    }
}