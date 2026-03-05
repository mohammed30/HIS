$dll = 'c:\Code30\HIS\src\src\publish\api-host\refs\Microsoft.AspNetCore.Identity.dll'
[Reflection.Assembly]::LoadFrom($dll)
$hasher = New-Object 'Microsoft.AspNetCore.Identity.PasswordHasher[System.Object]'
$hash = $hasher.HashPassword($null, 'adminstaff')
Write-Output $hash
