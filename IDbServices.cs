using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementApi.Model;
namespace UserManagementApi.DataAccess
{
    public interface IDbServices
    {
        public Task<IEnumerable<User>> GetUserList(int offset, int fetch);
        public Task<User> GetUser(int id);
        public Task<bool> CreateUser(User newUser);
        public Task<bool> DeleteUser(int id);
        public Task<IEnumerable<User>> SearchUser(string keyword);
        public Task<bool> UpdateUserDetails(User updateUser);
    }
}
