-- Synchronize LabTests to ServiceItems for Reception Ordering
-- Category 2 = LabTest, Discriminator = 'ServiceItem'

INSERT INTO [AppServiceItems] 
    ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [Discriminator], [ExtraProperties], [ConcurrencyStamp])
SELECT 
    [Id], [CreationTime], [IsDeleted], [Code], [Name], 2, [Price], [Unit], [ReferenceRange], [IsActive], 'ServiceItem', [ExtraProperties], [ConcurrencyStamp]
FROM [AppLabTests]
WHERE [Id] NOT IN (SELECT [Id] FROM [AppServiceItems])
  AND [Code] NOT IN (SELECT [Code] FROM [AppServiceItems]);

SELECT 'Migration Completed: ' + CAST(@@ROWCOUNT as varchar) + ' records synced to AppServiceItems.';
