BEGIN TRANSACTION;
ALTER TABLE [AppPatients] DROP CONSTRAINT [FK_AppPatients_AppPatientCategories_PatientCategoryId];

EXEC sp_rename N'[AppPatients].[PatientCategoryId]', N'PaymentMethodId', 'COLUMN';

EXEC sp_rename N'[AppPatients].[IX_AppPatients_PatientCategoryId]', N'IX_AppPatients_PaymentMethodId', 'INDEX';

ALTER TABLE [AppPatients] ADD CONSTRAINT [FK_AppPatients_AppPaymentMethods_PaymentMethodId] FOREIGN KEY ([PaymentMethodId]) REFERENCES [AppPaymentMethods] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260218151706_Fix_PatientCategoryId_PaymentMethodId', N'10.0.0');

COMMIT;
GO

