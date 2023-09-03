using SSTM.Core.Role;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface IRoleService
    {
        IEnumerable<Role> GetList();

        Role GetRecordById(long? Id);

        long GetRecordIdByName(string name);
    }
}