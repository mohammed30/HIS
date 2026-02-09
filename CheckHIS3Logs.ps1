
$connectionString = "Server=tcp:sql-asiahospital-we.database.windows.net,1433;Initial Catalog=HIS3;Persist Security Info=False;User ID=asiahospitaladmin;Password=Server@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT TOP 1 Message FROM Logs WHERE Level = 'Error' ORDER BY TimeStamp DESC"
    $msg = $command.ExecuteScalar()
    Write-Host "Last Error in HIS3: $msg"
    $connection.Close()
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
