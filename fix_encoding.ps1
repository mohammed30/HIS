$file = 'c:\Users\Mohammed\source\repos\HIS\src\src\HIS.HttpApi.Host\Pages\Account\Login.cshtml'
$bytes = [System.IO.File]::ReadAllBytes($file)
$wrongEncoding = [System.Text.Encoding]::GetEncoding('windows-1252')
$rightEncoding = [System.Text.Encoding]::UTF8
$text = $wrongEncoding.GetString($bytes)
$fixedText = $rightEncoding.GetString($rightEncoding.GetBytes($text))
[System.IO.File]::WriteAllText($file + '.check.txt', $text.Substring(9000, 500), [System.Text.Encoding]::UTF8)
