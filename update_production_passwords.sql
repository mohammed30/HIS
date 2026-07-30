-- ============================================================
-- Update passwords for non-admin users on production database
-- Password = Username (except admin and AdminStaff)
-- Date: 2026-07-30
-- ============================================================

UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEMIZ3VrCAHJRPibnJDTkHOrE8GLb6WBbx+H2xq8OYs8hwvse/a2ptBnlyhx/wvKChA==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'doctor';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEMqCgOXkBgNioH17n03nIqFxaYYWhXlNd0TpojN662pKv0Ea+JqLRXhmnu6n5TzUEQ==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'labmanager';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEOAv28xBk1XglfWJun1HUP2lkWA3+P4ffxsJbliLK+QjInlQfZyMdsrRDp5iBF/jmg==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'labtech';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEPtunmS+x7BFf6A95tTas0F7izM0Z6FUFzfqcGNdJKZBxSKWX8fCJaKVMTm0luQ3Cw==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'patient';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAECNmWJzs/OIAyJyi+j8rVeDPy6UDY58TLrT2kall9gHthcyD0BSCoNwKx+Rj/NZHNQ==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'pharmacist';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAELdEDlgm62bDKRaiwlEKNYsaAbaMe9D/XdKs68ZbUUpAf/i4d90gVkZKopZ1RNF7VQ==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'radmanager';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEL5vLH1Yp1q5wbVVEUI76KSP4zzVZ3z4XEq9XaOkrVYLSAcnsJ161KN8wkg+lGWOTA==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'radtech';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEKjxROXe2dwyZuOEt4u2ZQ3XxP1NRMPsaMXPhGpV/FOl73DrBnUBTWwH2uXgiy6IpQ==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'receptionist';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEEvHT5onrlxKvSuHUblHyqF58DtUVcGabD3X2uUq24Qq1MPxgYX/qRk8BzccKurNHg==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'security';
UPDATE AbpUsers SET PasswordHash = 'AQAAAAIAAYagAAAAEDpL7LkUjgQRSW4AkLHXQkN0umFdXheONhIM+2ZSG4/qdwj8Tmj3c5w514ustJgykw==', SecurityStamp = NEWID(), ConcurrencyStamp = NEWID(), LastModificationTime = GETUTCDATE() WHERE UserName = 'storekeeper';

-- Verify results
SELECT UserName, CASE WHEN PasswordHash IS NOT NULL THEN 'Updated' ELSE 'No Password' END AS Status, LastModificationTime
FROM AbpUsers WHERE UserName NOT IN ('admin', 'AdminStaff') ORDER BY UserName;
