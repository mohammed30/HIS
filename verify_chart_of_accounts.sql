-- SQL Script to verify Chart of Accounts on Production
-- Run this script on BOTH your Local Database and Azure Database to compare the results.

SELECT 
    Code,
    Name,
    NameAr,
    Type,
    ParentId,
    IsActive
FROM 
    AppAccounts
ORDER BY 
    Code;

-- You can also check the total count to see if there is a mismatch in the number of records
SELECT COUNT(*) AS TotalAccounts FROM AppAccounts;
