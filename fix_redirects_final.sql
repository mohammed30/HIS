-- Clear and set correct Local URIs for HIS_App
UPDATE OpenIddictApplications 
SET RedirectUris = '["http://localhost:4200", "http://localhost:4200/"]', 
    PostLogoutRedirectUris = '["http://localhost:4200", "http://localhost:4200/"]' 
WHERE ClientId = 'HIS_App';

-- Clear and set correct Local URIs for HIS_Swagger
UPDATE OpenIddictApplications 
SET RedirectUris = '["https://localhost:44382/swagger/oauth2-redirect.html", "http://localhost:44382/swagger/oauth2-redirect.html"]', 
    PostLogoutRedirectUris = '["https://localhost:44382", "http://localhost:44382"]' 
WHERE ClientId = 'HIS_Swagger';

GO
