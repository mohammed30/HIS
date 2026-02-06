-- Update HIS_App (Frontend)
-- RedirectUris and PostLogoutRedirectUris must be JSON arrays
UPDATE [OpenIddictApplications]
SET [RedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]',
    [PostLogoutRedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]',
    [ClientUri] = 'https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net'
WHERE [ClientId] = 'HIS_App';

-- Update HIS_Swagger (Backend / Swagger)
UPDATE [OpenIddictApplications]
SET [RedirectUris] = '["https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger/oauth2-redirect.html"]',
    [ClientUri] = 'https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/swagger'
WHERE [ClientId] = 'HIS_Swagger';
