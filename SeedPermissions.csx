using System;
using System.Data.SqlClient;

string connStr = "Server=(LocalDb)\\MSSQLLocalDB;Database=HIS;Trusted_Connection=True;";
string[] permissions = { "HIS.Billing.ReceiptVouchers.Cancel", "HIS.Billing.PaymentVouchers.Cancel" };
string[] roles = { "admin", "admin_staff" };

using (var conn = new SqlConnection(connStr))
{
    conn.Open();
    foreach (var role in roles)
    {
        foreach (var perm in permissions)
        {
            var cmd = new SqlCommand("IF NOT EXISTS (SELECT 1 FROM AbpPermissionGrants WHERE Name = @name AND ProviderName = 'R' AND ProviderKey = @key) INSERT INTO AbpPermissionGrants (Id, TenantId, Name, ProviderName, ProviderKey) VALUES (NEWID(), NULL, @name, 'R', @key)", conn);
            cmd.Parameters.AddWithValue("@name", perm);
            cmd.Parameters.AddWithValue("@key", role);
            cmd.ExecuteNonQuery();
        }
    }
    Console.WriteLine("Permissions seeded successfully.");
}
