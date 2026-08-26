using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HIS.EntityFrameworkCore;
using HIS.Accounting;

var optionsBuilder = new DbContextOptionsBuilder<HISDbContext>();
optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=HIS;Trusted_Connection=True;");
using (var context = new HISDbContext(optionsBuilder.Options))
{
    var accounts = context.Accounts.ToList();
    Console.WriteLine("Total Accounts: " + accounts.Count);
    foreach (var acc in accounts)
    {
        Console.WriteLine(acc.Code + " - " + acc.NameAr + " - Active: " + acc.IsActive);
    }
}
