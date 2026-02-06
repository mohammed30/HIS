-- CHECK CURRENT REDIRECT URIS
-- Verifying if the trailing slash update was actually applied.

SELECT [ClientId], [RedirectUris]
FROM [OpenIddictApplications]
WHERE [ClientId] = 'HIS_App';
