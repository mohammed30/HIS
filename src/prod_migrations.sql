BEGIN TRANSACTION;
DROP INDEX [IX_AbpPermissions_Name] ON [AbpPermissions];

ALTER TABLE [AbpUsers] ADD [LastSignInTime] datetimeoffset NULL;

DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AbpPermissions]') AND [c].[name] = N'GroupName');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [AbpPermissions] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [AbpPermissions] ALTER COLUMN [GroupName] nvarchar(128) NULL;

ALTER TABLE [AbpPermissions] ADD [ManagementPermissionName] nvarchar(128) NULL;

ALTER TABLE [AbpPermissions] ADD [ResourceName] nvarchar(256) NULL;

CREATE TABLE [AbpResourcePermissionGrants] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [Name] nvarchar(128) NOT NULL,
    [ProviderName] nvarchar(64) NOT NULL,
    [ProviderKey] nvarchar(64) NOT NULL,
    [ResourceName] nvarchar(256) NOT NULL,
    [ResourceKey] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_AbpResourcePermissionGrants] PRIMARY KEY ([Id])
);

CREATE TABLE [AbpUserPasskeys] (
    [CredentialId] varbinary(1024) NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Data] nvarchar(max) NULL,
    CONSTRAINT [PK_AbpUserPasskeys] PRIMARY KEY ([CredentialId]),
    CONSTRAINT [FK_AbpUserPasskeys_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AbpUserPasswordHistories] (
    [UserId] uniqueidentifier NOT NULL,
    [Password] nvarchar(256) NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_AbpUserPasswordHistories] PRIMARY KEY ([UserId], [Password]),
    CONSTRAINT [FK_AbpUserPasswordHistories_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_AbpPermissions_ResourceName_Name] ON [AbpPermissions] ([ResourceName], [Name]) WHERE [ResourceName] IS NOT NULL;

CREATE UNIQUE INDEX [IX_AbpResourcePermissionGrants_TenantId_Name_ResourceName_ResourceKey_ProviderName_ProviderKey] ON [AbpResourcePermissionGrants] ([TenantId], [Name], [ResourceName], [ResourceKey], [ProviderName], [ProviderKey]) WHERE [TenantId] IS NOT NULL;

CREATE INDEX [IX_AbpUserPasskeys_UserId] ON [AbpUserPasskeys] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260316183606_Upgrade_To_ABP_10_1_1', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DailyAttendances]') AND [c].[name] = N'Notes');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [DailyAttendances] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [DailyAttendances] ALTER COLUMN [Notes] nvarchar(1024) NULL;

ALTER TABLE [AppEmployees] ADD [BasicSalary] decimal(18,2) NULL;

CREATE INDEX [IX_DailyAttendances_Date] ON [DailyAttendances] ([Date]);

CREATE INDEX [IX_DailyAttendances_EmployeeId] ON [DailyAttendances] ([EmployeeId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260317055541_AddBasicSalaryToEmployee', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [AppDoctors] ADD [EveningConsultationFee] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [AppDoctors] ADD [MorningConsultationFee] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [AppAppointments] ADD [ConsultationFee] decimal(18,2) NOT NULL DEFAULT 0.0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260317112351_AddFeePropertiesToAppointmentAndDoctor', N'10.0.5');

COMMIT;
GO

