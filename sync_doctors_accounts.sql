DECLARE @DoctorsPayableId UNIQUEIDENTIFIER = '51be8562-60ee-90d8-4ad2-3a22f4c177f0';

DECLARE @MaxCode INT;
SELECT @MaxCode = ISNULL(MAX(CAST(Code AS INT)), 2150) FROM AppAccounts WHERE ParentId = @DoctorsPayableId;

DECLARE @DocId UNIQUEIDENTIFIER, @DocCode NVARCHAR(MAX), @DocNameAr NVARCHAR(MAX);

DECLARE cur CURSOR FOR 
SELECT Id, Code, NameAr FROM AppDoctors WHERE AccountId IS NULL;

OPEN cur;
FETCH NEXT FROM cur INTO @DocId, @DocCode, @DocNameAr;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @MaxCode = @MaxCode + 1;
    DECLARE @NewAccountId UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO AppAccounts (
        Id, ExtraProperties, ConcurrencyStamp, Code, Name, NameAr, Type, ParentId, CreationTime, IsDeleted, IsActive
    )
    VALUES (
        @NewAccountId,
        '{}',
        REPLACE(CAST(NEWID() AS nvarchar(36)), '-', ''),
        CAST(@MaxCode AS NVARCHAR(50)),
        'Dr. ' + ISNULL(@DocCode, ''),
        N'حق د. ' + ISNULL(@DocNameAr, ''),
        2, -- Liability
        @DoctorsPayableId,
        GETUTCDATE(),
        0,
        1
    );
    
    UPDATE AppDoctors
    SET AccountId = @NewAccountId
    WHERE Id = @DocId;
    
    FETCH NEXT FROM cur INTO @DocId, @DocCode, @DocNameAr;
END

CLOSE cur;
DEALLOCATE cur;
