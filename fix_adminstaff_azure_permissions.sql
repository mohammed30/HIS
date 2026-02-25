-- SQL Script to grant missing permissions to AdminStaff role on Azure HISDB
-- This grants access to Operations Reports, Purchase Orders, and other relevant features

DECLARE @RoleName NVARCHAR(256) = 'AdminStaff';

-- List of permissions to grant
DECLARE @Permissions TABLE (Name NVARCHAR(256));
INSERT INTO @Permissions (Name) VALUES 
('HIS.Operations.Default'),
('HIS.Operations.Report'),
('HIS.Operations.Manage'),
('HIS.Inventory.Default'),
('HIS.Inventory.PurchaseOrders'),
('HIS.Inventory.PurchaseRequisitions'),
('HIS.Inventory.Dashboard'),
('HIS.Inventory.Suppliers'),
('HIS.Billing.FinancialReports');

-- Insert missing permissions
INSERT INTO AbpPermissionGrants (Id, TenantId, [Name], ProviderName, ProviderKey)
SELECT NEWID(), NULL, p.Name, 'R', @RoleName
FROM @Permissions p
WHERE NOT EXISTS (
    SELECT 1 FROM AbpPermissionGrants 
    WHERE [Name] = p.Name 
    AND ProviderName = 'R' 
    AND ProviderKey = @RoleName
);

PRINT 'Permissions granted to role ' + @RoleName + '.';

-- Verify
SELECT [Name], ProviderKey FROM AbpPermissionGrants WHERE ProviderKey = @RoleName AND [Name] LIKE 'HIS%';
