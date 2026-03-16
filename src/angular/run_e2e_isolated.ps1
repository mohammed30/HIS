# PowerShell Script to run E2E tests in isolated sequential order with warming
$ErrorActionPreference = "Stop"

$frontendUrl = "http://localhost:4200"
$backendUrl = "https://localhost:44382"
$testDir = "./e2e"

Write-Host "--- Starting E2E Isolated Run ---" -ForegroundColor Cyan

# 1. Warm up server
Write-Host "Warming up servers..." -ForegroundColor Yellow
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy

try {
    Invoke-WebRequest -Uri $frontendUrl -UseBasicParsing -TimeoutSec 30 | Out-Null
    Invoke-WebRequest -Uri $backendUrl -UseBasicParsing -TimeoutSec 30 | Out-Null
    Write-Host "Servers are responsive." -ForegroundColor Green
} catch {
    Write-Host "Warning: Servers might still be loading or unreachable. Error: $($_.Exception.Message)" -ForegroundColor Red
}

# 2. Sequential Test Execution
$specs = Get-ChildItem -Path $testDir -Filter "*.spec.ts"

foreach ($spec in $specs) {
    Write-Host "`n>>> Running: $($spec.Name)" -ForegroundColor Cyan
    $name = $spec.Name
    
    $startTime = Get-Date
    npx playwright test "e2e/$name" --workers=1
    $endTime = Get-Date
    
    $duration = ($endTime - $startTime).TotalSeconds
    Write-Host "<<< Finished $name in $($duration)s" -ForegroundColor Green
}

Write-Host "`n--- E2E Isolated Run Complete ---" -ForegroundColor Cyan
