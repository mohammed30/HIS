-- SQL Script to update OpenIddict Applications to the new production URLs
-- Run this on your production database (db_aca183_his) to fix redirection and login issues.

USE [db_acc373_asiaback20];
GO

PRINT 'Updating OpenIddict Applications Redirect URIs...';

-- Update HIS_App (Angular Frontend)
UPDATE [OpenIddictApplications]
SET [RedirectUris] = '["http://asiahospitalt-001-site1.jtempurl.com", "http://asiahospitalt-001-site1.jtempurl.com/"]',
    [PostLogoutRedirectUris] = '["http://asiahospitalt-001-site1.jtempurl.com", "http://asiahospitalt-001-site1.jtempurl.com/"]',
    [ClientUri] = 'http://asiahospitalt-001-site1.jtempurl.com'
WHERE [ClientId] = 'HIS_App';

-- Update HIS_Swagger (Swagger UI / Backend)
UPDATE [OpenIddictApplications]
SET [RedirectUris] = '["http://asiaback2000-001-site1.gtempurl.com/swagger/oauth2-redirect.html", "http://asiaback2000-001-site1.gtempurl.com/swagger/"]',
    [PostLogoutRedirectUris] = '["http://asiaback2000-001-site1.gtempurl.com/swagger/oauth2-redirect.html"]',
    [ClientUri] = 'http://asiaback2000-001-site1.gtempurl.com/swagger'
WHERE [ClientId] = 'HIS_Swagger';

GO

-- Verification
SELECT [ClientId], [ClientUri], [RedirectUris], [PostLogoutRedirectUris] 
FROM [OpenIddictApplications]
WHERE [ClientId] IN ('HIS_App', 'HIS_Swagger');
