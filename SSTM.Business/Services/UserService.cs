using SSTM.Business.Interfaces;
using SSTM.Core.User;
using SSTM.Data.Infrastructure;
using SSTM.Models.User;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{
    public class UserService : RepositoryBase<User>, IUserService
    {
        public UserService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<User> GetDefaultList()
        {
            return Table.Where(a => !a.isDeleted && a.isActive);
        }
        public List<User> GetDirectorAndAdminList()
        {
            return Table.Where(a => !a.isDeleted && a.isActive && a.RoleId==1 || a.RoleId==4).ToList();
        }

        public IEnumerable<User> GetOuterRegisterUserList(bool isActive)
        {
            return Table.Where(a => !a.isDeleted && a.isActive== isActive && a.RoleId==0);
        }

        public IEnumerable<UsersListModel> GetList(int isActive)
        {
            return DataContext.SqlQuery<UsersListModel>("EXEC sstmo.GetListOfUsers @p0", isActive);
        }

        public User GetRecordForLogin(string email, string password)
        {
            return Table.Where(a => !a.isDeleted && a.isActive && a.Email == email && a.Password == password && a.RoleId != 0).FirstOrDefault();
        }

        public User GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id && a.RoleId != 0).FirstOrDefault();
        }

        public User GetOuterLoginRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id && a.RoleId == 0).FirstOrDefault();
        }
        public IEnumerable<User> GetRecordByStaff(int id)
        {
            return Table.AsNoTracking().Where(a => a.RoleId == id && a.RoleId != 0).ToList(); // staff role id 8
        }
       

        public void SaveRecord(User entity)
        {
            if (entity.Id == 0)
                Add(entity);
            else
                Update(entity);
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id && a.RoleId != 0).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }

        public bool isEmailExists(long Id, string email)
        {
            if (Table.Where(a => !a.isDeleted && a.Id != Id && a.Email == email && a.RoleId != 0).Count() > 0)
                return true;

            return false;
        }

        public bool isMobileExists(long Id, string mobile)
        {
            if (Table.Where(a => !a.isDeleted && a.Id != Id && a.Mobile == mobile && a.RoleId != 0).Count() > 0)
                return true;

            return false;
        }

        public bool isPasswordExists(long Id, string password)
        {
            if (Table.Where(a => !a.isDeleted && a.Id != Id && a.Password == password && a.RoleId != 0).Count() > 0)
                return true;

            return false;
        }

        public bool isMacAddressExists(string macAddress, long Id)
        {
            if (Table.Where(a => !a.isDeleted && a.MacAddress == macAddress && a.Id!= Id && a.RoleId != 2).Count() > 0)
                return true;

            return false;
        }

        public IEnumerable<User> GetUserByMacAddress(string macAddress, long Id)
        {
            return Table.Where(a => !a.isDeleted && a.MacAddress == macAddress && a.Id != Id && a.RoleId != 2).ToList();                
        }
    }
}