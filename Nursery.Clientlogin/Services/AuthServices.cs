using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Nursery.Clientlogin.Models;
using Nursery.Clientlogin.Common;
using Nursery.Clientlogin.Infrastructure;

namespace Nursery.Clientlogin.Services
{
    public class AuthServices
    {
        public static User Login(string username, string password)
        {
            UserDataServices.Initialize();
            var users =  UserDataServices.LoadUsers();
            
            var user = users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            
            if (user == null)
                throw new Exception("Wrong username or password!");
            
            string hashedInputPassword = HashPassword(password);
            
            if (user.PasswordHash != hashedInputPassword)
                throw new Exception("Wrong username or password!");
            
            return user;
        }
        
        public static User Register(string username, string password)
        {
            UserDataServices.Initialize();
            var users = UserDataServices.LoadUsers();

            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Username already exists!");
            
            string hashedPassword = HashPassword(password);
            
            var existingIds = users.Select(u => u.UserID);
            string userID = UserIDGenerator.Generate(existingIds);
            
            var newUser = new User(userID, username, hashedPassword, UserType.Standard);
            users.Add(newUser);
            
            UserDataServices.SaveUsers(users);
            UserPlantDataServices.InitializeUserPlants(userID);
            
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