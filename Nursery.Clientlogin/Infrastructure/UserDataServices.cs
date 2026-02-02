using System.Text.Json;
using Nursery.Clientlogin.Models;


namespace Nursery.Clientlogin.Infrastructure
{
    public static class UserDataServices
    {
        private static readonly string UsersFile = DataPaths.UsersFile;

        public static void Initialize()
        {
            if (!Directory.Exists(DataPaths.DataDirectory))
                Directory.CreateDirectory(DataPaths.DataDirectory);
            
            if (!File.Exists(UsersFile) || new FileInfo(UsersFile).Length == 0)
                File.WriteAllText(UsersFile, "[]");
        }

        public static List<User> LoadUsers()
        {
            if (!File.Exists(UsersFile))
                return new List<User>();
            
            string json = File.ReadAllText(UsersFile);

            if (string.IsNullOrWhiteSpace(json))
                return new List<User>();

            try
            {
                return JsonSerializer.Deserialize<List<User>>(json)
                       ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public static void SaveUsers(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true 
            });
            System.IO.File.WriteAllText(UsersFile, json);
        }
    }
}