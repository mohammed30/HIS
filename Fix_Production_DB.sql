USE [db_ac621c_his]; -- Update this only if your database name differs on production
GO

PRINT 'Starting to fix missing ABP columns...';

DECLARE @TableName NVARCHAR(128);

DECLARE TableCursor CURSOR FOR
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
  AND (TABLE_NAME LIKE 'AppInventory%' OR TABLE_NAME LIKE 'AppInternalRequest%');

OPEN TableCursor;
FETCH NEXT FROM TableCursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @SQL NVARCHAR(MAX);

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'ExtraProperties' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD ExtraProperties nvarchar(max) DEFAULT ''{}'' NOT NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added ExtraProperties to ' + @TableName;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'ConcurrencyStamp' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD ConcurrencyStamp nvarchar(40) DEFAULT NEWID() NOT NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added ConcurrencyStamp to ' + @TableName;
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'CreationTime' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD CreationTime datetime2 DEFAULT GETDATE() NOT NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added CreationTime to ' + @TableName;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'CreatorId' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD CreatorId uniqueidentifier NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added CreatorId to ' + @TableName;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'LastModificationTime' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD LastModificationTime datetime2 NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added LastModificationTime to ' + @TableName;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'LastModifierId' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD LastModifierId uniqueidentifier NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added LastModifierId to ' + @TableName;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'IsDeleted' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD IsDeleted bit DEFAULT 0 NOT NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added IsDeleted to ' + @TableName;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'DeleterId' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD DeleterId uniqueidentifier NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added DeleterId to ' + @TableName;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'DeletionTime' AND Object_ID = Object_ID(@TableName))
    BEGIN
        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@TableName) + N' ADD DeletionTime datetime2 NULL;'
        EXEC sp_executesql @SQL;
        PRINT 'Added DeletionTime to ' + @TableName;
    END

    FETCH NEXT FROM TableCursor INTO @TableName;
END

CLOSE TableCursor;
DEALLOCATE TableCursor;
GO

PRINT 'Production DB Schema Fix Completed.';
GO
