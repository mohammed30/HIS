$user = "asiahisbackend-001"
$pass = "Oldlazy@123"
$ftpHost = "ftp://win1135.site4now.net/site1"

$webclient = New-Object System.Net.WebClient
$webclient.Credentials = New-Object System.Net.NetworkCredential($user, $pass)

Write-Host "Downloading appsettings.json..."
try {
    $webclient.DownloadFile("$ftpHost/appsettings.json", "c:\Users\Mohammed\source\repos\HIS\appsettings_ftp.json")
    Write-Host "Downloaded appsettings.json"
} catch {
    Write-Host "Failed to download appsettings.json: $_"
}

Write-Host "Downloading appsettings.Production.json..."
try {
    $webclient.DownloadFile("$ftpHost/appsettings.Production.json", "c:\Users\Mohammed\source\repos\HIS\appsettings.Production_ftp.json")
    Write-Host "Downloaded appsettings.Production.json"
} catch {
    Write-Host "Failed to download appsettings.Production.json: $_"
}
