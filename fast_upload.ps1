$user = "asiaback2000-001"
$pass = "Oldlazy@123"
$ftpHost = "ftp://win1035.site4now.net/site1"

$webclient = New-Object System.Net.WebClient
$webclient.Credentials = New-Object System.Net.NetworkCredential($user, $pass)

$files = @(
    "c:\Users\Mohammed\source\repos\HIS\publish\backend\HIS.Application.dll",
    "c:\Users\Mohammed\source\repos\HIS\publish\backend\HIS.Application.Contracts.dll",
    "c:\Users\Mohammed\source\repos\HIS\publish\backend\HIS.Domain.Shared.dll",
    "c:\Users\Mohammed\source\repos\HIS\publish\backend\web.config"
)

# Create a dummy app_offline.htm locally
$offlineFile = "c:\Users\Mohammed\source\repos\HIS\publish\backend\app_offline.htm"
"Taking app offline" | Out-File $offlineFile

# Upload app_offline.htm
Write-Host "Taking app offline..."
$offlineUri = "$ftpHost/app_offline.htm"
$webclient.UploadFile($offlineUri, $offlineFile) | Out-Null
Start-Sleep -Seconds 5
Start-Sleep -Seconds 3

# Upload files
foreach ($file in $files) {
    $fileName = Split-Path $file -Leaf
    $ftpUri = "$ftpHost/$fileName"
    Write-Host "Uploading $fileName..."
    $webclient.UploadFile($ftpUri, $file) | Out-Null
}

# Delete app_offline.htm
Write-Host "Bringing app online..."
$request = [System.Net.FtpWebRequest]::Create("$ftpHost/app_offline.htm")
$request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
$request.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile
$request.GetResponse().Close() | Out-Null

Write-Host "Done!"
