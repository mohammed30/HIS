
$connectionString = "Server=tcp:sql-asiahospital-we.database.windows.net,1433;Initial Catalog=HISDB;Persist Security Info=False;User ID=asiahospitaladmin;Password=Server@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$query = "SELECT TOP 1 * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AppProfessions'; SELECT COUNT(*) FROM AppProfessions;"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $reader = $command.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "Table AppProfessions EXISTS."
    }
    else {
        Write-Host "Table AppProfessions DOES NOT EXIST."
    }
    
    if ($reader.NextResult() -and $reader.Read()) {
        Write-Host "Count: $($reader[0])"
    }
    
    $connection.Close()
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
