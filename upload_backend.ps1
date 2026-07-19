$ftpHost = "ftp://win1135.site4now.net/site1"
$user = "asiahisbackend-001"
$pass = "Oldlazy@123"
$localPath = "C:\Users\Mohammed\source\repos\HIS\publish\backend"

function New-FtpDir ($ftpUri) {
    try {
        $request = [System.Net.FtpWebRequest]::Create($ftpUri)
        $request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
        $request.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
        $response = $request.GetResponse()
        $response.Close()
    } catch {}
}

function Get-FtpFileSize ($ftpUri) {
    try {
        $request = [System.Net.FtpWebRequest]::Create($ftpUri)
        $request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
        $request.Method = [System.Net.WebRequestMethods+Ftp]::GetFileSize
        $response = $request.GetResponse()
        $size = $response.ContentLength
        $response.Close()
        return $size
    } catch {
        return -1
    }
}

$files = Get-ChildItem -Path $localPath -Recurse
foreach ($item in $files) {
    $relPath = $item.FullName.Substring($localPath.Length + 1).Replace('\', '/')
    $ftpUri = "$ftpHost/$relPath"
    if ($item.PSIsContainer) {
        New-FtpDir $ftpUri
    } else {
        $remoteSize = Get-FtpFileSize $ftpUri
        if ($remoteSize -eq $item.Length) {
            Write-Host "Skipped existing library/file: $relPath"
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
