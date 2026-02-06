-- FULL FIX for OpenIddict (URLs + Permissions)
-- Adds 'scp:offline_access' verification and trailing slash support.

-- UPDATE HIS_App (Frontend)
UPDATE [OpenIddictApplications]
SET 
    -- Fix URLs (allow both with and without trailing slash)
    [RedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net/"]',
    [PostLogoutRedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net/"]',
    [ClientUri] = 'https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net',
    
    -- Fix Permissions (Add scp:offline_access)
    -- Must include all existing permissions + scp:offline_access
    [Permissions] = '["gt:authorization_code","gt:password","gt:client_credentials","gt:refresh_token","gt:LinkLogin","gt:Impersonation","scp:address","scp:email","scp:phone","scp:profile","scp:roles","scp:HIS","ept:authorization","ept:token","ept:logout","scp:offline_access"]'
WHERE [ClientId] = 'HIS_App';

-- UPDATE HIS_Swagger (Backend Docs)
-- Ensure URLs are correct
UPDATE [OpenIddictApplications]
SET 
    [RedirectUris] = '["https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger/oauth2-redirect.html"]',
    [ClientUri] = 'https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger',
    [PostLogoutRedirectUris] = '[]',
    -- Swagger usually doesn't need offline_access, but ensuring consistency
    [Permissions] = '["gt:authorization_code","scp:address","scp:email","scp:phone","scp:profile","scp:roles","scp:HIS","ept:authorization","ept:token","ept:logout"]'
WHERE [ClientId] = 'HIS_Swagger';

-- VERIFY
SELECT [ClientId], [Permissions], [RedirectUris]
FROM [OpenIddictApplications] 
WHERE [ClientId] IN ('HIS_App', 'HIS_Swagger');
