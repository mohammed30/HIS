UPDATE OpenIddictApplications 
SET RedirectUris = '["http://localhost:4200"]',
    PostLogoutRedirectUris = '["http://localhost:4200"]'
WHERE ClientId = 'HIS_App';

UPDATE OpenIddictApplications
SET RedirectUris = '["https://localhost:44382/swagger/oauth2-redirect.html"]'
WHERE ClientId = 'HIS_Swagger';

GO
