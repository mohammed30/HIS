
$connBase = "Server=tcp:sql-asiahospital-we.database.windows.net,1433;Persist Security Info=False;User ID=asiahospitaladmin;Password=Server@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

function CheckTable($dbName) {
    $connectionString = $connBase + "Initial Catalog=" + $dbName + ";"
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AppProfessions'"
        $count = $command.ExecuteScalar()
        Write-Host "Database: $dbName - Table AppProfessions Count: $count"
        $connection.Close()
    }
    catch {
        Write-Host "Database: $dbName - Error: $($_.Exception.Message)"
    }
}

CheckTable "HISDB"
CheckTable "HIS3"
