-- GRANT Missing Invoice Permissions
-- Run this script in HISDB to fix the AbpAuthorizationException

DELETE FROM [AbpPermissionGrants] 
WHERE [Name] = 'HIS.Billing.ManageInvoices' AND [ProviderName] = 'R';

INSERT INTO [AbpPermissionGrants] ([Id], [TenantId], [Name], [ProviderName], [ProviderKey])
SELECT NEWID(), NULL, 'HIS.Billing.ManageInvoices', 'R', 'AdminStaff'
UNION ALL
SELECT NEWID(), NULL, 'HIS.Billing.ManageInvoices', 'R', 'Receptionist'
UNION ALL
SELECT NEWID(), NULL, 'HIS.Billing.ManageInvoices', 'R', 'admin';

-- Verify
SELECT * FROM [AbpPermissionGrants] WHERE [Name] = 'HIS.Billing.ManageInvoices';
