-- CHECK DETAILED OPENIDDICT LOGS
-- OpenIddict often logs validation failures as 'Information', not 'Error'.

SELECT TOP 50 
    [TimeStamp], 
    [Level], 
    [Message], 
    [Properties]
FROM [Logs]
WHERE 
    -- Filter for Information level
    [Level] = 'Information'
    AND (
        [Message] LIKE '%rejected%' 
        OR [Message] LIKE '%redirect_uri%' 
        OR [Message] LIKE '%scope%'
        OR [Message] LIKE '%OpenIddict%'
    )
ORDER BY [TimeStamp] DESC;
