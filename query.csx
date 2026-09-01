#r "nuget: Microsoft.Data.SqlClient, 5.1.1"
using System;
using Microsoft.Data.SqlClient;

var connString = "Data Source=SQL1002.site4now.net;Initial Catalog=db_acc373_asiaback20;User Id=db_acc373_asiaback20_admin;Password=Oldlazy@123;Encrypt=True;TrustServerCertificate=True;";
using var conn = new SqlConnection(connString);
conn.Open();

Console.WriteLine("--- Migrations History ---");
using var cmd = new SqlCommand("SELECT MigrationId FROM __EFMigrationsHistory", conn);
using var reader = cmd.ExecuteReader();
while (reader.Read()) {
    Console.WriteLine(reader.GetString(0));
}
reader.Close();

Console.WriteLine("\n--- Tables ---");
using var cmd2 = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%Lab%'", conn);
using var reader2 = cmd2.ExecuteReader();
while (reader2.Read()) {
    Console.WriteLine(reader2.GetString(0));
}
reader2.Close();

Console.WriteLine("\n--- AppLabTests Columns ---");
using var cmd3 = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppLabTests'", conn);
using var reader3 = cmd3.ExecuteReader();
while (reader3.Read()) {
    Console.WriteLine(reader3.GetString(0));
}
