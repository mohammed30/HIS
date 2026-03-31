SET NOCOUNT ON;

-- List of users and the roles they should have
DECLARE @Mappings TABLE (UserName NVARCHAR(256), RoleName NVARCHAR(128));
INSERT INTO @Mappings (UserName, RoleName) VALUES 
('admin', 'admin'),
('adminstaff', 'AdminStaff');

DECLARE @AddedCount INT = 0;
DECLARE @CurrentUserName NVARCHAR(256);
DECLARE @CurrentRoleName NVARCHAR(128);

DECLARE mapping_cursor CURSOR FOR SELECT UserName, RoleName FROM @Mappings;
OPEN mapping_cursor;
FETCH NEXT FROM mapping_cursor INTO @CurrentUserName, @CurrentRoleName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Processing all users with name: ' + @CurrentUserName + ' for role: ' + @CurrentRoleName;
    
    -- Insert into AbpUserRoles if not exists
    INSERT INTO AbpUserRoles (UserId, RoleId)
    SELECT u.Id, r.Id
    FROM AbpUsers u, AbpRoles r
    WHERE u.UserName = @CurrentUserName 
      AND r.Name = @CurrentRoleName
      AND NOT EXISTS (SELECT 1 FROM AbpUserRoles WHERE UserId = u.Id AND RoleId = r.Id);
    
    SET @AddedCount = @AddedCount + @@ROWCOUNT;
    
    FETCH NEXT FROM mapping_cursor INTO @CurrentUserName, @CurrentRoleName;
END

CLOSE mapping_cursor;
DEALLOCATE mapping_cursor;

PRINT 'Mapping process completed.';
PRINT 'Total New Assignments Created: ' + CAST(@AddedCount AS NVARCHAR(10));
GO
