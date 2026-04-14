-- Migrate Buffet and Sundries accounts to production
-- Based on local HIS database structure

DECLARE @ParentExpensesId UNIQUEIDENTIFIER;

-- 1. Find the Parent Expenses account (5000) in production
SELECT @ParentExpensesId = Id 
FROM AppAccounts 
WHERE Code = '5000';

IF @ParentExpensesId IS NULL
BEGIN
    PRINT 'Error: Parent account 5000 (Expenses) not found in production.';
    RETURN;
END

-- 2. Add Sundry Expenses (5400) if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM AppAccounts WHERE Code = '5400')
BEGIN
    INSERT INTO AppAccounts (
        Id, Code, Name, NameAr, Type, ParentId, IsActive, 
        ConcurrencyStamp, CreationTime, IsDeleted, ExtraProperties
    )
    VALUES (
        NEWID(), 
        '5400', 
        'Petty Cash & Sundry Expenses', 
        N'نثريات ومصاريف متنوعة', 
        4, -- Expense Type
        @ParentExpensesId, 
        1, 
        NEWID(), 
        GETDATE(), 
        0,
        N'{}'
    );
    PRINT 'Account 5400 (Sundries) added.';
END
ELSE
BEGIN
    PRINT 'Account 5400 already exists.';
END

-- 3. Add Buffet Expenses (5410) if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM AppAccounts WHERE Code = '5410')
BEGIN
    INSERT INTO AppAccounts (
        Id, Code, Name, NameAr, Type, ParentId, IsActive, 
        ConcurrencyStamp, CreationTime, IsDeleted, ExtraProperties
    )
    VALUES (
        NEWID(), 
        '5410', 
        'Buffet Expenses', 
        N'مصاريف البوفيه', 
        4, -- Expense Type
        @ParentExpensesId, 
        1, 
        NEWID(), 
        GETDATE(), 
        0,
        N'{}'
    );
    PRINT 'Account 5410 (Buffet) added.';
END
ELSE
BEGIN
    PRINT 'Account 5410 already exists.';
END
