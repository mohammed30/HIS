-- COMPREHENSIVE OPENIDDICT DIAGNOSTIC
-- Run this to see the full state of your OpenIddict configuration

-- 1. Check Applications (Clients)
PRINT '=== OPENIDDICT APPLICATIONS ==='
SELECT 
    [ClientId],
    [ClientType],
    [ConsentType],
    [Permissions],
    [RedirectUris],
    [PostLogoutRedirectUris]
FROM [OpenIddictApplications];

-- 2. Check Scopes (IMPORTANT - offline_access must be registered here too!)
PRINT '=== OPENIDDICT SCOPES ==='
SELECT 
    [Name],
    [DisplayName],
    [Resources]
FROM [OpenIddictScopes];

-- 3. Check if 'offline_access' scope exists
PRINT '=== CHECKING FOR OFFLINE_ACCESS SCOPE ==='
IF NOT EXISTS (SELECT 1 FROM [OpenIddictScopes] WHERE [Name] = 'offline_access')
BEGIN
    PRINT 'WARNING: offline_access scope is MISSING from OpenIddictScopes table!'
    PRINT 'This could be causing the 400 error.'
END
ELSE
BEGIN
    PRINT 'OK: offline_access scope exists.'
END

-- 4. Check recent logs (if any)
PRINT '=== RECENT ERROR LOGS ==='
SELECT TOP 10 
    [TimeStamp], 
    [Level], 
    [Message]
FROM [Logs]
WHERE [Level] = 'Error' OR [Level] = 'Warning'
ORDER BY [TimeStamp] DESC;
