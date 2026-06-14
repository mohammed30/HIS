$connectionString = "Data Source=SQL1003.site4now.net;Initial Catalog=db_aca183_his;User Id=db_aca183_his_admin;Password=Oldlazy@123;Encrypt=True;TrustServerCertificate=True;"
$query = @"
DELETE FROM AppJournalEntryLines 
WHERE JournalEntryId IN (
    SELECT Id FROM AppJournalEntries WHERE ReferenceNumber LIKE 'DASH-JE-%'
);

DELETE FROM AppJournalEntries 
WHERE ReferenceNumber LIKE 'DASH-JE-%';
"@

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $rowsAffected = $command.ExecuteNonQuery()
    Write-Host "Success! Deleted test data rows. Rows affected: $rowsAffected"
    $connection.Close()
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
