
$connectionString = "Server=tcp:sql-asiahospital-we.database.windows.net,1433;Initial Catalog=HISDB;Persist Security Info=False;User ID=asiahospitaladmin;Password=Server@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$query = "SELECT TOP 10 Message, Exception, Level, TimeStamp FROM Logs WHERE Message LIKE '%Invalid object name%' OR Exception LIKE '%Invalid object name%' ORDER BY TimeStamp DESC"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $reader = $command.ExecuteReader()
    
    while ($reader.Read()) {
        Write-Host "-------------------"
        Write-Host "Time: $($reader['TimeStamp'])"
        Write-Host "Level: $($reader['Level'])"
        Write-Host "Message: $($reader['Message'])"
        Write-Host "Exception: $($reader['Exception'])"
    }
    
    $connection.Close()
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
