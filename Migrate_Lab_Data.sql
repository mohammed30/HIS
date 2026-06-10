USE [db_aca183_his];
GO
PRINT 'Starting Lab Data Migration...';
GO
-- Categories
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = 'F023806A-60BE-440D-8A5E-064F2DA1B924') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('F023806A-60BE-440D-8A5E-064F2DA1B924', 'ELECT', 'ELECTRO PHORESIS', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91', 3, 1, '{}', '05eab48e-bd80-4c6a-8f9d-5859a8f0291f', '2026-04-05 09:16:18.7466667');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = '907C225C-F5EA-4B01-98EB-19BDDCE49B59') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('907C225C-F5EA-4B01-98EB-19BDDCE49B59', '5', 'TUMOR MARKER', NULL, 6, 1, '{}', 'ef7ec95f-b39d-4fdf-ab7c-c7fb52c0cb2a', '2026-04-05 09:16:18.7466667');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = 'C2B77699-2EFC-4D23-B686-22449EA796BA') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('C2B77699-2EFC-4D23-B686-22449EA796BA', '9', 'SUPRA RENAL', NULL, 10, 1, '{}', '989b24a0-6357-43f3-8c95-37c0c517f265', '2026-04-05 09:16:18.7500000');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = '5DC53105-E8D5-4923-8120-8DF5D777527B') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('5DC53105-E8D5-4923-8120-8DF5D777527B', '6', 'IMMUNOLOGY', NULL, 7, 1, '{}', '73267a49-1499-4459-880b-9ad2e9a8bafc', '2026-04-05 09:16:18.7466667');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = '96107F49-2958-4C5E-99E1-A526190F8E8B') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('96107F49-2958-4C5E-99E1-A526190F8E8B', '7', 'TORCH PROFILE', NULL, 8, 1, '{}', 'b0322b80-f0d0-4076-ac62-924dcba62b1e', '2026-04-05 09:16:18.7466667');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = 'E68A31EA-5B9E-4E07-A8B4-AD01C4A10C4F') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('E68A31EA-5B9E-4E07-A8B4-AD01C4A10C4F', '3', 'ENDOCRINOLOGY THYROID F. TEST', NULL, 4, 1, '{}', '8c8693c8-d856-4cdc-b8fa-b1aa58f975f1', '2026-04-05 09:16:18.7466667');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = '36869772-F984-4CE2-B74B-B3B222EA6791') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('36869772-F984-4CE2-B74B-B3B222EA6791', 'H.PYL', 'H.PYLORI', 'C2B77699-2EFC-4D23-B686-22449EA796BA', 11, 1, '{}', '46a7b9a5-2a2b-476e-9c32-a77168227477', '2026-04-05 09:16:18.7500000');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91', '2', 'IRON PROFILE', NULL, 2, 1, '{}', '0a4dfee0-ee7b-4cfd-a24e-77757afea7f7', '2026-04-05 09:16:18.7466667');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = '6EAC0B3A-4C79-48DB-BBFB-C346177BC010') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('6EAC0B3A-4C79-48DB-BBFB-C346177BC010', '8', 'VIROLOGY', NULL, 9, 1, '{}', '3e0b3664-0d17-4dbd-a259-675e86387095', '2026-04-05 09:16:18.7500000');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = 'EE188ECF-7356-4523-A6CF-C9C2027930D1') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('EE188ECF-7356-4523-A6CF-C9C2027930D1', '4', 'FERTILITY', NULL, 5, 1, '{}', 'd405c5f0-a1ec-4c7f-bd52-c36560749fb1', '2026-04-05 09:16:18.7466667');
IF NOT EXISTS (SELECT 1 FROM AppLabTestCategories WHERE Id = '38601FFB-BA88-4A31-96EC-EE172F2AA501') 
INSERT INTO AppLabTestCategories (Id, Code, Name, ParentId, SortOrder, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime) 
VALUES ('38601FFB-BA88-4A31-96EC-EE172F2AA501', '1', 'HAEMATOLOGY', NULL, 1, 1, '{}', 'aa8fa9b0-ccdf-4cb3-9f89-1049c374fa49', '2026-04-05 09:16:18.7466667');
-- Lab Tests
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'C5AE196B-E183-4A08-9C92-019D5779ED5B') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('C5AE196B-E183-4A08-9C92-019D5779ED5B', '415', 'Ca++', 160.00, NULL, '8.5-10.5 mg/dL', 'mg/dL', 1, '{}', '77594e6f-0c84-4cc0-8234-10c14071cddf', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '831DAED0-532C-435B-8090-045EA3AE5F94') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('831DAED0-532C-435B-8090-045EA3AE5F94', '120', 'Serum Iron', 104.00, NULL, '60-170 mcg/dL', 'mcg/dL', 1, '{}', '1cf7319c-8d32-4444-a668-2cda2ee2d9d4', '2026-04-05 09:16:18.7500000', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'CF3910A5-5600-4BDD-B398-066ACB12D193') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('CF3910A5-5600-4BDD-B398-066ACB12D193', '418', 'urine k+', 155.00, NULL, '25-125 mEq/24hr', 'hr', 1, '{}', 'de9f19c3-6110-4b9d-88ed-9bb53c8df4a9', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'FCEE4D5A-6904-4272-A504-0A3A4CE82A0F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('FCEE4D5A-6904-4272-A504-0A3A4CE82A0F', '309', 'Rheumatoid Factor', 298.00, NULL, 'Negative (< 14 IU/mL)', NULL, 1, '{}', '600922c5-1a48-4b94-ab4f-6a00d32d0e7f', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0AFE314C-5CC3-4EDC-9D5C-0B21F2A2B7C1') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0AFE314C-5CC3-4EDC-9D5C-0B21F2A2B7C1', '461', 'U. Amylase', 116.00, NULL, '24-400 U/L', 'U/L', 1, '{}', 'b058946a-4463-4b90-9be3-22bfe2013c7c', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '6B749872-4E8C-4826-9754-0C8C4BCC9253') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('6B749872-4E8C-4826-9754-0C8C4BCC9253', '353', '24 Urine.Cortisol(pm)', 173.00, NULL, 'Evaluated alongside AM result', 'result', 1, '{}', '4f2ec7a1-fb88-4db7-80b0-4dad283172c9', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '1E95002E-B9D0-4B65-99D2-104A410809BE') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('1E95002E-B9D0-4B65-99D2-104A410809BE', '132', 'immuno electrophoresis', 178.00, NULL, 'Normal pattern (Pathologist dependent)', 'dependent', 1, '{}', '13a7ed63-3699-445d-9c1b-8a500ad1c0b8', '2026-04-05 09:16:18.7500000', 'F023806A-60BE-440D-8A5E-064F2DA1B924');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'C7324729-26C2-4541-9FEE-137BEDE44C85') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('C7324729-26C2-4541-9FEE-137BEDE44C85', '319', 'C4', 252.00, NULL, '15-45 mg/dL', 'mg/dL', 1, '{}', 'b094cac3-0ee6-441b-b3ec-36380f72a192', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'D8ED64C6-AADE-4946-8376-14F1E66B1230') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('D8ED64C6-AADE-4946-8376-14F1E66B1230', '130', 'Protein', 192.00, NULL, 'Total: 6.0-8.3 g/dL', 'g/dL', 1, '{}', 'e60427e1-ac83-415b-90cd-6cd473f2e4b0', '2026-04-05 09:16:18.7500000', 'F023806A-60BE-440D-8A5E-064F2DA1B924');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'B3DC8185-CD9E-4021-B8B3-16843E869A34') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('B3DC8185-CD9E-4021-B8B3-16843E869A34', '209', 'Testosterone', 160.00, NULL, '300-1000 ng/dL (M) / 15-70 ng/dL (F)', NULL, 1, '{}', '940e196c-fbae-4ca3-b33e-575243a0097b', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'D6E85C4C-0D4F-4B56-9BF4-1D67AC60BB4F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('D6E85C4C-0D4F-4B56-9BF4-1D67AC60BB4F', '111', 'A.P.T.T', 143.00, NULL, '25-35 seconds', 'seconds', 1, '{}', '58b98417-3260-4525-858e-cea008a98108', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '7FB47B19-E7D7-47F6-9931-1E5A543579C5') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('7FB47B19-E7D7-47F6-9931-1E5A543579C5', '401', 'FBS', 175.00, NULL, '70-99 mg/dL', 'mg/dL', 1, '{}', 'dd62d264-9456-4001-b081-b0137391176b', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '2A4EE6BA-403A-4300-AE1D-1EB443E0D74F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('2A4EE6BA-403A-4300-AE1D-1EB443E0D74F', '310', 'C.R Protein', 462.00, NULL, '< 10 mg/L', 'mg/L', 1, '{}', '45efd4ba-fec1-42af-b7f6-fc5b255e5917', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '37B8ED51-2796-42C7-BC64-1F77D245BD07') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('37B8ED51-2796-42C7-BC64-1F77D245BD07', '402', '2 Hrs After meal', 108.00, NULL, '< 140 mg/dL', 'mg/dL', 1, '{}', '112a6ae2-42e3-45e2-87f3-64d69b63e3ee', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '7D614A52-C227-4C0E-A68D-2059638AAA8E') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('7D614A52-C227-4C0E-A68D-2059638AAA8E', '205', 'FSH', 179.00, NULL, 'Varies by age/cycle phase', NULL, 1, '{}', '4d49085d-1edd-42b7-bba9-9f7bd5834251', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '487972B2-4A65-43C8-A851-215F65C4FEE6') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('487972B2-4A65-43C8-A851-215F65C4FEE6', '713', 'Wound Swab', 173.00, NULL, 'No growth', 'growth', 1, '{}', '63559e00-7e28-43f1-81a6-38d1da717a24', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '8E154F1C-F4D6-42B3-B56B-22674E618693') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('8E154F1C-F4D6-42B3-B56B-22674E618693', 'UA', '????? ????? ?????? - Urinalysis', 25.00, '???? ??? ?????', 'pH: 4.5-8.0', '-', 1, '{}', '2E74C16C-48DF-4269-8055-B4BF74E29D78', '2026-03-15 17:41:38.9700000', NULL);
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '33EA5CFD-2733-4167-8168-236810650C90') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('33EA5CFD-2733-4167-8168-236810650C90', '104', 'ESR', 116.00, NULL, '0-20 mm/hr', 'mm/hr', 1, '{}', '93569154-278e-4899-a94d-6b0b7ba5b4d9', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'FD5E7A19-94A2-4F5F-B386-23708874FBE5') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('FD5E7A19-94A2-4F5F-B386-23708874FBE5', '117', 'B.M Biopsy & Aspiration', 119.00, NULL, 'Pathologist report dependent', 'dependent', 1, '{}', 'b6817d7b-44c4-4e59-b127-5e8ccc17b1e3', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'A13EA884-88D9-433A-9D6B-2376EF5E05CD') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('A13EA884-88D9-433A-9D6B-2376EF5E05CD', '318', 'C3', 262.00, NULL, '80-160 mg/dL', 'mg/dL', 1, '{}', 'eb3b9d3c-bbc6-4a3d-a430-ad95ad8b1c2f', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'FC87F5BD-808D-41AA-AC89-2583C36759EE') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('FC87F5BD-808D-41AA-AC89-2583C36759EE', '419', 'Creatinine Clearance', 136.00, NULL, '90-120 mL/min', 'mL/min', 1, '{}', 'fb613728-7007-4d4c-b8f8-04bc2a92492f', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '78BDC215-CF65-4F74-9C03-287B7573EFE9') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('78BDC215-CF65-4F74-9C03-287B7573EFE9', '124', 'VIT.B12', 159.00, NULL, '200-900 pg/mL', 'pg/mL', 1, '{}', '3d58f631-2dbb-4fd6-a8be-731fe3f84614', '2026-04-05 09:16:18.7500000', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'CCDFA45E-71AC-4B81-AA8A-28C116260847') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('CCDFA45E-71AC-4B81-AA8A-28C116260847', '218', 'CA19-9', 100.00, NULL, '< 37 U/mL', 'U/mL', 1, '{}', '45812d95-4fad-4890-ae43-dfd790373c82', '2026-04-05 09:16:18.7500000', '907C225C-F5EA-4B01-98EB-19BDDCE49B59');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '56A60220-5B83-4F0F-9D2B-2C17D4C0ABA2') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('56A60220-5B83-4F0F-9D2B-2C17D4C0ABA2', '219', 'Total PSA', 126.00, NULL, '< 4.0 ng/mL', 'ng/mL', 1, '{}', '6aa803c7-05cf-4548-a880-03b23c848349', '2026-04-05 09:16:18.7500000', '907C225C-F5EA-4B01-98EB-19BDDCE49B59');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'C634801B-494B-43F3-AA78-2C9F7493FC43') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('C634801B-494B-43F3-AA78-2C9F7493FC43', '210', 'Progesterone', 187.00, NULL, 'Varies by cycle phase', NULL, 1, '{}', 'f0f25f11-d1bf-4b5e-bab3-d5a4aca56a6a', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'BA11838C-3A2A-4C51-A755-2D092A3BE0EE') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('BA11838C-3A2A-4C51-A755-2D092A3BE0EE', '215', 'CEA', 161.00, NULL, '< 3.0 ng/mL', 'ng/mL', 1, '{}', '1ca1b112-bb10-41c2-ba97-ba2c2b4bfacc', '2026-04-05 09:16:18.7500000', '907C225C-F5EA-4B01-98EB-19BDDCE49B59');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F1DC5CC5-F27B-42D6-9849-2D5322F38B5C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F1DC5CC5-F27B-42D6-9849-2D5322F38B5C', '454', 'D. Dimer', 112.00, NULL, '< 0.50 mg/L FEU', 'FEU', 1, '{}', '1dac5dd8-19ec-4d83-8828-648b69b1f10b', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '92D6D0CD-475E-40A6-A33C-2D94729FCFA4') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('92D6D0CD-475E-40A6-A33C-2D94729FCFA4', '801', 'Cytopathology', 181.00, NULL, 'Negative for malignancy or atypia', NULL, 1, '{}', '45749a86-c0c4-44ef-9725-f868fc590369', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '1BA4277F-746B-4F04-94F2-3005D2A64D5E') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('1BA4277F-746B-4F04-94F2-3005D2A64D5E', '345', 'HIV (1+2) Abs', 182.00, NULL, 'Non-reactive', 'reactive', 1, '{}', '0dd7f077-ae2f-41f7-b9f5-8a85e423c18a', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '42EE0CC5-75FA-4C21-A81B-32E7B0150B2F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('42EE0CC5-75FA-4C21-A81B-32E7B0150B2F', '216', 'Ca15-3', 132.00, NULL, '< 30 U/mL', 'U/mL', 1, '{}', '02554fd6-8a22-416a-9b38-ccc44fe1d36b', '2026-04-05 09:16:18.7500000', '907C225C-F5EA-4B01-98EB-19BDDCE49B59');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '04641D6A-F9EA-47DB-8B59-34FD68438BBC') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('04641D6A-F9EA-47DB-8B59-34FD68438BBC', '510', 'Serum -Ascitic Albumin Gradient(SAAG)', 131.00, NULL, '< 1.1 g/dL (Non-portal HTN) / > 1.1 g/dL (Portal HTN)', 'HTN', 1, '{}', '1c0672a4-90f6-413c-be17-f82bc3e70a09', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '51B93F92-2D99-4877-BAA8-353A87CFA22D') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('51B93F92-2D99-4877-BAA8-353A87CFA22D', '342', 'HEV Abs IgG/IgM', 135.00, NULL, 'Non-reactive', 'reactive', 1, '{}', '8a89b760-a475-4d7b-a0f3-a38d8fc55ff5', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'D0482115-F3CD-47E5-89A9-3640B01660E4') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('D0482115-F3CD-47E5-89A9-3640B01660E4', '320', 'Anti CCP', 384.00, NULL, '< 20 u/mL (Negative)', NULL, 1, '{}', '9c704525-fb5d-4ed8-91a9-e27c06a9ad8e', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '26CE79C2-7D1F-43B3-9A96-389FCF222CCF') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('26CE79C2-7D1F-43B3-9A96-389FCF222CCF', '602', 'Stool Analysis', 184.00, NULL, 'Formed', 'Formed', 1, '{}', '7e608ba7-e127-46f6-9d30-6bed4b6f3552', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'BB60243A-D5C1-41EB-8320-3BBB2C85432E') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('BB60243A-D5C1-41EB-8320-3BBB2C85432E', '702', 'Urine For C/S', 192.00, NULL, '< 10', NULL, 1, '{}', 'b967a92c-98dc-4921-bafa-4af99f791010', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'DB484769-75EE-443F-B409-3BE9EB09ECAE') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('DB484769-75EE-443F-B409-3BE9EB09ECAE', '710', 'Wet.preparation', 174.00, NULL, 'Negative (No clues cells', NULL, 1, '{}', '0816e5ff-d0ac-436f-8a96-3621e5ab6b7e', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '107FDA3B-F09F-49A3-B51F-3CA557A59124') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('107FDA3B-F09F-49A3-B51F-3CA557A59124', '321', 'ANCA Profile', 492.00, NULL, 'Negative', NULL, 1, '{}', '1e19233a-3023-40ac-8afc-5a274963a081', '2026-04-05 09:16:18.7533333', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F2FF0E10-BCDB-4045-B648-3D673CEC1469') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F2FF0E10-BCDB-4045-B648-3D673CEC1469', '407', 'P.P Blood Sugar', 123.00, NULL, '< 140 mg/dL', 'mg/dL', 1, '{}', '15b78fe2-8972-4d82-83f0-4294b526564c', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '9843B264-68EC-4581-AAE1-4138B46F7E3F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('9843B264-68EC-4581-AAE1-4138B46F7E3F', '330', 'Toxo IgM', 117.00, NULL, 'Negative', NULL, 1, '{}', 'c825fc6b-e63e-44b8-8420-4e782be39fc1', '2026-04-05 09:16:18.7533333', '96107F49-2958-4C5E-99E1-A526190F8E8B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '77098554-DC46-4022-943A-41B8ABA21478') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('77098554-DC46-4022-943A-41B8ABA21478', '221', 'a.Feto Protein', 143.00, NULL, '< 10 ng/mL', 'ng/mL', 1, '{}', 'bcf6ccab-a929-4e29-b1ae-eface67e4f0b', '2026-04-05 09:16:18.7500000', '907C225C-F5EA-4B01-98EB-19BDDCE49B59');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'D9E1D52D-BFA5-42A9-981F-4469450300BA') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('D9E1D52D-BFA5-42A9-981F-4469450300BA', '220', 'Free PSA', 169.00, NULL, 'Ratio > 25% generally indicates lower risk', 'risk', 1, '{}', 'd563b63d-6243-4d9e-bff4-bf5ddb90230b', '2026-04-05 09:16:18.7500000', '907C225C-F5EA-4B01-98EB-19BDDCE49B59');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '1A8685AF-644B-4ED7-9B4C-45E20921C2B8') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('1A8685AF-644B-4ED7-9B4C-45E20921C2B8', '333', 'Rubella IgG', 153.00, NULL, 'Negative (Positive indicates immunity)', NULL, 1, '{}', '7807ca5a-f79c-48ac-8cc1-2a1105062354', '2026-04-05 09:16:18.7533333', '96107F49-2958-4C5E-99E1-A526190F8E8B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '609AD3DF-20C2-4A84-A7B1-46D661E0F850') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('609AD3DF-20C2-4A84-A7B1-46D661E0F850', '125', 'Folic Acid', 136.00, NULL, '2.7-17.0 ng/mL', 'ng/mL', 1, '{}', '20c347c2-5480-4ba8-9d4a-bb7ee556a958', '2026-04-05 09:16:18.7500000', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'B13F6B44-89F3-49E8-8FE7-471576438446') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('B13F6B44-89F3-49E8-8FE7-471576438446', '306', 'Anti Phospholipid IgG', 364.00, NULL, 'Negative', NULL, 1, '{}', 'f038e403-1629-4204-80b2-967b0d8bce98', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '26018690-120C-4474-98CA-476305B02075') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('26018690-120C-4474-98CA-476305B02075', '455', 'Pro.BNP', 163.00, NULL, '< 125 pg/mL', 'pg/mL', 1, '{}', 'f7251932-10b9-4178-9f61-dda3d8152f3d', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'DA13C543-569F-4900-8C7C-49E4EAF8E548') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('DA13C543-569F-4900-8C7C-49E4EAF8E548', '405', 'HbA1c', 114.00, NULL, '4.0%-5.6% (Non-diabetic)', 'diabetic', 1, '{}', '332f824e-50b5-4511-887c-cb4e1a235aa2', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '6EE027DE-13B5-41B4-A0CA-4BC4A42D32F1') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('6EE027DE-13B5-41B4-A0CA-4BC4A42D32F1', '512', 'Spot Urine for calcium/creatinine ratio', 124.00, NULL, '< 0.14 mg/mg', 'mg/mg', 1, '{}', '78705869-c8f2-46c2-ad2f-a6ba2a3c29ed', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0E1253D3-4BC3-4527-A50A-54DE2E43A5D9') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0E1253D3-4BC3-4527-A50A-54DE2E43A5D9', '508', 'CSF Analysis', 122.00, NULL, 'Clear; Protein 15-45 mg/dL; Gluc 40-70 mg/dL', 'mg/dL', 1, '{}', '03cadfda-ab38-4900-b660-ff5a40d81b42', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '2DD2AC8E-AFA7-4155-A523-558B1138DEF9') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('2DD2AC8E-AFA7-4155-A523-558B1138DEF9', '708', 'ZN Stain For AAFB', 193.00, NULL, 'Negative (No Acid-Fast Bacilli seen)', NULL, 1, '{}', '47a75d29-0976-457b-8b7a-2192a26913c3', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '05760D75-9700-4834-BC6A-573DA0016ACF') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('05760D75-9700-4834-BC6A-573DA0016ACF', '431', 'AST (GOT)', 193.00, NULL, '8-40 U/L', 'U/L', 1, '{}', '796692e4-7041-49c2-a88c-f3c0169317d7', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '6A47807E-E36F-4026-9DEC-5A2C1B37E493') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('6A47807E-E36F-4026-9DEC-5A2C1B37E493', '410', 'Blood Urea', 135.00, NULL, '7-20 mg/dL', 'mg/dL', 1, '{}', 'd437a3c6-c9fb-4c00-8bb2-2b27b1033c34', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '58742D91-1137-4F8D-A275-5FE7E02CF422') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('58742D91-1137-4F8D-A275-5FE7E02CF422', '433', 'ALK. Phosphatase', 173.00, NULL, '44-147 U/L', 'U/L', 1, '{}', '82c0bf8a-9c82-4afc-8742-3abd347e15dd', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5796F780-B135-4201-BA6F-61F0C1BC9D2F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5796F780-B135-4201-BA6F-61F0C1BC9D2F', '323', 'ANA Profile', 390.00, NULL, 'Negative', NULL, 1, '{}', 'cc598ed8-7222-456d-8a67-f28a26416fd2', '2026-04-05 09:16:18.7533333', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '969F443B-BCD5-4D88-BA49-64B112C73060') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('969F443B-BCD5-4D88-BA49-64B112C73060', '116', 'LE Cells', 68.00, NULL, 'Negative', NULL, 1, '{}', '192029b1-77af-440b-b03b-e973b0b9c592', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '09744A92-7CA8-4CDA-9DD4-64B1671E40A3') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('09744A92-7CA8-4CDA-9DD4-64B1671E40A3', '322', 'Beta 2-Glycoprotein', 286.00, NULL, 'Negative (< 20 SGU)', NULL, 1, '{}', '6e039bab-15fa-4d7c-b810-3288cb23738f', '2026-04-05 09:16:18.7533333', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F41F335D-1C98-4102-8A6D-64D6CD7FA201') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F41F335D-1C98-4102-8A6D-64D6CD7FA201', '212', 'AMH', 135.00, NULL, '1.0-4.0 ng/mL (Reproductive age females)', 'females', 1, '{}', '5da59e4c-02f7-4841-ab3d-ae0f786a0c20', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'E5312E03-1696-4AAD-BF99-64E3DF5734BA') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('E5312E03-1696-4AAD-BF99-64E3DF5734BA', '105', 'Platelets Count', 114.00, NULL, '150', NULL, 1, '{}', '14cc8e44-fcb9-4fa4-8106-000e4ac13597', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '56D0AC70-5B51-4006-9244-654032F235A1') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('56D0AC70-5B51-4006-9244-654032F235A1', '107', 'Reticulocyte Count', 136.00, NULL, '0.5%-1.5%', NULL, 1, '{}', '0f6783d9-6cc4-4c0b-b9c4-b99f29becef6', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '8A1D804B-09C5-4398-AE8A-6544C4E09809') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('8A1D804B-09C5-4398-AE8A-6544C4E09809', '301', 'ANA(ANF)', 437.00, NULL, 'Negative (< 1:40 titer)', NULL, 1, '{}', 'e282b543-c1ca-4454-8fda-10f8ad183207', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '58156A11-04E3-4770-A71B-65CCC2F86BAF') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('58156A11-04E3-4770-A71B-65CCC2F86BAF', '114', 'Stool for occult Blood', 116.00, NULL, 'Negative', NULL, 1, '{}', '229546b2-50f6-427c-b1dc-1cc840ee0e54', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '01EE9425-779F-4601-9937-65E59A52384B') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('01EE9425-779F-4601-9937-65E59A52384B', '108', 'Bleeding Time B.T', 69.00, NULL, '2-7 minutes', 'minutes', 1, '{}', 'd5d3942f-fcf9-41a5-83c1-ccd03b99d2f3', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5FBBFBB7-619B-4D6D-90CD-6645EB6937A3') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5FBBFBB7-619B-4D6D-90CD-6645EB6937A3', '452', 'T.CPK', 134.00, NULL, '22-198 U/L', 'U/L', 1, '{}', 'c5d1810d-d491-49c1-9184-018ba8f1fb2b', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '38AFB138-A08A-4663-A8F6-68404D7182C2') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('38AFB138-A08A-4663-A8F6-68404D7182C2', '436', 'Total Bilirubin', 109.00, NULL, '0.1-1.2 mg/dL', 'mg/dL', 1, '{}', '20ea7160-ab17-4d3d-85d1-2943b9ea49c6', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '10C06C60-4AD8-4CD5-9150-6855C906148E') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('10C06C60-4AD8-4CD5-9150-6855C906148E', '352', '24 Urine Cortisol(am)', 198.00, NULL, '10-100 mcg/24hr (Total daily)', 'daily', 1, '{}', 'ca119680-d37b-4a09-ac13-7f9edd44b4b0', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0B97BF1B-0C99-48A5-AB23-685A8DEB2132') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0B97BF1B-0C99-48A5-AB23-685A8DEB2132', '332', 'Rubella IgM', 194.00, NULL, 'Negative', NULL, 1, '{}', 'a222550c-a0d1-4f8a-818b-1835916cc052', '2026-04-05 09:16:18.7533333', '96107F49-2958-4C5E-99E1-A526190F8E8B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '4729A2D1-E359-4ECC-9F9E-6AD085F0E32C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('4729A2D1-E359-4ECC-9F9E-6AD085F0E32C', '712', 'Throat Swab', 149.00, NULL, 'Normal upper respiratory flora', 'flora', 1, '{}', '91e8b58f-a450-434e-9387-a5ed288617fa', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5704B4C0-9F2C-474B-BFC1-6C24930AB73B') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5704B4C0-9F2C-474B-BFC1-6C24930AB73B', '341', 'HBe Ag', 144.00, NULL, 'Non-reactive', 'reactive', 1, '{}', 'ae5caa94-e77b-4035-9b06-443155f0512d', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'C109EDF6-C67E-4EE5-8EC5-6D7EAD7A4963') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('C109EDF6-C67E-4EE5-8EC5-6D7EAD7A4963', '442', 'HDL- C', 134.00, NULL, '> 40 mg/dL (M) / > 50 mg/dL (F)', NULL, 1, '{}', 'd0814e85-d898-4c18-ae3d-144f3e75954d', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'AB927830-956E-4A59-A8BD-6E9BEA6D4DD6') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('AB927830-956E-4A59-A8BD-6E9BEA6D4DD6', '103', 'TWBC & differential Count', 117.00, NULL, '4', NULL, 1, '{}', '8382d4c3-283b-49c4-a79d-bee5ca87dcfb', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'A9DCBA86-0A55-4779-83E3-70E8DE9DD0A0') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('A9DCBA86-0A55-4779-83E3-70E8DE9DD0A0', '509', 'Serum effusion albumin gradient (SEAG)', 191.00, NULL, '< 1.1 g/dL (Exudate) / > 1.1 g/dL (Transudate)', 'Transudate', 1, '{}', '20c96ddc-d6e3-42e5-a331-fee91db025da', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '4E030A02-D34B-4947-9DE3-71839384A0AF') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('4E030A02-D34B-4947-9DE3-71839384A0AF', '346', 'AntiHBs Ag', 100.00, NULL, 'Positive (> 10 mIU/mL) indicates immunity', 'immunity', 1, '{}', '8901c6eb-29ec-4241-ac87-3d9441df6f50', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '88FCA53E-FEDB-4F78-B54F-7377F86BDCEB') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('88FCA53E-FEDB-4F78-B54F-7377F86BDCEB', '450', 'Troponin', 174.00, NULL, '< 0.04 ng/mL (highly lab-specific)', 'specific', 1, '{}', '045d6729-4a06-4614-b2b7-579b873f172a', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'BEB1A9BB-54CA-45DA-B89A-738694F70ACB') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('BEB1A9BB-54CA-45DA-B89A-738694F70ACB', '420', '24hr Urine Protein', 128.00, NULL, '< 150 mg/24hr', 'hr', 1, '{}', '610f148c-f352-407d-97cf-e5dd657df804', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'BA4D10F0-1D1F-4ADC-9B4B-78311EAF57B9') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('BA4D10F0-1D1F-4ADC-9B4B-78311EAF57B9', '305', 'Anti Phospholipid IgM', 261.00, NULL, 'Negative', NULL, 1, '{}', '4225ebd3-e3e7-43cf-97c2-9e9ea40a9dc0', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5FDAF00C-5FB4-4D37-8A08-7A8E90DADF16') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5FDAF00C-5FB4-4D37-8A08-7A8E90DADF16', '709', 'Direct Gram stain', 158.00, NULL, 'No organisms seen', 'seen', 1, '{}', 'b86346c1-1dbe-42b7-a33e-6de94d9be3e6', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'DC938F4C-1FD7-47CC-8FCD-7AF3CF587097') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('DC938F4C-1FD7-47CC-8FCD-7AF3CF587097', '704', 'Pus', 103.00, NULL, 'No growth', 'growth', 1, '{}', '8ae645fe-dff2-4779-b2eb-d0bec7b61adf', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '142902D5-E9C1-4F45-9609-7BCE10211E32') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('142902D5-E9C1-4F45-9609-7BCE10211E32', '802', 'Histopathology', 199.00, NULL, 'Benign tissue architecture', 'architecture', 1, '{}', '154e4812-b46f-4b67-86fb-960f2317df0a', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '6E80DEDB-DD8F-46B4-B1BF-7C6D2BE906FC') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('6E80DEDB-DD8F-46B4-B1BF-7C6D2BE906FC', '506', 'Body Fluid Analysis', 187.00, NULL, 'Variable based on fluid type', 'type', 1, '{}', 'c178cb9d-d44b-4ebc-9e84-d7baa9efdda9', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F1EB21AC-4932-445A-BFCB-7CA05510CBA6') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F1EB21AC-4932-445A-BFCB-7CA05510CBA6', '350', 'S.Cortisol(am)', 123.00, NULL, '5-23 mcg/dL', 'mcg/dL', 1, '{}', 'ceb1ce18-588a-4bf7-ad3a-0da3cec8f86e', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'DEBE7A01-EE83-4468-9710-7EAC97EEBEB1') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('DEBE7A01-EE83-4468-9710-7EAC97EEBEB1', 'LIPID', '??? ?????? ?????? - Lipid Profile', 80.00, '???? 12 ????', 'Total Chol: <200, LDL: <100', 'mg/dL', 1, '{}', 'EE2D4FD5-A36D-4484-9B22-8E6B5A83FE23', '2026-03-15 17:41:38.9700000', NULL);
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5D061049-EA80-48AE-B7E7-7F0FE7753FBB') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5D061049-EA80-48AE-B7E7-7F0FE7753FBB', '612', 'QBC for Malaria', 157.00, NULL, 'Negative', NULL, 1, '{}', '11387293-a5c3-4b35-9d92-8d5034460e1c', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '249E85AA-A26C-4E15-BDC3-7FB3E6E07EAF') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('249E85AA-A26C-4E15-BDC3-7FB3E6E07EAF', '211', 'B.HCG', 111.00, NULL, '< 5.0 mIU/mL (Non-pregnant)', 'pregnant', 1, '{}', '3a24d29e-e63b-4a6a-b214-717f6d7cb806', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'BC79E76E-CA9A-4AE4-B16D-827B6C091935') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('BC79E76E-CA9A-4AE4-B16D-827B6C091935', '121', 'S.Ferritin', 126.00, NULL, '12-300 ng/mL', 'ng/mL', 1, '{}', '66d264a3-5fcc-4069-833c-ac9b4f3c52aa', '2026-04-05 09:16:18.7500000', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'B63C62C4-A2B9-4EBA-A251-84B3A1BA0E77') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('B63C62C4-A2B9-4EBA-A251-84B3A1BA0E77', '432', 'ALT (GPT)', 156.00, NULL, '7-56 U/L', 'U/L', 1, '{}', 'a04c0024-2d13-44f1-9a3e-2c7d76677cb0', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '57CCA383-CED6-43C9-A6F6-855D028C2B64') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('57CCA383-CED6-43C9-A6F6-855D028C2B64', '354', 'R.cortisol', 190.00, NULL, 'Baseline comparison', 'comparison', 1, '{}', '7b432205-e110-4074-97c7-eacaa27f2064', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'C83B32C6-5316-4467-9F12-87C681923F1D') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('C83B32C6-5316-4467-9F12-87C681923F1D', '312', 'ASO.Titer', 412.00, NULL, '< 200 IU/mL', 'IU/mL', 1, '{}', '9f96e6a5-5ef7-4f02-b815-fdf48678dad0', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '9062A4CA-10B9-4CD6-8C95-87DE8409413D') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('9062A4CA-10B9-4CD6-8C95-87DE8409413D', '411', 'Serum Creatinine', 194.00, NULL, '0.6-1.2 mg/dL', 'mg/dL', 1, '{}', 'd7139871-d350-4b82-a478-7b9df1358538', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '113F68D2-DD2F-4056-B534-88BAE5B0580F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('113F68D2-DD2F-4056-B534-88BAE5B0580F', '705', 'High Vaginal Swab C/S', 142.00, NULL, 'Normal vaginal flora / No pathogens', 'pathogens', 1, '{}', '3b8fcfdc-b4e5-4cb6-881b-52d463c7cc5f', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '888D9C6B-09E8-4E87-B36B-88E86B719123') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('888D9C6B-09E8-4E87-B36B-88E86B719123', '501', 'Acid Phosphatase', 137.00, NULL, '0.1-0.5 U/L', 'U/L', 1, '{}', 'ac51a50f-62bb-4242-929b-da4712121473', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'CC930294-426B-4C29-A64F-89C40053FF24') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('CC930294-426B-4C29-A64F-89C40053FF24', '703', 'Sputum', 110.00, NULL, 'Normal respiratory flora / No pathogens', 'pathogens', 1, '{}', '3875d172-644f-47e9-9674-707f21911d86', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0D25E8D0-2AD6-4269-B8D7-89C74EE96E99') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0D25E8D0-2AD6-4269-B8D7-89C74EE96E99', '201', 'TSH', 279.00, NULL, '0.4-4.0 mIU/L', 'mIU/L', 1, '{}', '8b25401e-f2c7-47ac-8885-72177f4ad401', '2026-04-05 09:16:18.7500000', 'E68A31EA-5B9E-4E07-A8B4-AD01C4A10C4F');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '541EF70D-C406-4867-8366-8A2C4AB4D320') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('541EF70D-C406-4867-8366-8A2C4AB4D320', '601', 'Urine general', 125.00, NULL, 'Clear', 'Clear', 1, '{}', '103e3f06-6d14-4ae9-a431-2efc0bc26506', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '55C597CB-080E-4D25-9444-8B9803010DD6') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('55C597CB-080E-4D25-9444-8B9803010DD6', '126', 'VIT.D.T', 153.00, NULL, '20-50 ng/mL', 'ng/mL', 1, '{}', '83d271b7-e183-4404-a067-645afacc29e0', '2026-04-05 09:16:18.7500000', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '32FC06D6-F6FB-4632-A9CE-8C61C71F6531') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('32FC06D6-F6FB-4632-A9CE-8C61C71F6531', '303', 'Anti Cardiolipin IgM', 293.00, NULL, 'Negative (< 12 MPL U/mL)', NULL, 1, '{}', '96a09d4b-3a8c-4676-b598-2922e4bb0862', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '83336DCA-4CE4-47B3-80F4-8E532B0D6B5C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('83336DCA-4CE4-47B3-80F4-8E532B0D6B5C', '437', 'D. Bilirubin', 166.00, NULL, '< 0.3 mg/dL', 'mg/dL', 1, '{}', '7b57c0c1-28e7-4209-86c9-fd6d222b057c', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '9CC63105-BA2E-4BF5-8D4F-92B3FE9667C8') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('9CC63105-BA2E-4BF5-8D4F-92B3FE9667C8', '417', 'Urine Na+', 186.00, NULL, '40-220 mEq/24hr', 'hr', 1, '{}', '12f75d3c-fee4-4fcc-8299-6f5cd9b3db51', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '7F8B1775-990E-45BE-92AF-95247781147A') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('7F8B1775-990E-45BE-92AF-95247781147A', '307', 'Anti-T.T-G(IgG)', 439.00, NULL, 'Negative (< 15 U/mL)', NULL, 1, '{}', '31177482-574c-4731-af11-81f6a1378d18', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F372067C-B3DA-4CF3-8F9B-9859181E986C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F372067C-B3DA-4CF3-8F9B-9859181E986C', '513', 'Spot Urine for Albumin/creatinine ratio', 176.00, NULL, '< 30 mcg/mg', 'mcg/mg', 1, '{}', '29d42e61-aaf6-4539-93a8-006574e18005', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '9C9102F7-4F9A-4C54-9889-9889D0EC1BD5') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('9C9102F7-4F9A-4C54-9889-9889D0EC1BD5', '203', 'Ft4', 390.00, NULL, '0.9-1.7 ng/dL', 'ng/dL', 1, '{}', '8217d0b7-8abc-474b-a869-0af7581e680e', '2026-04-05 09:16:18.7500000', 'E68A31EA-5B9E-4E07-A8B4-AD01C4A10C4F');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '955E3A75-5B03-4C74-A55E-9B24C4E8C327') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('955E3A75-5B03-4C74-A55E-9B24C4E8C327', '206', 'Prolactin', 152.00, NULL, '< 25 ng/mL (Females) / < 17 ng/mL (Males)', 'Males', 1, '{}', '35c2019a-83a3-4833-8b99-d4831f46c806', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'B3EE2781-FDD4-4135-989D-9BF625DF055A') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('B3EE2781-FDD4-4135-989D-9BF625DF055A', '412', 'S. Na+', 192.00, NULL, '135-145 mEq/L', 'mEq/L', 1, '{}', '7052cde1-1bb9-4b8b-b7f4-2bc3a8199062', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'C74F05C5-AC28-4499-A131-9C2B6F0B69F3') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('C74F05C5-AC28-4499-A131-9C2B6F0B69F3', '505', 'Blood Gases', 137.00, NULL, 'pH 7.35-7.45; pO2 75-100; pCO2 35-45', 'pCO', 1, '{}', '1d673111-57b9-4a47-b4b9-d4b01574d084', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'D6A2943C-82CA-483F-B886-9C5411AD6062') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('D6A2943C-82CA-483F-B886-9C5411AD6062', '336', 'HSV-1&2IgG', 125.00, NULL, 'Negative', NULL, 1, '{}', '39b8aa70-a244-4bce-8784-1f3501113b25', '2026-04-05 09:16:18.7533333', '96107F49-2958-4C5E-99E1-A526190F8E8B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'E3F1A3CF-D696-447D-A5BC-9D0BE6C0ACC4') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('E3F1A3CF-D696-447D-A5BC-9D0BE6C0ACC4', '803', 'Histopathology (Colon.com mastecto)', 115.00, NULL, 'Negative for malignancy / clean margins', NULL, 1, '{}', '32fa11be-1d00-40fe-a0c3-21755cd7a119', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'D3EB7C43-6E0A-40DC-A24E-9D87E8882BB1') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('D3EB7C43-6E0A-40DC-A24E-9D87E8882BB1', '308', 'Anti Gliadin IgG', 267.00, NULL, 'Negative (< 15 U/mL)', NULL, 1, '{}', 'db503dfb-ad61-45f8-bce1-f698c04823e2', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '61720181-077A-474D-A1D9-9E0D07BFC486') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('61720181-077A-474D-A1D9-9E0D07BFC486', '317', 'Stool for typhoid and Para Typhi Ags', 284.00, NULL, 'Negative', NULL, 1, '{}', '5e9b4a76-3412-45a7-977a-10b2852ae2f1', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '63D8300B-64B5-4817-8A6E-9FA4B274669A') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('63D8300B-64B5-4817-8A6E-9FA4B274669A', '443', 'LDL-C', 178.00, NULL, '< 100 mg/dL', 'mg/dL', 1, '{}', '0ae6572b-ecd5-4738-9144-15777ca73611', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F19CF4B0-D226-466B-8EA9-A003D101B58A') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F19CF4B0-D226-466B-8EA9-A003D101B58A', '348', 'HBe Ab.', 122.00, NULL, 'Non-reactive', 'reactive', 1, '{}', '2c0469d6-8440-4109-b6d8-05e7479216e7', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '9626AD8A-7774-447F-93F4-A11F446E7FA4') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('9626AD8A-7774-447F-93F4-A11F446E7FA4', '414', 'S.Uric Acid', 116.00, NULL, '3.5-7.2 mg/dL', 'mg/dL', 1, '{}', '05b9b18c-21a3-4857-a366-aaabc3388e9d', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F4CAA68C-A976-4BE7-BA63-A1462D94B3F8') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F4CAA68C-A976-4BE7-BA63-A1462D94B3F8', '131', 'Haemoglobin', 101.00, NULL, 'HbA > 95%', NULL, 1, '{}', 'd5df54e0-137e-427f-bc62-596c9303c03b', '2026-04-05 09:16:18.7500000', 'F023806A-60BE-440D-8A5E-064F2DA1B924');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '203790C5-0C59-43F0-8F96-A233477D0818') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('203790C5-0C59-43F0-8F96-A233477D0818', '462', 'S. Lipase', 132.00, NULL, '0-160 U/L', 'U/L', 1, '{}', '6e550b16-8b7e-4f98-8dbb-725702acda0f', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '40641CDF-78C4-4000-90ED-A24C2638D314') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('40641CDF-78C4-4000-90ED-A24C2638D314', '408', 'Fasting +2Hr 75 g Glucose', 185.00, NULL, '< 140 mg/dL (after 2 hours)', 'hours', 1, '{}', '8f3f037a-7bfd-41bd-820d-784686047a3b', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'ABB07864-2FC3-4956-A313-A347C8B5D0F4') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('ABB07864-2FC3-4956-A313-A347C8B5D0F4', '503', 'Chloride', 197.00, NULL, '96-106 mEq/L', 'mEq/L', 1, '{}', 'e760d5e7-346f-4637-8751-28365c2e67e6', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '967657AB-EC45-4F76-A351-A3CA37C12144') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('967657AB-EC45-4F76-A351-A3CA37C12144', '202', 'FT3', 209.00, NULL, '2.3-4.1 pg/mL', 'pg/mL', 1, '{}', '6a5a4174-a983-4d15-8da4-bb1012982f26', '2026-04-05 09:16:18.7500000', 'E68A31EA-5B9E-4E07-A8B4-AD01C4A10C4F');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'FDDEC1F0-BA29-4A79-BB0F-A3EACDCF0453') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('FDDEC1F0-BA29-4A79-BB0F-A3EACDCF0453', '316', 'IGE', 427.00, NULL, '< 100 kU/L', 'kU/L', 1, '{}', '17ddd9e6-6def-411f-b749-24fb0adade43', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '825CD039-D3F6-4E68-AA53-A4BDF890B0F0') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('825CD039-D3F6-4E68-AA53-A4BDF890B0F0', '603', 'Stool For Reducing Subs.', 148.00, NULL, 'Negative', NULL, 1, '{}', 'f79c4ae3-dc7d-4231-8ff0-3b7e2db71403', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '138CB330-AF84-4843-99C0-A4C64ACC497A') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('138CB330-AF84-4843-99C0-A4C64ACC497A', '434', 'T. Protein', 105.00, NULL, '6.0-8.3 g/dL', 'g/dL', 1, '{}', '21fc04e8-9ee5-4dd9-a428-d2fd8a8ad9fc', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F3F31749-F131-47C5-A64C-A593B951E221') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F3F31749-F131-47C5-A64C-A593B951E221', '331', 'Toxo IgG', 132.00, NULL, 'Negative', NULL, 1, '{}', '0c671abd-d09a-41d5-9d24-dd67265d7cce', '2026-04-05 09:16:18.7533333', '96107F49-2958-4C5E-99E1-A526190F8E8B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5475D25C-0034-4AD9-B709-A5F96D9EBCD2') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5475D25C-0034-4AD9-B709-A5F96D9EBCD2', '315', 'Pregnancy Test', 410.00, NULL, 'Negative (if not pregnant)', NULL, 1, '{}', 'dbcd881e-78de-46a4-82e4-312af2a7e4dc', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '1740E2AB-B079-4A69-A735-A603C3DB25FC') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('1740E2AB-B079-4A69-A735-A603C3DB25FC', '313', 'Widal for Typhoid test', 381.00, NULL, 'Negative (Titer < 1:80)', NULL, 1, '{}', 'd1914889-c1ae-4250-bec1-b0a2bfe8b7d9', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '25BA52DA-AA1F-4C9F-B95D-A796605275F5') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('25BA52DA-AA1F-4C9F-B95D-A796605275F5', '606', 'Urea Breath Test', 135.00, NULL, 'Negative', NULL, 1, '{}', '8394fe96-b4e2-4e76-963e-230dc951120d', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '88D725A9-12C5-4D3B-A049-AD2973A8858C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('88D725A9-12C5-4D3B-A049-AD2973A8858C', '453', 'LDH', 163.00, NULL, '140-280 U/L', 'U/L', 1, '{}', 'be608c43-d458-475a-b11f-c04d7aff76bb', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F56F3835-467F-4792-97CF-AD6B783A295B') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F56F3835-467F-4792-97CF-AD6B783A295B', 'CBC', '????? ???? ?????? - Complete Blood Count', 50.00, '?? ????? ????', 'WBC: 4.5-11.0, RBC: 4.5-5.5, Hb: 12-16', 'cells/mcL', 1, '{}', '4931C6AE-99C5-49B0-B199-DADF3B0C2F70', '2026-03-15 17:41:38.9700000', NULL);
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '57610233-80EC-4621-8CDA-AF59E9EF4234') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('57610233-80EC-4621-8CDA-AF59E9EF4234', '504', 'Urine for Bence Jon protein', 195.00, NULL, 'Negative', NULL, 1, '{}', '4aeb9211-17e1-4599-93ef-d695822bd607', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0EB6B089-57EB-4047-AD0E-B02763A5D8EB') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0EB6B089-57EB-4047-AD0E-B02763A5D8EB', '610', 'BF For Malaria', 119.00, NULL, 'Negative (No parasites seen)', NULL, 1, '{}', '3f2d62e1-58f2-4339-b729-f934fcb636f3', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '33F3EA73-9E54-404E-AD34-B1281188513B') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('33F3EA73-9E54-404E-AD34-B1281188513B', '207', 'LH', 181.00, NULL, 'Varies by age/cycle phase', NULL, 1, '{}', '23f6f6cf-28cb-48fd-9201-0a52022590f2', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '2909431D-9F8A-4F96-9F3C-B7428385589A') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('2909431D-9F8A-4F96-9F3C-B7428385589A', '439', 'Serum/CSF.lactate', 157.00, NULL, '0.5-1.0 mmol/L (Serum)', 'Serum', 1, '{}', '087fa6ad-1ea0-4f3d-a9c1-f7ad95e012bf', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'B9702E49-0BBB-4709-8347-B86BD27308D6') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('B9702E49-0BBB-4709-8347-B86BD27308D6', '413', 'S. K+', 124.00, NULL, '3.5-5.0 mEq/L', 'mEq/L', 1, '{}', '9c1033e4-e62f-45f5-8583-b80e4bdaf42c', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0A25C7A5-A2CB-4583-ACB1-B9A70842CCAC') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0A25C7A5-A2CB-4583-ACB1-B9A70842CCAC', '611', 'ICT Malaria (Ag)', 133.00, NULL, 'Negative', NULL, 1, '{}', 'feccce58-6af5-477e-9dbd-834c97ec4ff6', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'B722CF48-2612-420F-9B03-BA152770E2EE') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('B722CF48-2612-420F-9B03-BA152770E2EE', '706', 'Body Fluid C/S', 157.00, NULL, 'No growth (Sterile)', 'Sterile', 1, '{}', 'db71709b-8674-4583-82e2-75b11b7d0ed6', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '91785E0D-D386-4439-A1A1-BD07CF06529E') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('91785E0D-D386-4439-A1A1-BD07CF06529E', '340', 'HBsAg', 181.00, NULL, 'Non-reactive', 'reactive', 1, '{}', 'd34c08e4-ba93-4346-8a25-f480d65afa63', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'B9306EB2-A954-4055-8050-C08A04D18E83') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('B9306EB2-A954-4055-8050-C08A04D18E83', '113', 'Blood Group', 71.00, NULL, 'N/A (A/B/AB/O', 'A/B/AB/O', 1, '{}', 'd170081d-9861-443f-be2f-b163bc95d2d8', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '05B46374-0B1C-4735-A255-C2972A217B0D') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('05B46374-0B1C-4735-A255-C2972A217B0D', '714', 'ICT FOR T.B', 181.00, NULL, 'Negative', NULL, 1, '{}', '4cbd64a6-797f-4dba-9f44-3d09d88c7601', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '509A330D-5E4D-46D0-B423-C29E1F82D9A3') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('509A330D-5E4D-46D0-B423-C29E1F82D9A3', '701', 'Blood Culture', 132.00, NULL, 'No growth', 'growth', 1, '{}', '876f98ec-f195-4d24-8751-1870937cdc07', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '6FC4D984-4882-44A3-9817-C4A2FB5A9082') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('6FC4D984-4882-44A3-9817-C4A2FB5A9082', 'TSH', '????? ????? ??????? - Thyroid Stimulating Hormone', 60.00, '?? ????? ????', '0.4-4.0 mIU/L', 'mIU/L', 1, '{}', '7BDE7846-17B3-4432-A816-51F876026E54', '2026-03-15 17:41:38.9700000', NULL);
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '86741953-73AA-42EF-BBE2-C50E4ED93DB1') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('86741953-73AA-42EF-BBE2-C50E4ED93DB1', '344', 'Anti.HAV(IgM)', 198.00, NULL, 'Non-reactive', 'reactive', 1, '{}', '1e6e8337-a9a5-4814-ae3d-0c7acead0c8c', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'EBCBC0C6-683A-4454-AB05-C8857B178D97') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('EBCBC0C6-683A-4454-AB05-C8857B178D97', '311', 'VDRL', 316.00, NULL, 'Non-reactive', 'reactive', 1, '{}', '0d90585c-26a8-4682-9dd6-79f174f060f0', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '81A31A13-7704-44E4-92A2-C8C177E4F1BB') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('81A31A13-7704-44E4-92A2-C8C177E4F1BB', '304', 'Anti cardiolipin (IgG)', 452.00, NULL, 'Negative (< 15 GPL U/mL)', NULL, 1, '{}', '88672a21-ab7c-49a4-8af8-3abb343182fe', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '2D9A0BEA-8B57-493D-A26A-C922F63D7B9F') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('2D9A0BEA-8B57-493D-A26A-C922F63D7B9F', '314', 'Widal For Brucella', 388.00, NULL, 'Negative (Titer < 1:80)', NULL, 1, '{}', '32a0a117-cc48-4dbb-81c3-64d5ca37e5d7', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '150B8681-995B-46F9-B2A2-C9B003A5ACAE') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('150B8681-995B-46F9-B2A2-C9B003A5ACAE', '502', 'Magnesium', 162.00, NULL, '1.7-2.2 mg/dL', 'mg/dL', 1, '{}', 'a9b8f4a8-0714-491f-88ae-a59279b3061a', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'BCBFF076-383C-4589-B90E-CB41651833C2') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('BCBFF076-383C-4589-B90E-CB41651833C2', '707', 'Other Specimen', 113.00, NULL, 'No growth / Normal flora', 'flora', 1, '{}', 'c6ca4bbb-45b9-4275-996b-c7d4b71c739a', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '6C7C615D-415E-4319-B743-CBC7522FA23C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('6C7C615D-415E-4319-B743-CBC7522FA23C', '334', 'CMV (IgM)', 171.00, NULL, 'Negative', NULL, 1, '{}', '3ebb769e-2cb8-40f2-aead-23414ca9fe64', '2026-04-05 09:16:18.7533333', '96107F49-2958-4C5E-99E1-A526190F8E8B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '36FE9A42-1594-4212-8ACA-CC31662C0432') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('36FE9A42-1594-4212-8ACA-CC31662C0432', '460', 'S. Amylase', 171.00, NULL, '30-110 U/L', 'U/L', 1, '{}', '62c8aaab-f1d7-4bae-a2e8-a7eab521a2c0', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0378911B-45CC-40DC-A17E-D1168FCDC94C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0378911B-45CC-40DC-A17E-D1168FCDC94C', '110', 'PT&INR', 117.00, NULL, 'PT: 11-13.5 sec / INR: 0.8-1.1', 'INR', 1, '{}', '2723a09d-48a4-49c8-8be2-1a9d4d36ef39', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'F39418DE-DE5A-4762-8B3D-D1220CC4987D') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('F39418DE-DE5A-4762-8B3D-D1220CC4987D', '507', 'Spot Urine for Protein /creatinine Ratio', 141.00, NULL, '< 0.2 mg/mg', 'mg/mg', 1, '{}', '9bef1628-2c3c-44a7-abd9-22cf0218486f', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'E7FE2571-1387-41F9-BD85-D2FA48210425') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('E7FE2571-1387-41F9-BD85-D2FA48210425', '351', 'S.Cortisol(pm)', 183.00, NULL, '3-16 mcg/dL', 'mcg/dL', 1, '{}', '6ba5efbc-9278-4a97-b15a-5f3f921475e5', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '99BF031A-E25A-4351-B0A8-D36FE6C9BC93') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('99BF031A-E25A-4351-B0A8-D36FE6C9BC93', '102', 'Hb+PCV', 64.00, NULL, 'Hb: 12.0-17.5 g/dL / PCV: 36-50%', NULL, 1, '{}', '51f0d021-ea72-42db-99cd-f49c6cea9ad6', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5003EFC2-DCF9-4206-AFC1-D37236441526') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5003EFC2-DCF9-4206-AFC1-D37236441526', '441', 'Triglyceride', 106.00, NULL, '< 150 mg/dL', 'mg/dL', 1, '{}', '4a85301f-f091-48f7-aea1-2f1d938fc2e0', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '144BDA98-A4AB-4C94-A12E-D3808C481ED5') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('144BDA98-A4AB-4C94-A12E-D3808C481ED5', '711', 'Skin-Nail-Hair Scraping', 116.00, NULL, 'Negative for fungal elements', NULL, 1, '{}', '60761e1a-f618-4de6-9503-dc7a07e624ae', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '9E1D7520-027E-4171-9F53-D3A02A4018A5') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('9E1D7520-027E-4171-9F53-D3A02A4018A5', '435', 'S. Albumin', 154.00, NULL, '3.5-5.0 g/dL', 'g/dL', 1, '{}', '210b1a80-45bf-4648-8ebb-ac4772dea693', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '8F0364A2-F7DF-4DCF-9411-D5A0963805A0') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('8F0364A2-F7DF-4DCF-9411-D5A0963805A0', '112', 'Fibrinogen Level', 117.00, NULL, '200-400 mg/dL', 'mg/dL', 1, '{}', '7f631c24-2209-4984-a17c-5c98ede21794', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'ECC578A1-6C13-4447-B590-D6571755ACF5') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('ECC578A1-6C13-4447-B590-D6571755ACF5', '451', 'CK-MB', 100.00, NULL, '< 3 ng/mL', 'ng/mL', 1, '{}', '2ffc238e-bfb0-4fa4-9a17-8b0d78b9d2ce', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'AC04D234-B0DE-4AC9-A37E-DC9298F4204B') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('AC04D234-B0DE-4AC9-A37E-DC9298F4204B', '302', 'Ds DNA (IgG/IgM/IgA)', 420.00, NULL, 'Negative', NULL, 1, '{}', '0d82f689-3135-4091-b440-d1be9c3e0b67', '2026-04-05 09:16:18.7500000', '5DC53105-E8D5-4923-8120-8DF5D777527B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '6DC24F8E-8918-4749-B844-DCB4FED34CC1') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('6DC24F8E-8918-4749-B844-DCB4FED34CC1', '430', 'LFTS', 119.00, NULL, 'Panel composite', 'composite', 1, '{}', '0d52d3ce-dbbf-434c-96a1-d7086d6147e3', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'E0FF8676-BAD5-4098-978F-DE75E9E791AC') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('E0FF8676-BAD5-4098-978F-DE75E9E791AC', 'FBS', '??? ???? ?????? - Fasting Blood Sugar', 30.00, '???? 8-12 ????', '70-100 mg/dL', 'mg/dL', 1, '{}', 'BE6340FE-29CC-4F5A-B90B-ADDBCC219AFC', '2026-03-15 17:41:38.9700000', NULL);
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '41E81DAD-0423-4ECB-8686-DEA11B218F08') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('41E81DAD-0423-4ECB-8686-DEA11B218F08', '101', 'Complete Blood Count(CBC)', 75.00, NULL, 'Varies by individual parameter', NULL, 1, '{}', 'c65ebe9c-0ee8-4f2a-b260-9f0a38f5b177', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5E5D5172-73B6-45CB-9BD7-DEDB10D6FEC8') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5E5D5172-73B6-45CB-9BD7-DEDB10D6FEC8', '406', 'U - Microalbumin', 114.00, NULL, '< 30 mg/24hr', 'hr', 1, '{}', '6b190abc-b132-4225-96a8-13a66b382f66', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '94F037B1-AC38-4498-A494-DF9B21B823F2') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('94F037B1-AC38-4498-A494-DF9B21B823F2', '106', 'Sickling Test', 64.00, NULL, 'Negative', NULL, 1, '{}', 'f58a4159-48c8-4b59-b82f-51b04be57872', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '1F7D143D-3F41-485F-AF44-E10283FF8AFA') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('1F7D143D-3F41-485F-AF44-E10283FF8AFA', '421', '24hr Urine Calcium', 175.00, NULL, '100-300 mg/24hr', 'hr', 1, '{}', '54dc889b-8949-4787-adaa-de6621cb3cae', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '13CFEF00-9D68-42B3-8599-E114FC31B7E8') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('13CFEF00-9D68-42B3-8599-E114FC31B7E8', '605', '(Ag) in stool', 152.00, NULL, 'Negative', NULL, 1, '{}', 'eca0f8d6-c89c-4a46-9238-a93cf87ad008', '2026-04-05 09:16:18.7566667', '36869772-F984-4CE2-B74B-B3B222EA6791');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '0CD25A88-1693-4FEE-9DEA-E37CF5420F4E') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('0CD25A88-1693-4FEE-9DEA-E37CF5420F4E', '511', 'Seminal fluid analysis', 141.00, NULL, 'Volume >1.5mL', 'mL', 1, '{}', '3ce5645d-74bf-4ba2-a2ef-ee3923d2d490', '2026-04-05 09:16:18.7566667', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '7E6C5AC7-ACC2-4AE5-9467-E4D05042C7E7') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('7E6C5AC7-ACC2-4AE5-9467-E4D05042C7E7', '438', 'GGT', 182.00, NULL, '9-48 U/L', 'U/L', 1, '{}', 'dcc41564-e830-4ec7-a8e1-b40088a31374', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '4EA13955-0421-46D9-88BF-E5BB6A3F1046') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('4EA13955-0421-46D9-88BF-E5BB6A3F1046', '404', 'GTT', 150.00, NULL, 'Fasting < 95; 1hr < 180; 2hr < 155 mg/dL', 'mg/dL', 1, '{}', '6cba8154-394b-4ddf-addc-466f7baf072e', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'FDD4DCEE-C87E-40F3-BA08-E6991AEF0C00') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('FDD4DCEE-C87E-40F3-BA08-E6991AEF0C00', '208', 'E2', 198.00, NULL, 'Varies by age/cycle phase', NULL, 1, '{}', 'e26ce8ab-2e0d-4663-be99-8dd46fcb01e6', '2026-04-05 09:16:18.7500000', 'EE188ECF-7356-4523-A6CF-C9C2027930D1');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '9B8D583A-277B-4F5F-B4BF-E7B52CF5F79C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('9B8D583A-277B-4F5F-B4BF-E7B52CF5F79C', '347', 'HBc Core Ag.', 195.00, NULL, 'Non-reactive', 'reactive', 1, '{}', '6f4a67e4-f8f4-408f-9b4d-07cda6d121ac', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'E1C0CB9D-B215-4ABA-9782-EB220761663C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('E1C0CB9D-B215-4ABA-9782-EB220761663C', '403', 'RBS', 152.00, NULL, '< 200 mg/dL', 'mg/dL', 1, '{}', '65f4e656-ba48-4c26-9d9e-69ece19d24c5', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '3DD31FAD-2DBE-4AAB-87D4-EDC325D67805') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('3DD31FAD-2DBE-4AAB-87D4-EDC325D67805', '122', 'T.I.B.C', 176.00, NULL, '240-450 mcg/dL', 'mcg/dL', 1, '{}', '53d4cf49-40b7-4183-88e7-c0eea5ca42b3', '2026-04-05 09:16:18.7500000', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'A000DAC5-9489-4877-A920-EEA52D0D2137') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('A000DAC5-9489-4877-A920-EEA52D0D2137', '109', 'Clotting Time (C.T)', 132.00, NULL, '8-15 minutes', 'minutes', 1, '{}', '4da94ab9-0eab-484a-8d80-0507a795b093', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'D8576705-29FB-4831-B19C-EF2D83E21B69') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('D8576705-29FB-4831-B19C-EF2D83E21B69', '123', 'Transferrin Saturation', 102.00, NULL, '20%-50%', NULL, 1, '{}', '4a463ffd-78a8-4239-b185-683b043a2cb5', '2026-04-05 09:16:18.7500000', 'BD7D1CF8-505A-4445-9C9C-BCD2C1A2FC91');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '93A387FF-328D-4FB9-9938-F0DAB0F5F0E6') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('93A387FF-328D-4FB9-9938-F0DAB0F5F0E6', '343', 'Anti.HCV.Abs', 132.00, NULL, 'Non-reactive', 'reactive', 1, '{}', 'f5f98bd4-5186-4e36-b751-0f01381bbd73', '2026-04-05 09:16:18.7533333', '6EAC0B3A-4C79-48DB-BBFB-C346177BC010');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '2FB16B85-9C1E-4F67-92D8-F130C4906AE0') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('2FB16B85-9C1E-4F67-92D8-F130C4906AE0', '217', 'Ca125', 110.00, NULL, '< 35 U/mL', 'U/mL', 1, '{}', 'ae7271f9-f591-4671-b23c-38250e4e4890', '2026-04-05 09:16:18.7500000', '907C225C-F5EA-4B01-98EB-19BDDCE49B59');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'A21B274C-772F-422C-AC21-F4E2A0CF9999') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('A21B274C-772F-422C-AC21-F4E2A0CF9999', '416', 'Phosphorus', 102.00, NULL, '2.5-4.5 mg/dL', 'mg/dL', 1, '{}', 'b3e2770b-25d3-4c79-9783-ed38e01eb414', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'A62012D6-5369-4F8B-AEB4-F5F5118DB415') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('A62012D6-5369-4F8B-AEB4-F5F5118DB415', '335', 'CMV(IgG)', 188.00, NULL, 'Negative (Positive indicates past exposure)', NULL, 1, '{}', '25f6c2c9-deba-4f12-839e-e34c7906cfc0', '2026-04-05 09:16:18.7533333', '96107F49-2958-4C5E-99E1-A526190F8E8B');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '5DB22A20-F291-47BA-9D99-F64DA293B432') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('5DB22A20-F291-47BA-9D99-F64DA293B432', '115', 'Direct Coomb''s Test', 116.00, NULL, 'Negative', NULL, 1, '{}', 'dcee6c53-4085-49d3-82f8-a8e0f67505dd', '2026-04-05 09:16:18.7500000', '38601FFB-BA88-4A31-96EC-EE172F2AA501');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = '4A3D3939-D453-482B-B3C2-FBE6FF097B2C') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('4A3D3939-D453-482B-B3C2-FBE6FF097B2C', '204', 'PTH', 290.00, NULL, '10-55 pg/mL', 'pg/mL', 1, '{}', '57cc0a85-5376-49da-b33c-c0feb9550193', '2026-04-05 09:16:18.7500000', 'E68A31EA-5B9E-4E07-A8B4-AD01C4A10C4F');
IF NOT EXISTS (SELECT 1 FROM AppLabTests WHERE Id = 'C25A407A-0F9E-4306-9683-FD4D091FA921') 
INSERT INTO AppLabTests (Id, Code, Name, Price, Instructions, ReferenceRange, Unit, IsActive, ExtraProperties, ConcurrencyStamp, CreationTime, CategoryId) 
VALUES ('C25A407A-0F9E-4306-9683-FD4D091FA921', '440', 'Cholesterol', 175.00, NULL, '< 200 mg/dL', 'mg/dL', 1, '{}', '9993d372-2fdd-4514-9ad0-aafc2164e61b', '2026-04-05 09:16:18.7533333', 'C2B77699-2EFC-4D23-B686-22449EA796BA');
GO
PRINT 'Lab Data Migration Completed Successfully.';
GO
