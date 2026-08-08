$ftpHost = "ftp://WIN8139.site4now.net:21/site1"
$user = "asiahosp-001"
$pass = "Oldlazy@123"
$localPath = "C:\Users\Mohammed\source\repos\HIS\src\angular\dist\HIS\browser"

# Function to create directory on FTP
function New-FtpDirectory ($uri) {
    try {
        $request = [System.Net.FtpWebRequest]::Create($uri)
        $request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
        $request.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
        $response = $request.GetResponse()
        $response.Close()
    } catch {
        # Directory might already exist, ignore
    }
}

$items = Get-ChildItem -Path $localPath -Recurse

Write-Host "Starting uncompressed upload of $($items.Count) items..."

foreach ($item in $items) {
    $relativePath = $item.FullName.Substring($localPath.Length + 1).Replace('\', '/')
    $ftpUri = "$ftpHost/$relativePath"
    
    if ($item.PSIsContainer) {
        Write-Host "Creating directory: $relativePath"
        New-FtpDirectory $ftpUri
    } else {
        Write-Host "Uploading file: $relativePath"
        try {
            $request = [System.Net.FtpWebRequest]::Create($ftpUri)
            $request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
            $request.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
            $request.UseBinary = $true
            $request.KeepAlive = $true
            
            $content = [System.IO.File]::ReadAllBytes($item.FullName)
            $request.ContentLength = $content.Length
            
            $requestStream = $request.GetRequestStream()
            $requestStream.Write($content, 0, $content.Length)
            $requestStream.Close()
            
            $response = $request.GetResponse()
            $response.Close()
        } catch {
            Write-Host "Failed to upload $relativePath : $_"
        }
    }
}
Write-Host "Upload completed successfully!"
