$connectionString = "Data Source=SQL1003.site4now.net;Initial Catalog=db_aca183_his;User Id=db_aca183_his_admin;Password=Oldlazy@123;Encrypt=True;TrustServerCertificate=True;"

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
