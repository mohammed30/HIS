/*
SQL Script to update OpenIddict Applications to Localhost
Run this on your LOCAL database (HIS) to fix redirection issues during development.
*/

USE [HIS];
GO

-- Update HIS_App (Angular Frontend)
UPDATE [OpenIddictApplications]
SET [RedirectUris] = '["http://localhost:4200", "http://localhost:4200/"]',
    [PostLogoutRedirectUris] = '["http://localhost:4200", "http://localhost:4200/"]'
WHERE [ClientId] = 'HIS_App';

-- Update HIS_Swagger (Swagger UI)
UPDATE [OpenIddictApplications]
SET [RedirectUris] = '["https://localhost:44382/swagger/oauth2-redirect.html", "http://localhost:44382/swagger/oauth2-redirect.html"]',
    [PostLogoutRedirectUris] = '["https://localhost:44382/swagger/", "http://localhost:44382/swagger/"]'
WHERE [ClientId] = 'HIS_Swagger';

GO

SELECT [ClientId], [RedirectUris], [PostLogoutRedirectUris] 
FROM [OpenIddictApplications]
WHERE [ClientId] IN ('HIS_App', 'HIS_Swagger');
