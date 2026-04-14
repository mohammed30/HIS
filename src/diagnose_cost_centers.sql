-- ============================================
-- فحص التطابق بين الكود وقاعدة البيانات
-- ============================================

-- 1) أعمدة جدول AppAccounts في البرودكشن
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AppAccounts'
ORDER BY ORDINAL_POSITION;

-- 2) هل عمود IsActive موجود؟ (موجود في الكود لكن ممكن غير موجود في البرودكشن)
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AppAccounts' AND COLUMN_NAME = 'IsActive';

-- 3) عرض الحسابات مع TenantId
SELECT Id, TenantId, Code, Name, NameAr, Type, ParentId, IsDeleted
FROM AppAccounts
ORDER BY Code;

-- 4) هل فيه Tenants مسجلين؟
SELECT Id, Name FROM AbpTenants;

-- 5) تحقق من أعمدة TenantId
SELECT TABLE_NAME, COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME IN ('AppAccounts', 'AppDepartments') 
  AND COLUMN_NAME = 'TenantId';
