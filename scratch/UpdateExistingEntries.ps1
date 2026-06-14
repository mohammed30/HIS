$connectionString = "Data Source=SQL1003.site4now.net;Initial Catalog=db_aca183_his;User Id=db_aca183_his_admin;Password=Oldlazy@123;Encrypt=True;TrustServerCertificate=True;"
$query = @"
DECLARE @search NVARCHAR(100) = NCHAR(1593)+NCHAR(1604)+NCHAR(1575)+NCHAR(1580); -- "علاج"

-- 1. Update existing AppInvoiceItems for Physical Therapy
UPDATE AppInvoiceItems
SET DepartmentId = '6edab871-d2a6-bfd3-0d0c-3a21c87debfd'
WHERE Description LIKE '%' + @search + '%'
  AND DepartmentId IS NULL;

-- 2. Update existing AppJournalEntryLines for the corresponding invoices
UPDATE l
SET l.CostCenterId = '9b8cdd56-e618-df55-0f8b-3a21c87debe3'
FROM AppJournalEntryLines l
JOIN AppJournalEntries e ON l.JournalEntryId = e.Id
JOIN AppInvoices i ON e.ReferenceNumber = i.InvoiceNumber
JOIN AppInvoiceItems item ON i.Id = item.InvoiceId
JOIN AppAccounts a ON l.AccountId = a.Id
WHERE item.Description LIKE '%' + @search + '%'
  AND a.Code LIKE '41%'
  AND l.CostCenterId IS NULL;
"@

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $rowsAffected = $command.ExecuteNonQuery()
    Write-Host "Success! Updated existing transactions. Rows affected: $rowsAffected"
    $connection.Close()
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
