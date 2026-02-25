-- SQL Script to force logout/refresh for adminstaff on Azure
-- This resets the SecurityStamp which forces the application to re-load user claims from the database
-- Run this if permissions were granted but the user still gets 403 Forbidden

DECLARE @UserId UNIQUEIDENTIFIER;
SELECT @UserId = Id FROM AbpUsers WHERE UserName = 'adminstaff';

IF @UserId IS NOT NULL
BEGIN
    UPDATE AbpUsers 
    SET SecurityStamp = NEWID() 
    WHERE Id = @UserId;
    
    PRINT 'SecurityStamp updated for user adminstaff. Please ask the user to logout and login again.';
END
ELSE
BEGIN
    PRINT 'User adminstaff not found.';
END
GO
