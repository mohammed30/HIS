$conn = "Server=.;Database=HIS;Trusted_Connection=True;TrustServerCertificate=true"

# Categories Extraction SQL
$sqlCat = @"
SET NOCOUNT ON;
SELECT 'IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = ''' + CAST(Id AS varchar(40)) + ''') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES (''' + CAST(Id AS varchar(40)) + ''', ''' + REPLACE(Code, '''', '''''') + ''', ''' + REPLACE(Name, '''', '''''') + ''', ' + ISNULL('''' + CAST(ParentId AS varchar(40)) + '''', 'NULL') + ', ' + CAST(SortOrder AS varchar) + ', ' + CAST(CAST(IsActive AS int) AS varchar) + ', ''' + ExtraProperties + ''', ''' + ConcurrencyStamp + ''', ''' + CONVERT(varchar, CreationTime, 121) + ''');' 
FROM AppLabTestCategories
"@

# Tests Extraction SQL
$sqlTst = @"
SET NOCOUNT ON;
SELECT 'IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = ''' + CAST(Id AS varchar(40)) + ''') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES (''' + CAST(Id AS varchar(40)) + ''', ''' + REPLACE(Code, '''', '''''') + ''', ''' + REPLACE(Name, '''', '''''') + ''', ' + CAST(Price AS varchar) + ', ' + ISNULL('''' + REPLACE(Instructions, '''', '''''') + '''', 'NULL') + ', ' + ISNULL('''' + REPLACE(ReferenceRange, '''', '''''') + '''', 'NULL') + ', ' + ISNULL('''' + REPLACE(Unit, '''', '''''') + '''', 'NULL') + ', ' + CAST(CAST(IsActive AS int) AS varchar) + ', ''' + ExtraProperties + ''', ''' + ConcurrencyStamp + ''', ''' + CONVERT(varchar, CreationTime, 121) + ''', ' + ISNULL('''' + CAST(CategoryId AS varchar(40)) + '''', 'NULL') + ');' 
FROM AppLabTests
"@

$header = @("USE [db_ac621c_his];", "GO", "PRINT 'Starting Lab Data Migration...';", "GO", "-- Categories")
$footer = @("GO", "PRINT 'Lab Data Migration Completed Successfully.';", "GO")

Write-Host "Extracting data from HIS local database..."

$categoriesData = sqlcmd -S . -d HIS -E -Q $sqlCat -y 0
$testsData = sqlcmd -S . -d HIS -E -Q $sqlTst -y 0

$finalScript = @()
$finalScript += $header
foreach($line in $categoriesData) { if($line.Trim() -and $line -notmatch "rows affected") { $finalScript += $line } }
$finalScript += "-- Lab Tests"
foreach($line in $testsData) { if($line.Trim() -and $line -notmatch "rows affected") { $finalScript += $line } }
$finalScript += $footer

$finalScript | Out-File -FilePath "Migrate_Lab_Data.sql" -Encoding utf8
Write-Host "SUCCESS: Script generated at Migrate_Lab_Data.sql"
