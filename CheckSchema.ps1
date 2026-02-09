
$connectionString = "Server=tcp:sql-asiahospital-we.database.windows.net,1433;Initial Catalog=HISDB;Persist Security Info=False;User ID=asiahospitaladmin;Password=Server@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AppProfessions'"
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "Schema: $($reader['TABLE_SCHEMA']) Table: $($reader['TABLE_NAME'])"
    }
    $connection.Close()
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
