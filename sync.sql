DECLARE @CurrentLiabilitiesId UNIQUEIDENTIFIER;
SELECT @CurrentLiabilitiesId = Id FROM AppAccounts WHERE Code = '2100';

DECLARE @DoctorsPayableId UNIQUEIDENTIFIER;
SELECT @DoctorsPayableId = Id FROM AppAccounts WHERE Code = '2150';

IF @DoctorsPayableId IS NULL
BEGIN
    SET @DoctorsPayableId = NEWID();
    INSERT INTO AppAccounts (Id, Code, Name, NameAr, Type, ParentId, IsActive, CreationTime, ExtraProperties, ConcurrencyStamp)
    VALUES (@DoctorsPayableId, '2150', 'Doctors Payable', N'حقوق الأطباء', 1, @CurrentLiabilitiesId, 1, GETDATE(), '{}', CAST(NEWID() AS NVARCHAR(50)));
END

DECLARE @DoctorId UNIQUEIDENTIFIER;
DECLARE @DoctorCode NVARCHAR(MAX);
DECLARE @DoctorNameAr NVARCHAR(MAX);
DECLARE @NextCode INT;

SELECT @NextCode = ISNULL(MAX(CAST(Code AS INT)), 2150) + 1 FROM AppAccounts WHERE ParentId = @DoctorsPayableId;

DECLARE doc_cursor CURSOR FOR
SELECT Id, Code, NameAr FROM AppDoctors WHERE AccountId IS NULL AND IsActive = 1;

OPEN doc_cursor;
FETCH NEXT FROM doc_cursor INTO @DoctorId, @DoctorCode, @DoctorNameAr;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @NewAccountId UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO AppAccounts (Id, Code, Name, NameAr, Type, ParentId, IsActive, CreationTime, ExtraProperties, ConcurrencyStamp)
    VALUES (@NewAccountId, CAST(@NextCode AS NVARCHAR(50)), 'Dr. ' + @DoctorCode, N'حق د. ' + @DoctorNameAr, 1, @DoctorsPayableId, 1, GETDATE(), '{}', CAST(NEWID() AS NVARCHAR(50)));
    
    UPDATE AppDoctors SET AccountId = @NewAccountId WHERE Id = @DoctorId;
    
    SET @NextCode = @NextCode + 1;
    
    FETCH NEXT FROM doc_cursor INTO @DoctorId, @DoctorCode, @DoctorNameAr;
END

CLOSE doc_cursor;
DEALLOCATE doc_cursor;
