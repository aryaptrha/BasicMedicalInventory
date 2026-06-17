using Microsoft.Data.SqlClient;
using InitialSetupMVC.Data;
using InitialSetupMVC.Models;
using System;
using System.Data;

namespace InitialSetupMVC.Repositories
{
    public class UserRepository
    {
        private readonly DbConnection _db;

        public UserRepository(DbConnection db)
        {
            _db = db;
        }

        public User? GetUserByEmail(string email)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetUserByEmail", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@Email", email.Trim());
            conn.Open();

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt64("Id"),
                    FullName = reader.GetString("FullName"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    RoleId = reader.GetInt64("RoleId"),
                    RoleName = reader.GetString("RoleName"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = DateTime.SpecifyKind(reader.GetDateTime("CreatedAt"), DateTimeKind.Utc)
                };
            }

            return null;
        }
    }
}
