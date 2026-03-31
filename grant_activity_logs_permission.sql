/*
    Grant Activity Logs permissions to admin and AdminStaff roles.
    Target Permission: HIS.ActivityLogs
*/

-- 1. Get Role IDs
DECLARE @AdminRoleId UNIQUEIDENTIFIER;
DECLARE @AdminStaffRoleId UNIQUEIDENTIFIER;

SELECT @AdminRoleId = Id FROM AbpRoles WHERE Name = 'admin';
SELECT @AdminStaffRoleId = Id FROM AbpRoles WHERE Name = 'AdminStaff';

-- 2. Define Permissions to Grant
DECLARE @Permissions TABLE (Name NVARCHAR(128));
INSERT INTO @Permissions (Name) VALUES 
('HIS.ActivityLogs'),
('AuditLogging.View'); -- Standard ABP permission if needed

-- 3. Grant to 'admin' role
INSERT INTO AbpPermissionGrants (Id, Name, ProviderName, ProviderKey)
SELECT NEWID(), p.Name, 'R', 'admin'
FROM @Permissions p
WHERE NOT EXISTS (
    SELECT 1 FROM AbpPermissionGrants 
    WHERE Name = p.Name AND ProviderName = 'R' AND ProviderKey = 'admin'
);

-- 4. Grant to 'AdminStaff' role
INSERT INTO AbpPermissionGrants (Id, Name, ProviderName, ProviderKey)
SELECT NEWID(), p.Name, 'R', 'AdminStaff'
FROM @Permissions p
WHERE NOT EXISTS (
    SELECT 1 FROM AbpPermissionGrants 
    WHERE Name = p.Name AND ProviderName = 'R' AND ProviderKey = 'AdminStaff'
);

-- 5. Verify
SELECT * FROM AbpPermissionGrants WHERE Name IN ('HIS.ActivityLogs', 'AuditLogging.View');
