-- COMPLETE FIX for HIS_App Permissions
-- Restores all required permissions including rst:code (response_type=code)

UPDATE [OpenIddictApplications]
SET 
    [Permissions] = '["ept:end_session","gt:authorization_code","rst:code","ept:authorization","ept:token","ept:revocation","ept:introspection","gt:password","gt:client_credentials","gt:refresh_token","gt:LinkLogin","gt:Impersonation","scp:address","scp:email","scp:phone","scp:profile","scp:roles","scp:HIS","scp:offline_access"]',
    [RedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]',
    [PostLogoutRedirectUris] = '["https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net","https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net"]'
WHERE [ClientId] = 'HIS_App';

-- VERIFY
SELECT [ClientId], [Permissions] FROM [OpenIddictApplications] WHERE [ClientId] = 'HIS_App';
