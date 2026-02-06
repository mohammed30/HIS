-- NUCLEAR OPTION: Delete and Re-Insert HIS_App
-- This ensures there are no hidden characters, whitespace issues, or corruption.

BEGIN TRANSACTION;

-- 1. DELETE existing application
DELETE FROM [OpenIddictApplications] WHERE [ClientId] = 'HIS_App';

-- 2. INSERT fresh application with validated values
INSERT INTO [OpenIddictApplications] (
    [Id],
    [ClientId],
    [ClientType],
    [ConsentType],
    [DisplayName],
    [Permissions],
    [RedirectUris],
    [PostLogoutRedirectUris],
    [ClientUri],
    [Requirements],
    [ExtraProperties],
    [ConcurrencyStamp],
    [CreationTime],
    [IsDeleted]
) VALUES (
    NEWID(),
    'HIS_App',
    'public',
    'implicit',
    'HIS Frontend',
    '["ept:end_session","gt:authorization_code","rst:code","ept:authorization","ept:token","ept:revocation","ept:introspection","gt:password","gt:client_credentials","gt:refresh_token","gt:LinkLogin","gt:Impersonation","scp:address","scp:email","scp:phone","scp:profile","scp:roles","scp:HIS","scp:offline_access"]',
    '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]',
    '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]',
    'https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net',
    '[]',
    '{}',
    NEWID(),
    GETDATE(),
    0
);

COMMIT TRANSACTION;

PRINT 'HIS_App has been reset successfully.';
