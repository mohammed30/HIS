INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, CreationTime, ExtraProperties, ConcurrencyStamp) 
VALUES 
(NEWID(), 'CBC', N'تعداد الدم الكامل - Complete Blood Count', 50, N'لا يحتاج صيام', N'WBC: 4.5-11.0, RBC: 4.5-5.5, Hb: 12-16', N'cells/mcL', 1, GETDATE(), '{}', NEWID()),
(NEWID(), 'FBS', N'سكر الدم الصائم - Fasting Blood Sugar', 30, N'صيام 8-12 ساعة', N'70-100 mg/dL', N'mg/dL', 1, GETDATE(), '{}', NEWID()),
(NEWID(), 'LIPID', N'فحص الدهون الشامل - Lipid Profile', 80, N'صيام 12 ساعة', N'Total Chol: <200, LDL: <100', N'mg/dL', 1, GETDATE(), '{}', NEWID()),
(NEWID(), 'TSH', N'هرمون الغدة الدرقية - Thyroid Stimulating Hormone', 60, N'لا يحتاج صيام', N'0.4-4.0 mIU/L', N'mIU/L', 1, GETDATE(), '{}', NEWID()),
(NEWID(), 'UA', N'تحليل البول الكامل - Urinalysis', 25, N'عينة بول نظيفة', N'pH: 4.5-8.0', N'-', 1, GETDATE(), '{}', NEWID());
