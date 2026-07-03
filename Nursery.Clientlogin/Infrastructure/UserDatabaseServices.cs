using Microsoft.Data.Sqlite;
using Nursery.Clientlogin.Common;
using Nursery.Clientlogin.Models;

namespace Nursery.Clientlogin.Infrastructure
{
    public static class UserDatabaseServices
    {
        private static readonly string ConnectionString =
            $"Data Source={DataPaths.NurseryDbFile}";
        
        public static void SaveUser(User user)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (UserID, Username, PasswordHash, Role)
                VALUES ($userId, $username, $passwordHash, $role);
            ";
            command.Parameters.AddWithValue("$userId", user.UserID);
            command.Parameters.AddWithValue("$username", user.Username);
            command.Parameters.AddWithValue("$passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("$role", user.Role.ToString());

            command.ExecuteNonQuery();
        }

        public static User? GetUserByUsername(string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT UserID, Username, PasswordHash, Role
                FROM Users
                WHERE Username = $username;
            ";
            command.Parameters.AddWithValue("$username", username);
            
            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return null;
            
            string userId = reader.GetString(0);
            string uname = reader.GetString(1);
            string passwordHash = reader.GetString(2);
            var role = Enum.Parse<UserType>(reader.GetString(3));
            
            return new User(userId, uname, passwordHash, role);
        }

        public static bool UsernameExists(string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = $username;";
            command.Parameters.AddWithValue("$username", username);
            
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public static List<string> GetAllUserIDs()
        {
            var ids = new List<string>();
            
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT UserID FROM Users;";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
                ids.Add(reader.GetString(0));

            return ids;
        }
    }
}