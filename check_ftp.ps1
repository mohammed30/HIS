$user = "asiahosp-001"
$pass = "Oldlazy@123"
$uri = "ftp://WIN8139.site4now.net:21/"

$request = [System.Net.FtpWebRequest]::Create($uri)
$request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
$request.Method = [System.Net.WebRequestMethods+Ftp]::ListDirectory

$response = $request.GetResponse()
$reader = New-Object System.IO.StreamReader($response.GetResponseStream())
$output = $reader.ReadToEnd()
$reader.Close()
$response.Close()

Write-Host $output
