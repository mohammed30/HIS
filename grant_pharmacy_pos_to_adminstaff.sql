-- SQL Script to grant Pharmacy POS permission to AdminStaff role
-- This applies the change immediately to the database

IF NOT EXISTS (
    SELECT 1 FROM AbpPermissionGrants 
    WHERE [Name] = 'HIS.Pharmacy.POS' 
    AND ProviderName = 'R' 
    AND ProviderKey = 'AdminStaff'
)
BEGIN
    INSERT INTO AbpPermissionGrants (Id, TenantId, [Name], ProviderName, ProviderKey)
    VALUES (NEWID(), NULL, 'HIS.Pharmacy.POS', 'R', 'AdminStaff');
    
    PRINT 'Permission HIS.Pharmacy.POS granted to role AdminStaff.';
END
ELSE
BEGIN
    PRINT 'Permission HIS.Pharmacy.POS already granted to role AdminStaff.';
END
GO
