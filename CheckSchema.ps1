
$connectionString = "Data Source=SQL1003.site4now.net;Initial Catalog=db_aca183_his;User Id=db_aca183_his_admin;Password=Oldlazy@123;Encrypt=True;TrustServerCertificate=True;"
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
