using Nursery.Core.Common;

namespace Nursery.Core.Models;

public class User
{
    public string UserID { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public UserType  Role { get; set; }

    public User(string userID, string username, string passwordHash, UserType role = UserType.Standard)
    {
        UserID = userID;
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}