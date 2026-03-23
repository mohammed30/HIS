# ==============================================================================
# Pharmacy & Inventory Data Transfer Script
# Transfers data safely from Local DB to Production DB using SqlBulkCopy & MERGE
# ==============================================================================

$LocalConnStr = "Server=.;Database=HIS;Trusted_Connection=True;TrustServerCertificate=True"
$ProdConnStr = "Data Source=SQL8012.site4now.net;Initial Catalog=db_ac621c_his;User Id=db_ac621c_his_admin;Password=Server@123;Encrypt=True;TrustServerCertificate=True;"

$TablesToCopy = @(
    @{ Name = "AppWarehouses"; Query = "SELECT * FROM AppWarehouses" },
    @{ Name = "AppSuppliers"; Query = "SELECT * FROM AppSuppliers" },
    @{ Name = "AppServiceItems"; Query = "SELECT * FROM AppServiceItems WHERE Id IN (SELECT ServiceItemId FROM AppDrugs WHERE ServiceItemId IS NOT NULL)" },
    @{ Name = "AppDrugs"; Query = "SELECT * FROM AppDrugs" },
    @{ Name = "AppInventoryItems"; Query = "SELECT * FROM AppInventoryItems" },
    @{ Name = "AppInventoryBatches"; Query = "SELECT * FROM AppInventoryBatches" }
)

Write-Host "Starting Pharmacy Data Transfer Process..." -ForegroundColor Cyan
Write-Host "Local DB: Server=.;Database=HIS" -ForegroundColor Yellow
Write-Host "Prod DB : SQL8012.site4now.net" -ForegroundColor Yellow
Write-Host "------------------------------------------------------"

try {
    $localConn = New-Object System.Data.SqlClient.SqlConnection($LocalConnStr)
    $prodConn = New-Object System.Data.SqlClient.SqlConnection($ProdConnStr)
    
    $localConn.Open()
    $prodConn.Open()

    foreach ($table in $TablesToCopy) {
        $tableName = $table.Name
        $query = $table.Query
        
        Write-Host "=> Exporting table [$tableName] from Local..." -NoNewline
        
        # 1. Read Data From Local
        $localCmd = New-Object System.Data.SqlClient.SqlCommand($query, $localConn)
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($localCmd)
        $dt = New-Object System.Data.DataTable
        $adapter.Fill($dt) | Out-Null
        
        Write-Host " Found $($dt.Rows.Count) rows." -ForegroundColor Green
        
        if ($dt.Rows.Count -eq 0) {
            continue
        }

        # 2. Upload to Production via Temp Table and INSERT where missing
        $tx = $prodConn.BeginTransaction()
        try {
            $tempTable = "#Temp_$tableName"
            
            # Create matching temp table
            $createTempSql = "SELECT TOP 0 * INTO $tempTable FROM $tableName;"
            $createCmd = New-Object System.Data.SqlClient.SqlCommand($createTempSql, $prodConn, $tx)
            $createCmd.ExecuteNonQuery() | Out-Null
            
            # Bulk copy to temp table
            [int]$options = [System.Data.SqlClient.SqlBulkCopyOptions]::KeepIdentity -bor [System.Data.SqlClient.SqlBulkCopyOptions]::KeepNulls
            $bulkCopy = New-Object System.Data.SqlClient.SqlBulkCopy($prodConn, $options, $tx)
            $bulkCopy.DestinationTableName = $tempTable
            $bulkCopy.BatchSize = 1000
            
            # Map columns explicitly
            foreach ($col in $dt.Columns) {
                $bulkCopy.ColumnMappings.Add($col.ColumnName, $col.ColumnName) | Out-Null
            }
            
            $bulkCopy.WriteToServer($dt)
            
            # Merge (INSERT) from temp table to actual table
            $mergeSql = "
            DECLARE @cols NVARCHAR(MAX);
            SELECT @cols = STRING_AGG(QUOTENAME(C.COLUMN_NAME), ', ')
            FROM tempdb.INFORMATION_SCHEMA.COLUMNS C
            WHERE C.TABLE_NAME LIKE '$tempTable%' AND C.COLUMN_NAME != 'Id';

            DECLARE @sql NVARCHAR(MAX) = '
            INSERT INTO $tableName (Id, ' + @cols + ')
            SELECT tmp.Id, ' + @cols + ' 
            FROM $tempTable tmp
            WHERE NOT EXISTS (SELECT 1 FROM $tableName t WHERE t.Id = tmp.Id);
            ';
            EXEC sp_executesql @sql;
            "
            $mergeCmd = New-Object System.Data.SqlClient.SqlCommand($mergeSql, $prodConn, $tx)
            $rowsAffected = $mergeCmd.ExecuteNonQuery()
            
            $tx.Commit()
            Write-Host "   -> Successfully migrated $rowsAffected NEW rows to production $tableName." -ForegroundColor Green
            
        } catch {
            $tx.Rollback()
            Write-Host "   -> Error migrating $tableName : $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    Write-Host "------------------------------------------------------"
    Write-Host "Transfer process completed successfully!" -ForegroundColor Cyan
    
} catch {
    Write-Host "Critical Error: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    if ($localConn.State -eq 'Open') { $localConn.Close() }
    if ($prodConn.State -eq 'Open') { $prodConn.Close() }
}
