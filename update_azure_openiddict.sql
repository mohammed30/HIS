-- Update OpenIddict Applications for Azure Environment

DECLARE @FrontendUrl NVARCHAR(MAX) = 'https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net';
DECLARE @BackendUrl NVARCHAR(MAX) = 'https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net';

PRINT 'Updating HIS_App (Angular Frontend)...';
UPDATE OpenIddictApplications
SET 
    ClientUri = @FrontendUrl,
    RedirectUris = '["' + @FrontendUrl + '", "' + @FrontendUrl + '/"]',
    PostLogoutRedirectUris = '["' + @FrontendUrl + '", "' + @FrontendUrl + '/"]'
WHERE ClientId = 'HIS_App';

PRINT 'Updating HIS_Swagger (Swagger UI)...';
UPDATE OpenIddictApplications
SET 
    ClientUri = @BackendUrl + '/swagger',
    RedirectUris = '["' + @BackendUrl + '/swagger/oauth2-redirect.html", "' + @BackendUrl + '/swagger/"]',
    PostLogoutRedirectUris = '["' + @BackendUrl + '/swagger/oauth2-redirect.html"]'
WHERE ClientId = 'HIS_Swagger';

-- Verification
SELECT ClientId, ClientUri, RedirectUris, PostLogoutRedirectUris 
FROM OpenIddictApplications 
WHERE ClientId IN ('HIS_App', 'HIS_Swagger');
