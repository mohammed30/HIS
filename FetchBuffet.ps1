$connectionString = "Server=.;Database=db_ac621c_his;Trusted_Connection=True;TrustServerCertificate=true"
$SqlConnection = New-Object System.Data.SqlClient.SqlConnection
$SqlConnection.ConnectionString = $connectionString
$SqlConnection.Open()

$query = "SELECT Id, Code, Name, Category, IsActive, Price, Unit, ReferenceRange, Instructions, ExtraProperties, ConcurrencyStamp, CreationTime FROM AppServiceItems WHERE Name LIKE N'%بوفيه%' OR Name LIKE N'%نثريات%'"
$SqlCmd = New-Object System.Data.SqlClient.SqlCommand
$SqlCmd.CommandText = $query
$SqlCmd.Connection = $SqlConnection

$SqlAdapter = New-Object System.Data.SqlClient.SqlDataAdapter
$SqlAdapter.SelectCommand = $SqlCmd
$DataSet = New-Object System.Data.DataSet
$SqlAdapter.Fill($DataSet)
$SqlConnection.Close()

$rows = $DataSet.Tables[0].Rows

$outputFile = "c:\Users\Mohammed\source\repos\mohammed30\HIS\Migrate_Buffet_Data.sql"
"USE [db_ac621c_his];" | Out-File -FilePath $outputFile -Encoding utf8
"GO" | Out-File -FilePath $outputFile -Encoding utf8 -Append
"PRINT 'Migrating Buffet and Sundries (Service Items)...';" | Out-File -FilePath $outputFile -Encoding utf8 -Append

if ($rows.Count -eq 0) {
    Write-Host "Warning: No records found for Buffet/Sundries in AppServiceItems."
} else {
    foreach ($row in $rows) {
        $Id = $row["Id"]
        $Code = $row["Code"]
        $Name = $row["Name"].Replace("'", "''")
        $Category = $row["Category"]
        $IsActive = if ($row["IsActive"]) { 1 } else { 0 }
        $Price = $row["Price"]
        $Unit = if ([string]::IsNullOrEmpty($row["Unit"].ToString())) { "NULL" } else { "N'" + $row["Unit"].ToString().Replace("'", "''") + "'" }
        $ReferenceRange = if ([string]::IsNullOrEmpty($row["ReferenceRange"].ToString())) { "NULL" } else { "N'" + $row["ReferenceRange"].ToString().Replace("'", "''") + "'" }
        $Instructions = if ([string]::IsNullOrEmpty($row["Instructions"].ToString())) { "NULL" } else { "N'" + $row["Instructions"].ToString().Replace("'", "''") + "'" }
        $ExtraProperties = "'" + $row["ExtraProperties"].Replace("'", "''") + "'"
        $ConcurrencyStamp = "'" + $row["ConcurrencyStamp"].Replace("'", "''") + "'"

        $sql = "IF NOT EXISTS (SELECT 1 FROM AppServiceItems WHERE Id = '$Id')
INSERT INTO AppServiceItems (Id, Code, Name, Category, IsActive, Price, Unit, ReferenceRange, Instructions, ExtraProperties, ConcurrencyStamp, CreationTime, IsDeleted, Discriminator)
VALUES ('$Id', '$Code', N'$Name', $Category, $IsActive, $Price, $Unit, $ReferenceRange, $Instructions, $ExtraProperties, $ConcurrencyStamp, GETDATE(), 0, 'ServiceItem');"
        
        $sql | Out-File -FilePath $outputFile -Encoding utf8 -Append
    }
}
"GO" | Out-File -FilePath $outputFile -Encoding utf8 -Append
Write-Host "SUCCESS: Generated Migrate_Buffet_Data.sql with $($rows.Count) records."
