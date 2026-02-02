using System;
using System.IO;

namespace Nursery.Clientlogin.Infrastructure;

public static class DataPaths
{
    public static readonly string DataDirectory =
        Path.Combine(AppContext.BaseDirectory, "Data");
    
    public static readonly string UsersFile =
        Path.Combine(DataDirectory, "Users.json");
}