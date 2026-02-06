-- ADD MISSING 'offline_access' SCOPE
-- This scope MUST exist in the OpenIddictScopes table for the OAuth flow to work.

IF NOT EXISTS (SELECT 1 FROM [OpenIddictScopes] WHERE [Name] = 'offline_access')
BEGIN
    INSERT INTO [OpenIddictScopes] (
        [Id],
        [Name],
        [DisplayName],
        [Resources],
        [ExtraProperties],
        [ConcurrencyStamp],
        [CreationTime],
        [IsDeleted]
    )
    VALUES (
        NEWID(),
        'offline_access',
        'Offline Access',
        '[]',
        '{}',
        NEWID(),
        GETDATE(),
        0
    );
    PRINT 'SUCCESS: offline_access scope has been added.'
END
ELSE
BEGIN
    PRINT 'INFO: offline_access scope already exists.'
END

-- VERIFY
SELECT [Name], [DisplayName] FROM [OpenIddictScopes];
