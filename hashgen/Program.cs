using Microsoft.AspNetCore.Identity;
using System;

public class Program
{
    public static void Main()
    {
        var hasher = new PasswordHasher<object>();
        // Using null for user as it's common in simplified hashing or when user object doesn't affect salt (V3)
        string hash = hasher.HashPassword(new object(), "adminstaff");
        Console.WriteLine(hash);
    }
}
