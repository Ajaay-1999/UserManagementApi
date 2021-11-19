using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using UserManagementApi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace UserManagementApi.DataAccess
{
    public class DbContext : IDbServices
    {
        private static string connectionString = @"data source=eng.learntron.net;initial catalog=Skilltron_Dev;user id=sa;password=LearnTron2015;MultipleActiveResultSets=True";
        public async Task<IEnumerable<User>> GetUserList(int offset, int fetch)
        {
            var connection = GetConnection();
            connection.Open();
            string sql = @"Select User_Id,User_Name,First_Name,Last_Name,Email
                           from [Skilltron_Dev].[dbo].[Users]
                           order by User_Id offset @offset rows fetch next @fetch rows only";
            var userList = await connection.QueryAsync<User>(sql, new { offset = offset, fetch = fetch });
            connection.Close();
            if (userList != null)
                return userList;
            return null;
        }
        public async Task<User> GetUser(int userId)
        {
            var connection = GetConnection();
            connection.Open();
            string sql = @"Select User_Id,User_Name,First_Name,Last_Name,Email
                           from [Skilltron_Dev].[dbo].[Users] where User_Id = @userId";
            var selectedUser = await connection.QueryFirstOrDefaultAsync<User>(sql, new { userId = userId });
            connection.Close();
            return selectedUser;
        }
        public async Task<bool> DeleteUser(int userId)
        {
            var connection = GetConnection();
            connection.Open();
            var isUserExist = await connection.ExecuteScalarAsync<int>("Select count(User_ID) from [Skilltron_Dev].[dbo].[Users] where User_Id = @userId", new { userId = userId });
            if (isUserExist == 0)
                return false;
            else
            {
                string sql = "Delete from [Skilltron_Dev].[dbo].[Users] where User_Id = @userId";
                var res = await connection.ExecuteAsync(sql, new { userId = userId });
                connection.Close();
                if (res > 0)
                    return true;
                return false;
            }
            
        }
        public async Task<bool> CreateUser(User newUser)
        {
            var connection = GetConnection();
            connection.Open();
            var isUserExist = connection.ExecuteScalar<int>("Select count(User_ID) from [Skilltron_Dev].[dbo].[Users] where User_Id = @Id", new { Id = newUser.User_Id });
            if (isUserExist > 0)
                return false;
            else
            {
                string sql = @"insert into [Skilltron_Dev].[dbo].[Users]
                             (User_Name,First_Name,Last_Name,Email,Organization_Identifier,Role_Name)
                             values(@userName,@firstName,@lastName,@email,'Dev','QA')";
                await connection.ExecuteAsync(sql, new {userName = newUser.User_Name, firstName = newUser.First_Name, lastName = newUser.Last_Name, email = newUser.Email });
                connection.Close();
                return true;
            }
            
        }
        public async Task<IEnumerable<User>> SearchUser(string keyword)
        {
            string sql = @"SELECT User_Id,User_Name,First_Name,Last_Name,Email
                          from [Skilltron_Dev].[dbo].[Users] where (User_Name like '%'+@searchText+'%')
                          or (First_Name like '%'+@searchText+'%')";
            var connection = GetConnection();
            connection.Open();
            var result = await connection.QueryAsync<User>(sql, new { searchText = keyword});
            connection.Close();           
            return result;
        }
        public async Task<bool> UpdateUserDetails(User updateUser)
        {
            var connection = GetConnection();
            connection.Open();
            var isUserExist = await connection.ExecuteScalarAsync<int>("Select count(User_ID) from [Skilltron_Dev].[dbo].[Users] where User_Id = @userId", new { userId = updateUser.User_Id });
            if (isUserExist == 0)
                return false;
            else
            {
                string sql = GetQueryString(updateUser);
                await connection.ExecuteAsync(sql, new { userId = updateUser.User_Id, userName = updateUser.User_Name, firstName = updateUser.First_Name, lastName = updateUser.Last_Name, email = updateUser.Email });
                connection.Close();
                return true;
            }
        }
        public static string GetQueryString(User updateUser)
        {
            StringBuilder updateColumns = new StringBuilder();
            StringBuilder sqlQuery = new StringBuilder();
            if (!string.IsNullOrEmpty(updateUser.User_Name))
                updateColumns.AppendLine("User_Name = @userName");
            if (!string.IsNullOrEmpty(updateUser.First_Name))
            {
                if (updateColumns.Length > 0)
                    updateColumns.Append(",");
                updateColumns.AppendLine("First_Name = @firstName");
            }
            if (!string.IsNullOrEmpty(updateUser.Last_Name))
            {
                if (updateColumns.Length > 0)
                    updateColumns.Append(",");
                updateColumns.AppendLine("Last_Name = @lastName");
            }
            if (!string.IsNullOrEmpty(updateUser.Email))
            {
                if (updateColumns.Length > 0)
                    updateColumns.Append(",");
                updateColumns.AppendLine("Email = @email");
            }
            sqlQuery.AppendLine("update [Skilltron_Dev].[dbo].[Users] set");
            sqlQuery.AppendLine(updateColumns.ToString());
            sqlQuery.AppendLine("where User_Id = @userId");
            return sqlQuery.ToString();
        }
        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(connectionString);
            return connection;
        }
    }
}
