USE [db_ac621c_his];
GO

-- 1. إضافة عمود CategoryId إلى جدول AppLabTests في حال لم يكن موجوداً
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'CategoryId' AND Object_ID = Object_ID(N'AppLabTests'))
BEGIN
    ALTER TABLE [AppLabTests] ADD CategoryId uniqueidentifier NULL;
    PRINT 'Added CategoryId column to AppLabTests.';
END
GO

-- 2. إنشاء جدول AppLabTestCategories في حال لم يكن موجوداً
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppLabTestCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AppLabTestCategories](
        [Id] [uniqueidentifier] NOT NULL,
        [Code] [nvarchar](32) NOT NULL,
        [Name] [nvarchar](128) NOT NULL,
        [ParentId] [uniqueidentifier] NULL,
        [SortOrder] [int] NOT NULL,
        [IsActive] [bit] NOT NULL,
        [ExtraProperties] [nvarchar](max) NOT NULL,
        [ConcurrencyStamp] [nvarchar](40) NOT NULL,
        [CreationTime] [datetime2](7) NOT NULL,
        [CreatorId] [uniqueidentifier] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModifierId] [uniqueidentifier] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT ((0)),
        [DeleterId] [uniqueidentifier] NULL,
        [DeletionTime] [datetime2](7) NULL,
     CONSTRAINT [PK_AppLabTestCategories] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
    ) ON [PRIMARY]
    PRINT 'Created AppLabTestCategories table.';
END
GO
