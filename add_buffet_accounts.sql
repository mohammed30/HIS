-- Corrected SQL script to add Petty Cash and Buffet accounts
-- Parent (Expenses): 4CA6AAA3-9A3C-1BAB-F761-3A20038DBC94

IF NOT EXISTS (SELECT 1 FROM AppAccounts WHERE Code = '5400')
BEGIN
    INSERT INTO AppAccounts (Id, CreationTime, IsDeleted, Code, Name, NameAr, Type, IsActive, ParentId, ConcurrencyStamp, ExtraProperties)
    VALUES (NEWID(), GETDATE(), 0, '5400', 'Petty Cash & Sundry Expenses', N'نثريات ومصاريف متنوعة', 4, 1, '4CA6AAA3-9A3C-1BAB-F761-3A20038DBC94', NEWID(), '{}');
END

IF NOT EXISTS (SELECT 1 FROM AppAccounts WHERE Code = '5410')
BEGIN
    INSERT INTO AppAccounts (Id, CreationTime, IsDeleted, Code, Name, NameAr, Type, IsActive, ParentId, ConcurrencyStamp, ExtraProperties)
    VALUES (NEWID(), GETDATE(), 0, '5410', 'Buffet Expenses', N'مصاريف البوفيه', 4, 1, '4CA6AAA3-9A3C-1BAB-F761-3A20038DBC94', NEWID(), '{}');
END

SELECT * FROM AppAccounts WHERE Code LIKE '54%' ORDER BY Code;
