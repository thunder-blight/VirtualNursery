using System;
using System.Linq;

namespace Nursery.Clientlogin.Infrastructure;

public class UserIDGenerator
{
    private static readonly string chars = "0123456789";

    public static string Generate(IEnumerable<string> existingIDs)
    {
        var random = new Random();
        string id;

        do
        {
            id = new string(
                Enumerable.Range(0,4)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray()
                );
        }
        
        while (existingIDs.Contains(id));
        
        return id;
    }
}