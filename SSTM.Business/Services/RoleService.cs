using SSTM.Business.Interfaces;
using SSTM.Core.Role;
using SSTM.Data.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace SSTM.Business.Services
{
    public class RoleService : RepositoryBase<Role>, IRoleService
    {
        public RoleService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Role> GetList()
        {
            return Table.Where(a => !a.isDeleted);
        }

        public Role GetRecordById(long? Id)
        {
            return Table.Where(a => a.Id == Id).FirstOrDefault();
        }

        public long GetRecordIdByName(string name)
        {
            return Table.Where(a => !a.isDeleted && a.RoleName == name).FirstOrDefault().Id;
        }
    }
}