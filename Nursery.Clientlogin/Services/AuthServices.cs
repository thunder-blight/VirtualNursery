using System.Security.Cryptography;
using System.Text;
using Nursery.Core.Models;
using Nursery.Core.Common;
using Nursery.Core.Infrastructure;

namespace Nursery.Clientlogin.Services
{
    public class AuthServices
    {
        public static User Login(string username, string password)
        {
            var user = UserDatabaseServices.GetUserByUsername(username);
            
            if (user == null)
                throw new Exception("Wrong username or password!");
            
            string hashedInputPassword = HashPassword(password);
            
            if (user.PasswordHash != hashedInputPassword)
                throw new Exception("Wrong username or password!");
            
            return user;
        }
        
        public static User Register(string username, string password)
        {
            if (UserDatabaseServices.UsernameExists(username))
                throw new Exception("Username already exists!");
            
            string hashedPassword = HashPassword(password);
            
            var existingIds = UserDatabaseServices.GetAllUserIDs();
            string userID = UserIDGenerator.Generate(existingIds);
            
            var newUser = new User(userID, username, hashedPassword, UserType.Standard);
            UserDatabaseServices.SaveUser(newUser);
            
            return newUser;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}