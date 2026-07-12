$ftpHost = "ftp://win1135.site4now.net/site1"
$user = "asiahisbackend-001"
$pass = "Oldlazy@123"
$localPath = "C:\Users\Mohammed\source\repos\HIS\src\publish_backend"

function New-FtpDir ($ftpUri) {
    try {
        $request = [System.Net.FtpWebRequest]::Create($ftpUri)
        $request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
        $request.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
        $response = $request.GetResponse()
        $response.Close()
    } catch {}
}

$files = Get-ChildItem -Path $localPath -Recurse
foreach ($item in $files) {
    $relPath = $item.FullName.Substring($localPath.Length + 1).Replace('\', '/')
    $ftpUri = "$ftpHost/$relPath"
    if ($item.PSIsContainer) {
        New-FtpDir $ftpUri
    } else {
        if ($item.Extension -eq ".dll" -and -not $item.Name.StartsWith("HIS.")) {
            Write-Host "Skipped backend library: $($item.Name)"
            continue
        }

        try {
            $webclient = New-Object System.Net.WebClient
            $webclient.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
            $webclient.UploadFile($ftpUri, $item.FullName) | Out-Null
            Write-Host "Uploaded: $relPath"
        } catch {
            Write-Host "Failed to upload $relPath : $_"
        }
    }
}
Write-Host "Backend upload completed."
