-- FORCE UPDATE for HIS_App (Frontend)
UPDATE [OpenIddictApplications]
SET 
    [RedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net/"]',
    [PostLogoutRedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net/"]',
    [ClientUri] = 'https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net'
WHERE [ClientId] = 'HIS_App';

-- FORCE UPDATE for HIS_Swagger (Backend Docs)
UPDATE [OpenIddictApplications]
SET 
    [RedirectUris] = '["https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger/oauth2-redirect.html"]',
    [ClientUri] = 'https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger',
    [PostLogoutRedirectUris] = '[]'
WHERE [ClientId] = 'HIS_Swagger';

-- VERIFY CHANGES
SELECT [ClientId], [RedirectUris], [ClientUri] 
FROM [OpenIddictApplications] 
WHERE [ClientId] IN ('HIS_App', 'HIS_Swagger');
