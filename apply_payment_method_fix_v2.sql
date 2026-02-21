-- FIX SCRIPT FOR MISSING PaymentMethodId COLUMN AND FK CONFLICT
-- Run this script on your Azure SQL Database

BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Rename PatientCategoryId to PaymentMethodId (if needed)
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppPatients' AND COLUMN_NAME = 'PatientCategoryId')
    AND NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppPatients' AND COLUMN_NAME = 'PaymentMethodId')
    BEGIN
        PRINT 'Renaming PatientCategoryId to PaymentMethodId...';
        EXEC sp_rename 'AppPatients.PatientCategoryId', 'PaymentMethodId', 'COLUMN';
    END
    ELSE
    BEGIN
        PRINT 'Column rename not needed (PatientCategoryId not found or PaymentMethodId already exists).';
    END

    -- 2. Rename the Index (if it exists with the old name)
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AppPatients_PatientCategoryId' AND object_id = OBJECT_ID('AppPatients'))
    BEGIN
        PRINT 'Renaming Index IX_AppPatients_PatientCategoryId...';
        EXEC sp_rename 'AppPatients.IX_AppPatients_PatientCategoryId', 'IX_AppPatients_PaymentMethodId', 'INDEX';
    END

    -- 3. Drop the old foreign key constraint (if exists)
    DECLARE @OldFkName NVARCHAR(128);
    SELECT @OldFkName = name FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID('AppPatients') 
    AND name = 'FK_AppPatients_AppPatientCategories_PatientCategoryId';

    IF @OldFkName IS NOT NULL
    BEGIN
        PRINT 'Dropping old Foreign Key ' + @OldFkName + '...';
        EXEC('ALTER TABLE [AppPatients] DROP CONSTRAINT [' + @OldFkName + ']');
    END

    -- 4. CLEAN UP ORPHANED RECORDS (CRITICAL FIX FOR FK CONFLICT)
    -- This sets any PaymentMethodId in AppPatients that doesn't exist in AppPaymentMethods to NULL
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppPatients' AND COLUMN_NAME = 'PaymentMethodId')
    BEGIN
        PRINT 'Cleaning up orphaned PaymentMethodId records...';
        
        -- Use dynamic SQL to avoid compile errors if column doesn't exist yet
        EXEC('UPDATE AppPatients 
              SET PaymentMethodId = NULL 
              WHERE PaymentMethodId IS NOT NULL 
              AND PaymentMethodId NOT IN (SELECT Id FROM AppPaymentMethods)');
              
        PRINT 'Orphaned records cleaned up.';
    END

    -- 5. Add the new foreign key constraint (if not exists)
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('AppPatients') AND name = 'FK_AppPatients_AppPaymentMethods_PaymentMethodId')
    BEGIN
        PRINT 'Adding new Foreign Key FK_AppPatients_AppPaymentMethods_PaymentMethodId...';
        ALTER TABLE [AppPatients] ADD CONSTRAINT [FK_AppPatients_AppPaymentMethods_PaymentMethodId] 
        FOREIGN KEY ([PaymentMethodId]) REFERENCES [AppPaymentMethods] ([Id]);
    END

    PRINT 'SUCCESS: Database schema fixed and data cleaned.';
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    PRINT 'ERROR: ' + ERROR_MESSAGE();
    ROLLBACK TRANSACTION;
END CATCH
