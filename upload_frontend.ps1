$ftpHost = "ftp://WIN8139.site4now.net:21/site1"
$user = "asiahosp-001"
$pass = "Oldlazy@123"
$localPath = "C:\Users\Mohammed\source\repos\HIS\src\angular\dist\HIS\browser"
$stateFile = "C:\Users\Mohammed\source\repos\HIS\src\angular\dist\upload_state.json"

# Load previous upload state
$uploadState = @{}
if (Test-Path $stateFile) {
    try {
        $uploadState = Get-Content $stateFile -Raw | ConvertFrom-Json -AsHashtable
    } catch {
        $uploadState = @{}
    }
}

# Function to get MD5 hash of a file
function Get-FileHashMD5 ($filePath) {
    $stream = [System.IO.File]::OpenRead($filePath)
    $md5 = [System.Security.Cryptography.MD5]::Create()
    $hash = [BitConverter]::ToString($md5.ComputeHash($stream)).Replace("-", "").ToLower()
    $stream.Close()
    $md5.Dispose()
    return $hash
}

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
        # Directories are cheap, we can just try to create them (or check cache)
        if (-not $uploadState.ContainsKey($relativePath)) {
            Write-Host "Creating directory: $relativePath"
            New-FtpDirectory $ftpUri
            $uploadState[$relativePath] = "dir"
        }
    } else {
        $currentHash = Get-FileHashMD5 $item.FullName
        
        if ($uploadState.ContainsKey($relativePath) -and $uploadState[$relativePath] -eq $currentHash) {
            Write-Host "Skipping unchanged file: $relativePath"
            continue
        }

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
            
            # Save hash on success
            $uploadState[$relativePath] = $currentHash
        } catch {
            Write-Host "Failed to upload $relativePath : $_"
        }
    }
}

# Save the updated state to avoid re-uploading next time
$uploadState | ConvertTo-Json | Set-Content $stateFile

Write-Host "Upload completed successfully!"
