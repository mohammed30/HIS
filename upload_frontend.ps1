$ftpHost = "ftp://win8139.site4now.net:21/site1"
$user = "asiahosp-001"
$pass = "Oldlazy@123"
$localPath = "C:\Users\Mohammed\source\repos\HIS\src\angular\dist\HIS\browser"
$zipPath = "C:\Users\Mohammed\source\repos\HIS\src\angular\dist\HIS\browser.zip"

Write-Host "Compressing frontend files into a single ZIP archive to prevent FTP connection blocks..."
Compress-Archive -Path "$localPath\*" -DestinationPath $zipPath -Force

$ftpUri = "$ftpHost/browser.zip"

Write-Host "Uploading browser.zip to the server..."
try {
    $request = [System.Net.FtpWebRequest]::Create($ftpUri)
    $request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
    $request.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
    $request.KeepAlive = $false
    $request.UsePassive = $true
    $request.UseBinary = $true
    
    $content = [System.IO.File]::ReadAllBytes($zipPath)
    $request.ContentLength = $content.Length
    
    $requestStream = $request.GetRequestStream()
    $requestStream.Write($content, 0, $content.Length)
    $requestStream.Close()
    
    $response = $request.GetResponse()
    $response.Close()
    Write-Host "Upload completed successfully! You can now extract browser.zip from your hosting control panel."
} catch {
    Write-Host "Failed to upload browser.zip : $_"
}

