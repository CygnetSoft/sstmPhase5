using SSTM.Core.User;
using SSTM.Models.User;
using System.Collections.Generic;

namespace SSTM.Business.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetDefaultList();
        List<User> GetDirectorAndAdminList();

        IEnumerable<UsersListModel> GetList(int isActive);
        IEnumerable<User> GetOuterRegisterUserList(bool isActive);
        User GetRecordForLogin(string email, string password);

        User GetRecordById(long Id);
        User GetOuterLoginRecordById(long Id);

        void SaveRecord(User entity);

        void DeleteRecord(long Id);

        bool isEmailExists(long Id, string email);

        bool isMobileExists(long Id, string mobile);

        bool isPasswordExists(long Id, string password);

        bool isMacAddressExists(string macAddress, long Id);

        IEnumerable<User> GetRecordByStaff(int id);

        IEnumerable<User> GetUserByMacAddress(string macAddress, long Id);
    }
}