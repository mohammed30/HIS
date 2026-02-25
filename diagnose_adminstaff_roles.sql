-- SQL Script to verify user and role mapping on Azure HISDB

-- 1. Check if AdminStaff role exists
SELECT Id, [Name] FROM AbpRoles WHERE [Name] = 'AdminStaff';

-- 2. Check if user adminstaff exists
SELECT Id, UserName, Email FROM AbpUsers WHERE UserName = 'adminstaff';

-- 3. Check mapping
SELECT u.UserName, r.[Name] AS RoleName
FROM AbpUserRoles ur
JOIN AbpUsers u ON ur.UserId = u.Id
JOIN AbpRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'adminstaff';

-- 4. Check permissions for that role again to be 100% sure
SELECT [Name], ProviderKey FROM AbpPermissionGrants WHERE ProviderKey = 'AdminStaff';
