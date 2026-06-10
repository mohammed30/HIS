USE [db_aca183_his];
GO

PRINT 'Syncing Lab Tests to Reception Service Catalog...';

-- إدراج جميع التحاليل المسجلة في قسم المختبر إلى جدول الخدمات الطبية ليراها الاستقبال
INSERT INTO AppServiceItems (
    Id, 
    Code, 
    Name, 
    Category,
    IsActive,
    Price,
    Unit,
    ReferenceRange,
    Instructions,
    ExtraProperties, 
    ConcurrencyStamp, 
    CreationTime, 
    IsDeleted,
    Discriminator
)
SELECT 
    Id, 
    Code, 
    Name, 
    2 AS Category,       -- 2 = LabTest Enum Value
    IsActive, 
    Price, 
    Unit, 
    ReferenceRange, 
    Instructions, 
    ExtraProperties, 
    ConcurrencyStamp, 
    CreationTime, 
    0 AS IsDeleted,      -- Not deleted
    'ServiceItem' AS Discriminator
FROM AppLabTests
WHERE Id NOT IN (SELECT Id FROM AppServiceItems);

PRINT 'Sync Completed Successfully. You should now see all tests in Reception!';
GO
