-- Check if PaymentMethodId column exists
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppPatients' AND COLUMN_NAME = 'PaymentMethodId')
    PRINT 'PaymentMethodId column exists.'
ELSE
    PRINT 'PaymentMethodId column DOES NOT exist.'

-- Check if PatientCategoryId (old column) still exists
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppPatients' AND COLUMN_NAME = 'PatientCategoryId')
    PRINT 'PatientCategoryId column still exists (Migration might be pending).'
ELSE
    PRINT 'PatientCategoryId column does not exist.'

-- Count patients with NULL PaymentMethodId (if column exists)
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppPatients' AND COLUMN_NAME = 'PaymentMethodId')
BEGIN
    DECLARE @NullCount INT
    SELECT @NullCount = COUNT(*) FROM AppPatients WHERE PaymentMethodId IS NULL
    PRINT 'Patients with NULL PaymentMethodId: ' + CAST(@NullCount AS NVARCHAR(10))
END

-- List top 10 patients to inspect data
SELECT TOP 10 Id, MRN, FirstNameAr, LastNameAr, PaymentMethodId FROM AppPatients ORDER BY CreationTime DESC

-- List all Payment Methods
SELECT * FROM AppPaymentMethods
