$connectionString = "Server=tcp:sql-asiahospital-we.database.windows.net,1433;Initial Catalog=HIS3;Persist Security Info=False;User ID=asiahospitaladmin;Password=Server@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

Write-Host "Testing connection to Azure SQL..." -ForegroundColor Cyan

try {
    $conn = New-Object System.Data.SqlClient.SqlConnection
    $conn.ConnectionString = $connectionString
    $conn.Open()
    Write-Host "✅ Connection Successful!" -ForegroundColor Green
    $conn.Close()
}
catch {
    Write-Host "❌ Connection Failed!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
