-- سكربت لتحديث السيريال (SerialNumber) لسندات الدفع والصرف القديمة
-- يتم ترتيب السندات القديمة (التي السيريال لها = 0) بناءً على تاريخ الإنشاء، ثم إعطائها رقم تسلسلي يبدأ بعد أعلى سيريال موجود في القاعدة

BEGIN TRANSACTION;

BEGIN TRY
    ---------------------------------------------------
    -- 1. تحديث سندات القبض / الصرف (Receipt Vouchers)
    ---------------------------------------------------
    DECLARE @MaxReceiptSerial BIGINT;
    SELECT @MaxReceiptSerial = ISNULL(MAX(SerialNumber), 0) FROM [AppReceiptVouchers];

    WITH CTE_Receipts AS (
        SELECT 
            Id,
            SerialNumber,
            ROW_NUMBER() OVER (ORDER BY CreationTime ASC) as RowNum
        FROM [AppReceiptVouchers]
        WHERE SerialNumber = 0 OR SerialNumber IS NULL
    )
    UPDATE CTE_Receipts
    SET SerialNumber = @MaxReceiptSerial + RowNum;

    PRINT 'تم تحديث سندات القبض بنجاح.';

    ---------------------------------------------------
    -- 2. تحديث سندات الدفع (Payment Vouchers)
    ---------------------------------------------------
    DECLARE @MaxPaymentSerial BIGINT;
    SELECT @MaxPaymentSerial = ISNULL(MAX(SerialNumber), 0) FROM [AppPaymentVouchers];

    WITH CTE_Payments AS (
        SELECT 
            Id,
            SerialNumber,
            ROW_NUMBER() OVER (ORDER BY CreationTime ASC) as RowNum
        FROM [AppPaymentVouchers]
        WHERE SerialNumber = 0 OR SerialNumber IS NULL
    )
    UPDATE CTE_Payments
    SET SerialNumber = @MaxPaymentSerial + RowNum;

    PRINT 'تم تحديث سندات الدفع بنجاح.';

    COMMIT TRANSACTION;
    PRINT 'تم حفظ التعديلات في قاعدة البيانات.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'حدث خطأ أثناء التحديث. تم إلغاء التعديلات.';
    PRINT ERROR_MESSAGE();
END CATCH;
