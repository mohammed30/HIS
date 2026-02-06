-- Helper to generate a new GUID if needed, though we insert checks.

-- 1. HIS_App (Frontend)
IF NOT EXISTS (SELECT 1 FROM [OpenIddictApplications] WHERE [ClientId] = 'HIS_App')
BEGIN
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
        [LogoUri],
        [ApplicationType],
        [JsonWebKeySet],
        [ExtraProperties],
        [ConcurrencyStamp],
        [CreationTime],
        [IsDeleted]
    )
    VALUES (
        NEWID(),
        'HIS_App',
        'public',
        'implicit',
        'Console Test / Angular Application',
        '["gt:authorization_code","gt:password","gt:client_credentials","gt:refresh_token","gt:LinkLogin","gt:Impersonation","sc:address","sc:email","sc:phone","sc:profile","sc:roles","sc:HIS","ept:authorization","ept:token","ept:logout"]',
        '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]',
        '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]',
        'https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net',
        '/images/clients/angular.svg',
        'web',
        NULL,
        '{}',
        NEWID(),
        GETDATE(),
        0
    );
END

-- 2. HIS_Swagger (Backend Docs)
IF NOT EXISTS (SELECT 1 FROM [OpenIddictApplications] WHERE [ClientId] = 'HIS_Swagger')
BEGIN
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
        [LogoUri],
        [ApplicationType],
        [JsonWebKeySet],
        [ExtraProperties],
        [ConcurrencyStamp],
        [CreationTime],
        [IsDeleted]
    )
    VALUES (
        NEWID(),
        'HIS_Swagger',
        'public',
        'implicit',
        'Swagger Application',
        '["gt:authorization_code","sc:address","sc:email","sc:phone","sc:profile","sc:roles","sc:HIS","ept:authorization","ept:token","ept:logout"]',
        '["https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger/oauth2-redirect.html"]',
        '[]',
        'https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger',
        '/images/clients/swagger.svg',
        'web',
        NULL,
        '{}',
        NEWID(),
        GETDATE(),
        0
    );
END
