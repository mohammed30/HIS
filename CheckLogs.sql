-- Retrieve the last 20 log entries to diagnose the 400 error
-- Look for 'invalid_redirect_uri', 'invalid_client', or 'invalid_scope' in the Message or Properties columns.

SELECT TOP 20 
    [TimeStamp], 
    [Level], 
    [Message], 
    [Exception], 
    [Properties]
FROM [Logs]
ORDER BY [TimeStamp] DESC;
