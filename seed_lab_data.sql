
-- Seed Lab Test Categories and Tests
BEGIN TRANSACTION;

-- Clean existing data if needed (Optional)
-- DELETE FROM [AppLabTests];
-- DELETE FROM [AppLabTestCategories];

INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('04a0cf9b-69f9-4202-8faa-9b86889b569d', GETDATE(), 0, '1', 'HAEMATOLOGY', NULL, 1, 1, '{}', '7e185cae-55a0-4ebf-b5b5-e1b0bcd6205a');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('b9562a93-aa8e-453c-a651-b42c6c2e1416', GETDATE(), 0, '2', 'IRON PROFILE', NULL, 2, 1, '{}', 'c517351f-69d9-48b0-950b-289ac402ec6f');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('8568bf8a-b570-437a-8951-fdb58974d3b2', GETDATE(), 0, 'ELECT', 'ELECTRO PHORESIS', 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 3, 1, '{}', 'fbfdd109-859c-4d91-b161-eaa4eda2c5bf');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('464b76fa-cc28-4138-ba46-eb267b65dc55', GETDATE(), 0, '3', 'ENDOCRINOLOGY THYROID F. TEST', NULL, 4, 1, '{}', 'f62a1662-731e-4fd8-8d9d-159ba5a6e818');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('b747a31e-47a7-4d39-8c74-9a5ed24a19ec', GETDATE(), 0, '4', 'FERTILITY', NULL, 5, 1, '{}', '1bda7f9b-e07a-4552-87f4-c8be7859ab90');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f4c16adf-1181-41d5-8015-6204dd96091f', GETDATE(), 0, '5', 'TUMOR MARKER', NULL, 6, 1, '{}', '25018a9e-88b8-4763-8798-59fbe5aae70c');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('25ceca2e-d90f-438e-b233-72f4b58d752c', GETDATE(), 0, '6', 'IMMUNOLOGY', NULL, 7, 1, '{}', 'c165d7af-824c-4c7c-b20c-89bbef050273');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('51f0f77b-20e3-4e1f-a719-4d19e631d08b', GETDATE(), 0, '7', 'TORCH PROFILE', NULL, 8, 1, '{}', '383c37af-4b84-4f38-b406-b015360f6fc3');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('9df0c9a3-207e-4236-96d9-bd28759c7b97', GETDATE(), 0, '8', 'VIROLOGY', NULL, 9, 1, '{}', '3f19fd86-1162-4575-8d8e-73915ecbddd0');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('290eebaf-c748-4072-90d6-4f06bf2da076', GETDATE(), 0, '9', 'SUPRA RENAL', NULL, 10, 1, '{}', '52181d84-06ce-46e5-b741-b3f7d0b20a05');
INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', GETDATE(), 0, 'H.PYL', 'H.PYLORI', '290eebaf-c748-4072-90d6-4f06bf2da076', 11, 1, '{}', '520deeac-37a3-4ef0-b4ae-950f3f9eb96d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('49d1968b-6758-4d58-864a-6c2844ad67eb', GETDATE(), 0, '101', 'Complete Blood Count(CBC)', 78, 'Varies by individual parameter', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '68293d63-ca6c-47fb-9545-6da05f6c2db7');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('49d1968b-6758-4d58-864a-6c2844ad67eb', GETDATE(), 0, '101', 'Complete Blood Count(CBC)', 2, 78, NULL, 'Varies by individual parameter', 1, '{}', 'a02d5490-cecb-4c9d-920a-569e52c43c7d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('2d76aed0-ccc6-4870-ab2b-89bc279d4fb5', GETDATE(), 0, '102', 'Hb+PCV', 130, 'Hb: 12.0-17.5 g/dL / PCV: 36-50%', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'a137ecbc-974e-4bc7-b871-aa582f584ab3');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('2d76aed0-ccc6-4870-ab2b-89bc279d4fb5', GETDATE(), 0, '102', 'Hb+PCV', 2, 130, NULL, 'Hb: 12.0-17.5 g/dL / PCV: 36-50%', 1, '{}', '1b07421b-af04-4722-af27-b81cc33a45d1');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('c5adbf8d-1494-402c-82b6-2ad290c1692a', GETDATE(), 0, '103', 'TWBC & differential Count', 130, '4', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'e2402721-9b54-4de5-8513-0b019fee2caf');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('c5adbf8d-1494-402c-82b6-2ad290c1692a', GETDATE(), 0, '103', 'TWBC & differential Count', 2, 130, NULL, '4', 1, '{}', 'dcdef6a4-32e3-44e3-85a0-ac9eed99c633');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('e29473c5-dee1-4440-a117-ffb285257d4d', GETDATE(), 0, '104', 'ESR', 67, '0-20 mm/hr', 'mm/hr', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'e6552345-14b1-434c-b7ef-6b8cea7fcfad');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('e29473c5-dee1-4440-a117-ffb285257d4d', GETDATE(), 0, '104', 'ESR', 2, 67, 'mm/hr', '0-20 mm/hr', 1, '{}', '5410341b-2ec5-428b-a7ad-a2cd7c84325d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('b5dcf7bc-d5ad-4b58-afde-7d761fc1ff07', GETDATE(), 0, '105', 'Platelets Count', 147, '150', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '8672eb77-0041-4cde-b701-013d306e2693');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('b5dcf7bc-d5ad-4b58-afde-7d761fc1ff07', GETDATE(), 0, '105', 'Platelets Count', 2, 147, NULL, '150', 1, '{}', '0e0dc4d0-6364-4e2e-8ade-e67fd1b8dc93');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('13f69343-233a-4934-833a-bccae50b6750', GETDATE(), 0, '106', 'Sickling Test', 143, 'Negative', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '9252ca9c-1189-412f-a7f9-c951d9b9a70d');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('13f69343-233a-4934-833a-bccae50b6750', GETDATE(), 0, '106', 'Sickling Test', 2, 143, NULL, 'Negative', 1, '{}', 'c68288df-d0a8-4610-a9f4-8b160f3f0ca4');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('32fdb4f3-ab7a-4ebd-8d72-c04fd09f2d26', GETDATE(), 0, '107', 'Reticulocyte Count', 50, '0.5%-1.5%', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'fdbe0bd4-a443-4e43-b3aa-a45d9fd2210e');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('32fdb4f3-ab7a-4ebd-8d72-c04fd09f2d26', GETDATE(), 0, '107', 'Reticulocyte Count', 2, 50, NULL, '0.5%-1.5%', 1, '{}', '72d20c92-0b27-4c2c-aecb-2f6f2eb9df71');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('4a8e76d0-51d3-4f70-b6f8-829242ec76a2', GETDATE(), 0, '108', 'Bleeding Time B.T', 85, '2-7 minutes', 'minutes', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '6dd3e1b9-e07a-463f-a7c0-10ed342c0aa0');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('4a8e76d0-51d3-4f70-b6f8-829242ec76a2', GETDATE(), 0, '108', 'Bleeding Time B.T', 2, 85, 'minutes', '2-7 minutes', 1, '{}', '122f5020-2d88-4a66-baea-f181bd54111c');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('8194086f-ae7e-40ca-86ac-42645b6925ac', GETDATE(), 0, '109', 'Clotting Time (C.T)', 84, '8-15 minutes', 'minutes', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'd6884195-4ca5-408c-8d62-7ef87dc13add');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('8194086f-ae7e-40ca-86ac-42645b6925ac', GETDATE(), 0, '109', 'Clotting Time (C.T)', 2, 84, 'minutes', '8-15 minutes', 1, '{}', '367474c8-e146-4cf1-9e29-d62f795f0c6b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('0f02afcb-0788-4d10-8af4-3c95833e1e24', GETDATE(), 0, '110', 'PT&INR', 101, 'PT: 11-13.5 sec / INR: 0.8-1.1', 'INR', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'bd62fca5-8464-4cb4-8427-f988d82cb291');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('0f02afcb-0788-4d10-8af4-3c95833e1e24', GETDATE(), 0, '110', 'PT&INR', 2, 101, 'INR', 'PT: 11-13.5 sec / INR: 0.8-1.1', 1, '{}', '6e550570-648e-4175-b5fe-7b9dba49385a');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('6366d878-a0c1-406a-82f6-3c6419166e1d', GETDATE(), 0, '111', 'A.P.T.T', 113, '25-35 seconds', 'seconds', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'cc60800d-08b9-4e9d-9eb8-fa24ae6750d0');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('6366d878-a0c1-406a-82f6-3c6419166e1d', GETDATE(), 0, '111', 'A.P.T.T', 2, 113, 'seconds', '25-35 seconds', 1, '{}', 'b2c3d8fc-cba3-4d5b-8377-98eb5d42484d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('944f8b7b-a48e-4f63-a9d6-350a704efdc0', GETDATE(), 0, '112', 'Fibrinogen Level', 98, '200-400 mg/dL', 'mg/dL', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'ec67bbf2-31fe-4c38-8284-3b626b2123ec');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('944f8b7b-a48e-4f63-a9d6-350a704efdc0', GETDATE(), 0, '112', 'Fibrinogen Level', 2, 98, 'mg/dL', '200-400 mg/dL', 1, '{}', '80143149-848d-4b14-89ea-c5930088912b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d8ca7fd4-0f4d-4ec9-aed5-aee355f8cbe4', GETDATE(), 0, '113', 'Blood Group', 147, 'N/A (A/B/AB/O', 'A/B/AB/O', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '9df0570d-4a7b-4b3d-972d-4f382c8691ff');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d8ca7fd4-0f4d-4ec9-aed5-aee355f8cbe4', GETDATE(), 0, '113', 'Blood Group', 2, 147, 'A/B/AB/O', 'N/A (A/B/AB/O', 1, '{}', '23f4efb8-acd7-4ae2-a986-d909f258aa94');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('46b72bc8-644b-4350-ab18-f6eeb7a17847', GETDATE(), 0, '114', 'Stool for occult Blood', 88, 'Negative', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', 'b178c22d-fa28-49c0-b92d-f9dd9e396e42');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('46b72bc8-644b-4350-ab18-f6eeb7a17847', GETDATE(), 0, '114', 'Stool for occult Blood', 2, 88, NULL, 'Negative', 1, '{}', '63c68ab0-d04e-49f2-b2a0-f128da5e5fc4');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('a899580b-7cdb-4b06-be2f-de712aa13c07', GETDATE(), 0, '115', 'Direct Coomb''s Test', 134, 'Negative', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '94f01e4a-2f79-4f93-943d-c7f577e71d3f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('a899580b-7cdb-4b06-be2f-de712aa13c07', GETDATE(), 0, '115', 'Direct Coomb''s Test', 2, 134, NULL, 'Negative', 1, '{}', '95fafeaf-d3da-4dd7-9cd3-21395ed6fcc2');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('61a65f67-0b2e-4a6b-a14b-69af4b4cd082', GETDATE(), 0, '116', 'LE Cells', 141, 'Negative', NULL, '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '7b2e988e-95e4-4a9b-97cd-04770694dea1');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('61a65f67-0b2e-4a6b-a14b-69af4b4cd082', GETDATE(), 0, '116', 'LE Cells', 2, 141, NULL, 'Negative', 1, '{}', '31f787b6-b799-4956-b6db-06239e450770');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('32f5c3f8-09ad-4d0a-a58d-400450d331a8', GETDATE(), 0, '117', 'B.M Biopsy & Aspiration', 138, 'Pathologist report dependent', 'dependent', '04a0cf9b-69f9-4202-8faa-9b86889b569d', 1, '{}', '6461aa76-bba1-4f36-9dae-811088ae3832');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('32f5c3f8-09ad-4d0a-a58d-400450d331a8', GETDATE(), 0, '117', 'B.M Biopsy & Aspiration', 2, 138, 'dependent', 'Pathologist report dependent', 1, '{}', '42a3cfdf-ce7a-466a-8db8-35ea376b41a3');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('bdf916e8-654a-492c-a19f-6f90d07378c3', GETDATE(), 0, '120', 'Serum Iron', 124, '60-170 mcg/dL', 'mcg/dL', 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 1, '{}', 'e2df057c-616f-4b6d-8daa-4b4aa254190e');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('bdf916e8-654a-492c-a19f-6f90d07378c3', GETDATE(), 0, '120', 'Serum Iron', 2, 124, 'mcg/dL', '60-170 mcg/dL', 1, '{}', '90b66af1-917f-4833-9249-21ca8fee9209');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('3487ea38-e6e0-4078-8857-9b979e7eec23', GETDATE(), 0, '121', 'S.Ferritin', 156, '12-300 ng/mL', 'ng/mL', 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 1, '{}', '3e649b89-c460-4b2b-a6fb-720fed42b27a');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('3487ea38-e6e0-4078-8857-9b979e7eec23', GETDATE(), 0, '121', 'S.Ferritin', 2, 156, 'ng/mL', '12-300 ng/mL', 1, '{}', '3b20292d-06f4-4df0-9232-7343dfa3c422');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('fd1f7abf-0dd4-4bb9-a7b4-233a0c146625', GETDATE(), 0, '122', 'T.I.B.C', 183, '240-450 mcg/dL', 'mcg/dL', 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 1, '{}', '2fd7af4e-e092-40c8-92e7-eb3a2174e81c');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('fd1f7abf-0dd4-4bb9-a7b4-233a0c146625', GETDATE(), 0, '122', 'T.I.B.C', 2, 183, 'mcg/dL', '240-450 mcg/dL', 1, '{}', 'ba735b4b-3f23-407f-b737-2a8d2a97f79c');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('025dc6b3-0c71-4f06-9d7f-915b77355be3', GETDATE(), 0, '123', 'Transferrin Saturation', 170, '20%-50%', NULL, 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 1, '{}', '6b493e7f-7a08-40b0-bb4f-b411d5b29dbe');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('025dc6b3-0c71-4f06-9d7f-915b77355be3', GETDATE(), 0, '123', 'Transferrin Saturation', 2, 170, NULL, '20%-50%', 1, '{}', '1cd4f148-c58a-47b3-8afa-afd36f3ab8a5');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('75544f69-8b71-4110-bc88-0688f4ea9e19', GETDATE(), 0, '124', 'VIT.B12', 171, '200-900 pg/mL', 'pg/mL', 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 1, '{}', '66ec2870-5d23-45eb-a676-bcb2cca71330');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('75544f69-8b71-4110-bc88-0688f4ea9e19', GETDATE(), 0, '124', 'VIT.B12', 2, 171, 'pg/mL', '200-900 pg/mL', 1, '{}', '64b4c51f-6049-43e9-8497-33ef91c0bba4');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('bf6e9cfb-2b70-4745-994e-915c1b39d108', GETDATE(), 0, '125', 'Folic Acid', 163, '2.7-17.0 ng/mL', 'ng/mL', 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 1, '{}', 'e0e5f5b3-0b43-44ea-9f86-c92d2b9cb5bb');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('bf6e9cfb-2b70-4745-994e-915c1b39d108', GETDATE(), 0, '125', 'Folic Acid', 2, 163, 'ng/mL', '2.7-17.0 ng/mL', 1, '{}', '251e012b-7429-4c6e-9224-55ccc32f6cf3');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('84f1aa50-47c2-45c8-96e1-25eeb4f41b2b', GETDATE(), 0, '126', 'VIT.D.T', 183, '20-50 ng/mL', 'ng/mL', 'b9562a93-aa8e-453c-a651-b42c6c2e1416', 1, '{}', 'ed61162c-2290-4bc8-b4f6-8a494d941857');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('84f1aa50-47c2-45c8-96e1-25eeb4f41b2b', GETDATE(), 0, '126', 'VIT.D.T', 2, 183, 'ng/mL', '20-50 ng/mL', 1, '{}', '945f5e64-f064-4238-a78c-9d207f059153');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('afe8dd2e-c49f-407f-b573-525042aec5ea', GETDATE(), 0, '130', 'Protein', 175, 'Total: 6.0-8.3 g/dL', 'g/dL', '8568bf8a-b570-437a-8951-fdb58974d3b2', 1, '{}', '4c623015-fd96-4136-8fca-68dfb2b2fade');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('afe8dd2e-c49f-407f-b573-525042aec5ea', GETDATE(), 0, '130', 'Protein', 2, 175, 'g/dL', 'Total: 6.0-8.3 g/dL', 1, '{}', '21ea7734-62d4-4a21-bba2-5b98fdd9e52a');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('30ea4072-7c24-4cd8-aaa5-259ccaf5eaf6', GETDATE(), 0, '131', 'Haemoglobin', 197, 'HbA > 95%', NULL, '8568bf8a-b570-437a-8951-fdb58974d3b2', 1, '{}', 'c184ea22-3ed9-48cf-87a8-4ec7bde8a815');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('30ea4072-7c24-4cd8-aaa5-259ccaf5eaf6', GETDATE(), 0, '131', 'Haemoglobin', 2, 197, NULL, 'HbA > 95%', 1, '{}', '915bbb93-8813-497c-8103-ca4f1d279990');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('01c69c61-3e36-4897-9d00-ce757c59b7ef', GETDATE(), 0, '132', 'immuno electrophoresis', 155, 'Normal pattern (Pathologist dependent)', 'dependent', '8568bf8a-b570-437a-8951-fdb58974d3b2', 1, '{}', '04d1d845-130c-4292-aa32-0d4823edccd8');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('01c69c61-3e36-4897-9d00-ce757c59b7ef', GETDATE(), 0, '132', 'immuno electrophoresis', 2, 155, 'dependent', 'Normal pattern (Pathologist dependent)', 1, '{}', 'f6860994-a2c5-480e-899d-a349c46efeff');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f2f7c106-bdf8-466c-8945-8d80ca3c1277', GETDATE(), 0, '201', 'TSH', 440, '0.4-4.0 mIU/L', 'mIU/L', '464b76fa-cc28-4138-ba46-eb267b65dc55', 1, '{}', '0d0d3146-99fd-46a8-8fa1-68f7b2d40387');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('f2f7c106-bdf8-466c-8945-8d80ca3c1277', GETDATE(), 0, '201', 'TSH', 2, 440, 'mIU/L', '0.4-4.0 mIU/L', 1, '{}', '0851be59-daa3-48bc-81ee-587cd2b4ea56');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f6df114d-e559-4c1b-8cbf-d3a5f2a6ef01', GETDATE(), 0, '202', 'FT3', 330, '2.3-4.1 pg/mL', 'pg/mL', '464b76fa-cc28-4138-ba46-eb267b65dc55', 1, '{}', '58c1b6dc-fdd3-422a-ad74-d85fba971a46');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('f6df114d-e559-4c1b-8cbf-d3a5f2a6ef01', GETDATE(), 0, '202', 'FT3', 2, 330, 'pg/mL', '2.3-4.1 pg/mL', 1, '{}', '7cb41d64-722b-41bf-8e7b-169fb1329047');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('9a367f19-0f40-4de8-959d-3bcddf3ff209', GETDATE(), 0, '203', 'Ft4', 387, '0.9-1.7 ng/dL', 'ng/dL', '464b76fa-cc28-4138-ba46-eb267b65dc55', 1, '{}', '0961b813-5a62-4573-954b-f7282109ace5');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('9a367f19-0f40-4de8-959d-3bcddf3ff209', GETDATE(), 0, '203', 'Ft4', 2, 387, 'ng/dL', '0.9-1.7 ng/dL', 1, '{}', '2431db8b-c58e-412f-a988-766d4a6d2530');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('a74ecdfe-cde1-4331-ad1b-67f029eb2bb9', GETDATE(), 0, '204', 'PTH', 383, '10-55 pg/mL', 'pg/mL', '464b76fa-cc28-4138-ba46-eb267b65dc55', 1, '{}', 'b74c9ce0-c156-4cd8-a5cd-57692acc4eaa');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('a74ecdfe-cde1-4331-ad1b-67f029eb2bb9', GETDATE(), 0, '204', 'PTH', 2, 383, 'pg/mL', '10-55 pg/mL', 1, '{}', '7f5030d1-75bf-4434-aff8-0dcf81128cf3');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7b553f8b-a444-45f5-b01f-45115b36f6e2', GETDATE(), 0, '205', 'FSH', 115, 'Varies by age/cycle phase', NULL, 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', '2aa1bccb-f866-49c2-ac31-9522af7363fe');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7b553f8b-a444-45f5-b01f-45115b36f6e2', GETDATE(), 0, '205', 'FSH', 2, 115, NULL, 'Varies by age/cycle phase', 1, '{}', 'e5edc6ea-fe7c-4ffc-826e-02b79b091c91');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('cea30890-c686-4efb-8731-4d75ff3c7fc9', GETDATE(), 0, '206', 'Prolactin', 155, '< 25 ng/mL (Females) / < 17 ng/mL (Males)', 'Males', 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', '596ea031-a36e-4a9e-a93d-8fbe1f69642c');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('cea30890-c686-4efb-8731-4d75ff3c7fc9', GETDATE(), 0, '206', 'Prolactin', 2, 155, 'Males', '< 25 ng/mL (Females) / < 17 ng/mL (Males)', 1, '{}', 'ae6730e1-c363-4251-b7b1-fb7e413dba5b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('70f40bae-a7e9-48da-9103-dbacc398a1f9', GETDATE(), 0, '207', 'LH', 100, 'Varies by age/cycle phase', NULL, 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', '100f9d6f-6d96-4810-b5bc-d670d885210b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('70f40bae-a7e9-48da-9103-dbacc398a1f9', GETDATE(), 0, '207', 'LH', 2, 100, NULL, 'Varies by age/cycle phase', 1, '{}', 'f9a9b004-79b4-465b-ade1-2bc4f2a12872');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f5ba8b83-eaa1-409f-9086-07b6bb38378e', GETDATE(), 0, '208', 'E2', 192, 'Varies by age/cycle phase', NULL, 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', 'f3ae279b-14ed-44b6-919d-c8b44aeda7f6');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('f5ba8b83-eaa1-409f-9086-07b6bb38378e', GETDATE(), 0, '208', 'E2', 2, 192, NULL, 'Varies by age/cycle phase', 1, '{}', '12841bc1-0dbf-4121-89bc-3269b8a38490');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('026eb482-6fe3-4d45-9ff5-9824eff08f5f', GETDATE(), 0, '209', 'Testosterone', 147, '300-1000 ng/dL (M) / 15-70 ng/dL (F)', NULL, 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', 'a037fdb7-5153-48ae-9b20-422fa165d084');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('026eb482-6fe3-4d45-9ff5-9824eff08f5f', GETDATE(), 0, '209', 'Testosterone', 2, 147, NULL, '300-1000 ng/dL (M) / 15-70 ng/dL (F)', 1, '{}', '172a80b2-248f-4b7b-a4a4-5647b954bbe9');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('0e81e902-5b9a-4f8b-8d2f-bceeb9de1e33', GETDATE(), 0, '210', 'Progesterone', 166, 'Varies by cycle phase', NULL, 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', '48db57bb-63bf-405d-99a4-251a530ad935');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('0e81e902-5b9a-4f8b-8d2f-bceeb9de1e33', GETDATE(), 0, '210', 'Progesterone', 2, 166, NULL, 'Varies by cycle phase', 1, '{}', 'f9f523fd-1084-48a7-823e-e7cd21944e3d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d311ed34-6035-4da1-bb2e-84ade84faeb1', GETDATE(), 0, '211', 'B.HCG', 105, '< 5.0 mIU/mL (Non-pregnant)', 'pregnant', 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', '923b3c9e-5680-4631-946c-27603eaca586');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d311ed34-6035-4da1-bb2e-84ade84faeb1', GETDATE(), 0, '211', 'B.HCG', 2, 105, 'pregnant', '< 5.0 mIU/mL (Non-pregnant)', 1, '{}', '39f54cb8-22ca-41c6-bc34-daf201526498');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('dd1a58c8-0226-45f1-8aa7-9e1b7b8b8229', GETDATE(), 0, '212', 'AMH', 123, '1.0-4.0 ng/mL (Reproductive age females)', 'females', 'b747a31e-47a7-4d39-8c74-9a5ed24a19ec', 1, '{}', 'bf651e7d-ca2d-468d-bbd0-e9086756bdb8');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('dd1a58c8-0226-45f1-8aa7-9e1b7b8b8229', GETDATE(), 0, '212', 'AMH', 2, 123, 'females', '1.0-4.0 ng/mL (Reproductive age females)', 1, '{}', 'ae50b284-11d8-44f2-aa39-d169fc822529');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d0042df0-14ed-4182-ad07-00cc59bd470c', GETDATE(), 0, '215', 'CEA', 195, '< 3.0 ng/mL', 'ng/mL', 'f4c16adf-1181-41d5-8015-6204dd96091f', 1, '{}', 'f2bc2c8b-6760-4301-ac02-504af89f9cd7');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d0042df0-14ed-4182-ad07-00cc59bd470c', GETDATE(), 0, '215', 'CEA', 2, 195, 'ng/mL', '< 3.0 ng/mL', 1, '{}', 'ebfe761f-11b7-47c8-ac4f-19a989326356');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('07c02a79-92d6-460f-9b2b-d5382c7d2f49', GETDATE(), 0, '216', 'Ca15-3', 141, '< 30 U/mL', 'U/mL', 'f4c16adf-1181-41d5-8015-6204dd96091f', 1, '{}', '23884769-59c0-472f-a75a-787163b21510');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('07c02a79-92d6-460f-9b2b-d5382c7d2f49', GETDATE(), 0, '216', 'Ca15-3', 2, 141, 'U/mL', '< 30 U/mL', 1, '{}', 'f77be684-6762-46ce-9602-bd08ff336624');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d1fec3d6-be02-415a-b981-66ee2bbf9c01', GETDATE(), 0, '217', 'Ca125', 195, '< 35 U/mL', 'U/mL', 'f4c16adf-1181-41d5-8015-6204dd96091f', 1, '{}', '14034a9c-49b1-436a-aee3-d6fa68b72aa7');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d1fec3d6-be02-415a-b981-66ee2bbf9c01', GETDATE(), 0, '217', 'Ca125', 2, 195, 'U/mL', '< 35 U/mL', 1, '{}', 'ae915bee-3aca-455a-baa0-3863dbad8997');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('874d8563-e2ca-4a11-8b09-fb5f0dae1514', GETDATE(), 0, '218', 'CA19-9', 159, '< 37 U/mL', 'U/mL', 'f4c16adf-1181-41d5-8015-6204dd96091f', 1, '{}', 'ebe98be5-a73d-4fef-ac4d-2adfffbb7810');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('874d8563-e2ca-4a11-8b09-fb5f0dae1514', GETDATE(), 0, '218', 'CA19-9', 2, 159, 'U/mL', '< 37 U/mL', 1, '{}', 'b11a0ab3-598d-48e3-88c5-b8df213c1f58');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('10634001-61b6-48bb-b4ea-3a9941b9d80c', GETDATE(), 0, '219', 'Total PSA', 175, '< 4.0 ng/mL', 'ng/mL', 'f4c16adf-1181-41d5-8015-6204dd96091f', 1, '{}', '70826ead-6662-4edc-8cf4-a86acc3136e9');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('10634001-61b6-48bb-b4ea-3a9941b9d80c', GETDATE(), 0, '219', 'Total PSA', 2, 175, 'ng/mL', '< 4.0 ng/mL', 1, '{}', 'adf1603e-e027-4514-813d-b64a33450fee');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('25cbf23d-38da-483d-b473-bc3447c4ed2e', GETDATE(), 0, '220', 'Free PSA', 198, 'Ratio > 25% generally indicates lower risk', 'risk', 'f4c16adf-1181-41d5-8015-6204dd96091f', 1, '{}', '73614920-e620-47bf-8490-317a75c847cc');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('25cbf23d-38da-483d-b473-bc3447c4ed2e', GETDATE(), 0, '220', 'Free PSA', 2, 198, 'risk', 'Ratio > 25% generally indicates lower risk', 1, '{}', '2b92e167-909b-4519-9b8e-a9ca5f8bd2f8');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('9fd5348e-2473-49fc-9b42-8f5c3a9e9675', GETDATE(), 0, '221', 'a.Feto Protein', 163, '< 10 ng/mL', 'ng/mL', 'f4c16adf-1181-41d5-8015-6204dd96091f', 1, '{}', 'caa815a8-a633-4a88-8b9e-ec30dce69f8b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('9fd5348e-2473-49fc-9b42-8f5c3a9e9675', GETDATE(), 0, '221', 'a.Feto Protein', 2, 163, 'ng/mL', '< 10 ng/mL', 1, '{}', '3f27b2c1-a098-4d31-86dc-632d2e5beeb8');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('dd4ee113-36ef-4d0a-b1ac-e29ae2216519', GETDATE(), 0, '301', 'ANA(ANF)', 441, 'Negative (< 1:40 titer)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'aeb22015-12e1-4e14-8a9f-0e7e59478b75');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('dd4ee113-36ef-4d0a-b1ac-e29ae2216519', GETDATE(), 0, '301', 'ANA(ANF)', 2, 441, NULL, 'Negative (< 1:40 titer)', 1, '{}', 'd5abd0a7-bd6a-4a1a-8e5f-72ea4b63be42');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('a16d1f9d-d512-4d9a-a6dd-048381a6edbd', GETDATE(), 0, '302', 'Ds DNA (IgG/IgM/IgA)', 374, 'Negative', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '4ddbe532-77aa-4c98-b1ba-2b69e3571857');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('a16d1f9d-d512-4d9a-a6dd-048381a6edbd', GETDATE(), 0, '302', 'Ds DNA (IgG/IgM/IgA)', 2, 374, NULL, 'Negative', 1, '{}', 'b0f397a1-437e-4dc1-8bf5-dda4ec53973f');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('547d35eb-a483-419e-b2f4-21dd35006f54', GETDATE(), 0, '303', 'Anti Cardiolipin IgM', 365, 'Negative (< 12 MPL U/mL)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '6b780dc6-e85c-499a-9769-e8f5a3b59371');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('547d35eb-a483-419e-b2f4-21dd35006f54', GETDATE(), 0, '303', 'Anti Cardiolipin IgM', 2, 365, NULL, 'Negative (< 12 MPL U/mL)', 1, '{}', 'c796f463-bdcc-4c31-b440-5c2095e1d2c5');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('249eec79-684e-4735-9b64-9c9a4e89a3cf', GETDATE(), 0, '304', 'Anti cardiolipin (IgG)', 500, 'Negative (< 15 GPL U/mL)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '6442d203-d273-4c9d-9631-396ee3fc6fd4');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('249eec79-684e-4735-9b64-9c9a4e89a3cf', GETDATE(), 0, '304', 'Anti cardiolipin (IgG)', 2, 500, NULL, 'Negative (< 15 GPL U/mL)', 1, '{}', '2d5141a8-b991-4c7d-9346-9b976384f97f');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('9a5829d9-b46d-4e30-ac33-81f87dc625cb', GETDATE(), 0, '305', 'Anti Phospholipid IgM', 472, 'Negative', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '137ba34a-7e3e-4088-b922-af9168ce1fa0');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('9a5829d9-b46d-4e30-ac33-81f87dc625cb', GETDATE(), 0, '305', 'Anti Phospholipid IgM', 2, 472, NULL, 'Negative', 1, '{}', '2242d31c-a2ca-4d23-b12a-6131d5e47275');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ff27aafe-b164-4ddc-bd28-c645913eb1a9', GETDATE(), 0, '306', 'Anti Phospholipid IgG', 345, 'Negative', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'd303f843-93aa-41ec-8eb8-5260755bdd5b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ff27aafe-b164-4ddc-bd28-c645913eb1a9', GETDATE(), 0, '306', 'Anti Phospholipid IgG', 2, 345, NULL, 'Negative', 1, '{}', '36f92d68-b9ed-45c6-b4ab-611fe7d75158');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('709f8afa-c5b0-4b6a-903e-39cb7a300800', GETDATE(), 0, '307', 'Anti-T.T-G(IgG)', 332, 'Negative (< 15 U/mL)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'ac999839-42f9-4940-a443-c0a22b4eca23');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('709f8afa-c5b0-4b6a-903e-39cb7a300800', GETDATE(), 0, '307', 'Anti-T.T-G(IgG)', 2, 332, NULL, 'Negative (< 15 U/mL)', 1, '{}', '9d55baca-ed59-479f-9358-389d54bedf90');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('c7b0ae3a-9b5b-4bdb-bf4b-4ea7ca777463', GETDATE(), 0, '308', 'Anti Gliadin IgG', 421, 'Negative (< 15 U/mL)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '6b97dcbe-667d-454f-be32-51e2624a73a3');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('c7b0ae3a-9b5b-4bdb-bf4b-4ea7ca777463', GETDATE(), 0, '308', 'Anti Gliadin IgG', 2, 421, NULL, 'Negative (< 15 U/mL)', 1, '{}', '2db735f1-8fbe-434a-bc4b-77a0c2cd2e53');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('2de3c26e-6868-42b4-892a-cb0535ad4484', GETDATE(), 0, '309', 'Rheumatoid Factor', 367, 'Negative (< 14 IU/mL)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'cc00052d-f01f-4b47-98fa-4864718f8abd');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('2de3c26e-6868-42b4-892a-cb0535ad4484', GETDATE(), 0, '309', 'Rheumatoid Factor', 2, 367, NULL, 'Negative (< 14 IU/mL)', 1, '{}', 'c56e56f7-d7d3-46c4-ad2a-1bf36cc7f9a2');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('3afc8f10-cb8d-4019-ade8-a2e16e0137c2', GETDATE(), 0, '310', 'C.R Protein', 428, '< 10 mg/L', 'mg/L', '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'ebec0c92-7c07-4523-a604-812eb7f60ecd');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('3afc8f10-cb8d-4019-ade8-a2e16e0137c2', GETDATE(), 0, '310', 'C.R Protein', 2, 428, 'mg/L', '< 10 mg/L', 1, '{}', '858bd6cd-fdea-485c-83e2-1d960bd73e3d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('e36cde3b-7922-4277-9288-f2773d5b2e2d', GETDATE(), 0, '311', 'VDRL', 326, 'Non-reactive', 'reactive', '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'e11a6579-eb4d-4eb5-9c40-b65f1e1e0053');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('e36cde3b-7922-4277-9288-f2773d5b2e2d', GETDATE(), 0, '311', 'VDRL', 2, 326, 'reactive', 'Non-reactive', 1, '{}', 'd659248f-57cb-468c-ba07-edb83922267e');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('273d9967-0c88-4b5d-a6c1-72dd241466d9', GETDATE(), 0, '312', 'ASO.Titer', 328, '< 200 IU/mL', 'IU/mL', '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '1aad1928-9faf-48f7-996a-09ea8a247cdf');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('273d9967-0c88-4b5d-a6c1-72dd241466d9', GETDATE(), 0, '312', 'ASO.Titer', 2, 328, 'IU/mL', '< 200 IU/mL', 1, '{}', '569404cd-e140-4e79-84f4-94b891f4858b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('b7ef0251-8966-4d7c-ae5a-9890c21fbd8e', GETDATE(), 0, '313', 'Widal for Typhoid test', 500, 'Negative (Titer < 1:80)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'b86c333d-5773-4ef4-a01b-fbbdc5c1ccea');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('b7ef0251-8966-4d7c-ae5a-9890c21fbd8e', GETDATE(), 0, '313', 'Widal for Typhoid test', 2, 500, NULL, 'Negative (Titer < 1:80)', 1, '{}', 'af92870d-dc20-48dc-b9e4-247bc5242548');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('1422a017-4a1e-4f66-b812-7005b8bfa966', GETDATE(), 0, '314', 'Widal For Brucella', 378, 'Negative (Titer < 1:80)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'a4d34690-836e-40fc-93e8-f75d19ad8f07');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('1422a017-4a1e-4f66-b812-7005b8bfa966', GETDATE(), 0, '314', 'Widal For Brucella', 2, 378, NULL, 'Negative (Titer < 1:80)', 1, '{}', 'a1eda713-0aa9-4110-a254-545abadb7044');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('6c5fd4e4-68d5-4710-8773-40d9c49ab687', GETDATE(), 0, '315', 'Pregnancy Test', 424, 'Negative (if not pregnant)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '8acab410-902a-45dd-9580-ab6cb51a4be0');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('6c5fd4e4-68d5-4710-8773-40d9c49ab687', GETDATE(), 0, '315', 'Pregnancy Test', 2, 424, NULL, 'Negative (if not pregnant)', 1, '{}', 'd1ad3571-d88c-4e57-be5e-41f46c64b6cd');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('3e9a3196-1d59-44f4-a14e-fc0ba8acb55a', GETDATE(), 0, '316', 'IGE', 322, '< 100 kU/L', 'kU/L', '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '97e7c42a-1e4d-4083-917e-396dc76fd100');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('3e9a3196-1d59-44f4-a14e-fc0ba8acb55a', GETDATE(), 0, '316', 'IGE', 2, 322, 'kU/L', '< 100 kU/L', 1, '{}', 'c29bfd43-b6b1-41b4-bf0a-b7e11f9b3627');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ea86491a-8d67-4bfd-a778-8ffe53043e0f', GETDATE(), 0, '317', 'Stool for typhoid and Para Typhi Ags', 255, 'Negative', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'c3b7c6be-b615-423d-b6a5-eec9f5afa647');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ea86491a-8d67-4bfd-a778-8ffe53043e0f', GETDATE(), 0, '317', 'Stool for typhoid and Para Typhi Ags', 2, 255, NULL, 'Negative', 1, '{}', '7c094a66-f22e-4e88-ada9-41ea19ffe07e');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('8ac057ca-2cc1-455b-a748-39fcad542e53', GETDATE(), 0, '318', 'C3', 352, '80-160 mg/dL', 'mg/dL', '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '27989b06-749c-4701-b478-2e95b78be455');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('8ac057ca-2cc1-455b-a748-39fcad542e53', GETDATE(), 0, '318', 'C3', 2, 352, 'mg/dL', '80-160 mg/dL', 1, '{}', '382f8509-048c-45a1-af93-9e0b63dadace');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('0e4f397b-477d-4717-95de-4e31a51f3146', GETDATE(), 0, '319', 'C4', 348, '15-45 mg/dL', 'mg/dL', '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'a29987ea-430a-4b27-aba5-9c6270dcb9e2');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('0e4f397b-477d-4717-95de-4e31a51f3146', GETDATE(), 0, '319', 'C4', 2, 348, 'mg/dL', '15-45 mg/dL', 1, '{}', '9e6150e6-967e-466c-a25b-be58944af4af');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('3156673d-3f7f-457b-8501-74c20ccd93c4', GETDATE(), 0, '320', 'Anti CCP', 307, '< 20 u/mL (Negative)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'cf59e5c1-3e95-47bd-8cd8-9ddb51251474');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('3156673d-3f7f-457b-8501-74c20ccd93c4', GETDATE(), 0, '320', 'Anti CCP', 2, 307, NULL, '< 20 u/mL (Negative)', 1, '{}', 'c11ec093-4f6c-4545-9a84-b8e7f5e5dcd9');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('054504fc-4903-497d-a1a6-1580099798a5', GETDATE(), 0, '321', 'ANCA Profile', 391, 'Negative', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '7ced32d1-887e-426a-bb0d-2430bfb6b6f9');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('054504fc-4903-497d-a1a6-1580099798a5', GETDATE(), 0, '321', 'ANCA Profile', 2, 391, NULL, 'Negative', 1, '{}', '25e27e6f-db56-4eb6-9220-05eedd00c7fb');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('8d531741-da3f-4423-b03b-a5b7cd1b236e', GETDATE(), 0, '322', 'Beta 2-Glycoprotein', 412, 'Negative (< 20 SGU)', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', '4ee497a0-c2f9-48f2-a189-10825c87cdea');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('8d531741-da3f-4423-b03b-a5b7cd1b236e', GETDATE(), 0, '322', 'Beta 2-Glycoprotein', 2, 412, NULL, 'Negative (< 20 SGU)', 1, '{}', 'c97e13c9-df86-49a4-8f6b-60b9e34763c6');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d1780cad-929e-418a-a8b6-6435e19c0f6a', GETDATE(), 0, '323', 'ANA Profile', 456, 'Negative', NULL, '25ceca2e-d90f-438e-b233-72f4b58d752c', 1, '{}', 'aafe5a94-bbc8-41a7-9c0d-1853db02b0fa');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d1780cad-929e-418a-a8b6-6435e19c0f6a', GETDATE(), 0, '323', 'ANA Profile', 2, 456, NULL, 'Negative', 1, '{}', '62cef735-4ef6-4f68-adbb-02034dca39d2');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d26d07f5-3830-4611-ab45-e36d463d5662', GETDATE(), 0, '330', 'Toxo IgM', 158, 'Negative', NULL, '51f0f77b-20e3-4e1f-a719-4d19e631d08b', 1, '{}', '351fbb53-e536-42cc-af8e-e9693d172a34');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d26d07f5-3830-4611-ab45-e36d463d5662', GETDATE(), 0, '330', 'Toxo IgM', 2, 158, NULL, 'Negative', 1, '{}', 'c58a1c6e-fdce-4298-bb54-56858de4892f');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('67bcf8e4-82cf-46d4-a08c-af0efd94a1a6', GETDATE(), 0, '331', 'Toxo IgG', 192, 'Negative', NULL, '51f0f77b-20e3-4e1f-a719-4d19e631d08b', 1, '{}', 'c44de645-7ea6-4dd3-97b9-867a908d98b8');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('67bcf8e4-82cf-46d4-a08c-af0efd94a1a6', GETDATE(), 0, '331', 'Toxo IgG', 2, 192, NULL, 'Negative', 1, '{}', 'cc8b74e0-b455-4991-b623-0baac863d4cc');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('c4b07bd5-87ea-4fb4-ae59-47d9f5a4aa17', GETDATE(), 0, '332', 'Rubella IgM', 183, 'Negative', NULL, '51f0f77b-20e3-4e1f-a719-4d19e631d08b', 1, '{}', '9e94ba57-68e5-4d31-89a3-d12c0c9fd402');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('c4b07bd5-87ea-4fb4-ae59-47d9f5a4aa17', GETDATE(), 0, '332', 'Rubella IgM', 2, 183, NULL, 'Negative', 1, '{}', '19c0feeb-ebe0-4f37-b007-141741a2436f');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('b5a08300-f213-4353-843c-4d75929c0929', GETDATE(), 0, '333', 'Rubella IgG', 138, 'Negative (Positive indicates immunity)', NULL, '51f0f77b-20e3-4e1f-a719-4d19e631d08b', 1, '{}', 'c0ee9641-7331-42a8-84f0-e4d404753835');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('b5a08300-f213-4353-843c-4d75929c0929', GETDATE(), 0, '333', 'Rubella IgG', 2, 138, NULL, 'Negative (Positive indicates immunity)', 1, '{}', 'c11d3110-f1e0-46ac-87b5-b56b28189bec');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d5d1d9d8-9009-4acb-bb19-ede494240e1e', GETDATE(), 0, '334', 'CMV (IgM)', 103, 'Negative', NULL, '51f0f77b-20e3-4e1f-a719-4d19e631d08b', 1, '{}', '16bf6e57-6526-42c9-9996-b1349ac1f792');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d5d1d9d8-9009-4acb-bb19-ede494240e1e', GETDATE(), 0, '334', 'CMV (IgM)', 2, 103, NULL, 'Negative', 1, '{}', 'f4c065c5-e9cf-4d2d-8de5-bc091699feb7');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('c6dc20a1-8b13-42fb-acf7-3d9817cd3fae', GETDATE(), 0, '335', 'CMV(IgG)', 197, 'Negative (Positive indicates past exposure)', NULL, '51f0f77b-20e3-4e1f-a719-4d19e631d08b', 1, '{}', 'ab3c7a0e-a5a4-47c1-aa95-51c216043ae5');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('c6dc20a1-8b13-42fb-acf7-3d9817cd3fae', GETDATE(), 0, '335', 'CMV(IgG)', 2, 197, NULL, 'Negative (Positive indicates past exposure)', 1, '{}', '8cb14645-0ba4-4a69-8aad-28174d0cb2f9');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('17938371-fd13-44ef-bfda-e7de858b4977', GETDATE(), 0, '336', 'HSV-1&2IgG', 166, 'Negative', NULL, '51f0f77b-20e3-4e1f-a719-4d19e631d08b', 1, '{}', '070f65de-379f-4ad3-978b-ba3ab6cfae1e');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('17938371-fd13-44ef-bfda-e7de858b4977', GETDATE(), 0, '336', 'HSV-1&2IgG', 2, 166, NULL, 'Negative', 1, '{}', '191bc8c2-d73d-4fe0-abc7-7b56d80fa6dd');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('022651b8-4a89-4234-baed-11b31fac9a5f', GETDATE(), 0, '340', 'HBsAg', 144, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', 'ee2a7b02-3672-4ba5-b0ea-818c8e89321d');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('022651b8-4a89-4234-baed-11b31fac9a5f', GETDATE(), 0, '340', 'HBsAg', 2, 144, 'reactive', 'Non-reactive', 1, '{}', '29b039a3-f8b1-443a-925e-5c75b609c68e');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('24eb0953-4c0e-458f-a783-f8dfc064b38d', GETDATE(), 0, '341', 'HBe Ag', 155, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', '0f958f97-46f7-492f-9b16-c6a7a1976f5f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('24eb0953-4c0e-458f-a783-f8dfc064b38d', GETDATE(), 0, '341', 'HBe Ag', 2, 155, 'reactive', 'Non-reactive', 1, '{}', '63287356-6944-44e6-936e-1588c2b6c28d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('5339c7a0-9424-46e8-bca1-b7dcfcbe1fb9', GETDATE(), 0, '342', 'HEV Abs IgG/IgM', 190, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', 'e8ca6543-8a8e-4ce9-95c5-e318c576ba37');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('5339c7a0-9424-46e8-bca1-b7dcfcbe1fb9', GETDATE(), 0, '342', 'HEV Abs IgG/IgM', 2, 190, 'reactive', 'Non-reactive', 1, '{}', '02f1b902-c3a7-4e00-bd1c-ab391bf4c117');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('e9990c28-8128-4820-9b62-efa3ec35725e', GETDATE(), 0, '343', 'Anti.HCV.Abs', 131, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', '88b40116-9a31-4e12-a477-cc410907df22');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('e9990c28-8128-4820-9b62-efa3ec35725e', GETDATE(), 0, '343', 'Anti.HCV.Abs', 2, 131, 'reactive', 'Non-reactive', 1, '{}', 'ff2ba1cd-8080-4c5a-af7b-1083ec039b69');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ded0004f-94b2-4c6b-a8ba-9648b504d3be', GETDATE(), 0, '344', 'Anti.HAV(IgM)', 174, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', 'f2e63587-ff3f-42ab-b1db-5a3662b11c74');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ded0004f-94b2-4c6b-a8ba-9648b504d3be', GETDATE(), 0, '344', 'Anti.HAV(IgM)', 2, 174, 'reactive', 'Non-reactive', 1, '{}', '6dae819c-8773-4d82-acf4-572c3bbb1d33');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('9ac3d25a-8270-4cd0-a2e4-221a30295913', GETDATE(), 0, '345', 'HIV (1+2) Abs', 181, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', '380d0627-51e4-4e4e-8a36-bb8b61babe69');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('9ac3d25a-8270-4cd0-a2e4-221a30295913', GETDATE(), 0, '345', 'HIV (1+2) Abs', 2, 181, 'reactive', 'Non-reactive', 1, '{}', '87452e08-78e0-4764-beed-cd1582d97361');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('2bc19368-9920-46da-9469-984f30538e47', GETDATE(), 0, '346', 'AntiHBs Ag', 115, 'Positive (> 10 mIU/mL) indicates immunity', 'immunity', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', 'efa2dd95-b607-4ff2-a62f-050b8fd80852');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('2bc19368-9920-46da-9469-984f30538e47', GETDATE(), 0, '346', 'AntiHBs Ag', 2, 115, 'immunity', 'Positive (> 10 mIU/mL) indicates immunity', 1, '{}', 'd938e56d-c4b3-4812-bac5-d7044c805914');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('dc678852-4e45-4498-b8cf-2c77c8d9fac6', GETDATE(), 0, '347', 'HBc Core Ag.', 161, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', 'dc8e40dd-5a5b-4990-a6f8-3c05fb4be847');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('dc678852-4e45-4498-b8cf-2c77c8d9fac6', GETDATE(), 0, '347', 'HBc Core Ag.', 2, 161, 'reactive', 'Non-reactive', 1, '{}', 'd1bb5d68-7b8b-40bc-9a31-1e352e90bdb2');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('72a9a691-a056-47bb-987d-b2c422bec461', GETDATE(), 0, '348', 'HBe Ab.', 172, 'Non-reactive', 'reactive', '9df0c9a3-207e-4236-96d9-bd28759c7b97', 1, '{}', 'f4ce8369-96e6-4580-9bcb-01842ff79a3d');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('72a9a691-a056-47bb-987d-b2c422bec461', GETDATE(), 0, '348', 'HBe Ab.', 2, 172, 'reactive', 'Non-reactive', 1, '{}', '32c11e55-6763-4b80-b0ea-d4fede76db8a');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('a1c529ec-1312-4fe9-aac5-8811771009a9', GETDATE(), 0, '350', 'S.Cortisol(am)', 200, '5-23 mcg/dL', 'mcg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '1b7cf3be-01e9-4286-b8dd-3b9fdb8f3dec');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('a1c529ec-1312-4fe9-aac5-8811771009a9', GETDATE(), 0, '350', 'S.Cortisol(am)', 2, 200, 'mcg/dL', '5-23 mcg/dL', 1, '{}', '38acd156-3e06-4e13-8291-7300f9523f6d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ca653896-f45d-4399-822c-a67c3681cdcc', GETDATE(), 0, '351', 'S.Cortisol(pm)', 128, '3-16 mcg/dL', 'mcg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'ec9fac6b-52da-4c48-898b-90e9e6602c0d');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ca653896-f45d-4399-822c-a67c3681cdcc', GETDATE(), 0, '351', 'S.Cortisol(pm)', 2, 128, 'mcg/dL', '3-16 mcg/dL', 1, '{}', '093cfa86-7d83-494a-b257-5fe89def8464');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('24301f0b-f4e0-4763-b487-6307418c6397', GETDATE(), 0, '352', '24 Urine Cortisol(am)', 105, '10-100 mcg/24hr (Total daily)', 'daily', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '9c6086e7-adeb-43e9-b03a-9b5a9042e589');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('24301f0b-f4e0-4763-b487-6307418c6397', GETDATE(), 0, '352', '24 Urine Cortisol(am)', 2, 105, 'daily', '10-100 mcg/24hr (Total daily)', 1, '{}', 'dddb5d53-6f27-47a0-8287-4218a94f549c');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d8fe9804-96c0-42bd-aac8-08fc6ab09152', GETDATE(), 0, '353', '24 Urine.Cortisol(pm)', 193, 'Evaluated alongside AM result', 'result', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '42fb3e2b-e220-42e4-95db-49946e582391');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d8fe9804-96c0-42bd-aac8-08fc6ab09152', GETDATE(), 0, '353', '24 Urine.Cortisol(pm)', 2, 193, 'result', 'Evaluated alongside AM result', 1, '{}', '4f89f053-3b4b-42e3-92d0-a725be97c5d7');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7d462fd2-0fc7-45a6-a6ca-f2fb0f2bdfd6', GETDATE(), 0, '354', 'R.cortisol', 108, 'Baseline comparison', 'comparison', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'a5355d45-eabc-4daa-a7ef-e1e6e1aaa77e');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7d462fd2-0fc7-45a6-a6ca-f2fb0f2bdfd6', GETDATE(), 0, '354', 'R.cortisol', 2, 108, 'comparison', 'Baseline comparison', 1, '{}', 'dafd9afb-6d25-4bb5-ab91-1c6bbdf7097b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d7ea90ec-d846-4afb-8202-7cb3799f5c37', GETDATE(), 0, '401', 'FBS', 118, '70-99 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '7f36be67-2b4b-430b-86aa-1c36488b7950');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d7ea90ec-d846-4afb-8202-7cb3799f5c37', GETDATE(), 0, '401', 'FBS', 2, 118, 'mg/dL', '70-99 mg/dL', 1, '{}', 'c66cac85-b756-45be-901b-7e82ee5b97de');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('322f2c89-77f2-4854-a6bd-737e1ba2b5e5', GETDATE(), 0, '402', '2 Hrs After meal', 132, '< 140 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'f4db8196-f89b-45c3-9b93-569c838afaee');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('322f2c89-77f2-4854-a6bd-737e1ba2b5e5', GETDATE(), 0, '402', '2 Hrs After meal', 2, 132, 'mg/dL', '< 140 mg/dL', 1, '{}', 'e7bb90a8-f696-46d7-8302-25c5cf7fdbba');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('0491b211-9707-4495-b3ad-b7aac136f5a3', GETDATE(), 0, '403', 'RBS', 147, '< 200 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '895571a4-1316-406f-aa01-9923c410f831');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('0491b211-9707-4495-b3ad-b7aac136f5a3', GETDATE(), 0, '403', 'RBS', 2, 147, 'mg/dL', '< 200 mg/dL', 1, '{}', '37e6a501-e505-40ae-a475-590484167455');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f06eec33-48b6-4812-9abc-4b749ee2afc1', GETDATE(), 0, '404', 'GTT', 117, 'Fasting < 95; 1hr < 180; 2hr < 155 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '45d4824f-ac1e-446f-b95d-88a83e996b68');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('f06eec33-48b6-4812-9abc-4b749ee2afc1', GETDATE(), 0, '404', 'GTT', 2, 117, 'mg/dL', 'Fasting < 95; 1hr < 180; 2hr < 155 mg/dL', 1, '{}', '5e5df915-533d-4d43-90c4-313bc55c37c3');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('63ababf2-151e-456c-8a1f-2c0d43efc4ca', GETDATE(), 0, '405', 'HbA1c', 192, '4.0%-5.6% (Non-diabetic)', 'diabetic', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'e25edfea-86ef-4e94-aab4-2f19f73bd874');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('63ababf2-151e-456c-8a1f-2c0d43efc4ca', GETDATE(), 0, '405', 'HbA1c', 2, 192, 'diabetic', '4.0%-5.6% (Non-diabetic)', 1, '{}', 'd5d75682-103e-4417-afe0-1f19887d4c85');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('1db79d95-df69-4b4a-9d58-359a60fba48c', GETDATE(), 0, '406', 'U - Microalbumin', 169, '< 30 mg/24hr', 'hr', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '30a35cfb-b786-46aa-859d-a898a0b3fdb8');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('1db79d95-df69-4b4a-9d58-359a60fba48c', GETDATE(), 0, '406', 'U - Microalbumin', 2, 169, 'hr', '< 30 mg/24hr', 1, '{}', '9b365040-b289-4460-ab56-662e755a0477');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('fdc6be19-3b2a-45ea-9191-8d9641457aef', GETDATE(), 0, '407', 'P.P Blood Sugar', 147, '< 140 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '31e87873-04b2-4346-aa3c-c81dfb332dfc');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('fdc6be19-3b2a-45ea-9191-8d9641457aef', GETDATE(), 0, '407', 'P.P Blood Sugar', 2, 147, 'mg/dL', '< 140 mg/dL', 1, '{}', '8c7d9c92-6657-4192-9d3a-0ef6134e0ee6');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('2c7539c4-ef99-4076-bf5e-7c1b5a1bb4a7', GETDATE(), 0, '408', 'Fasting +2Hr 75 g Glucose', 155, '< 140 mg/dL (after 2 hours)', 'hours', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '0c7b38ac-991f-48a3-8c10-fe24154e6117');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('2c7539c4-ef99-4076-bf5e-7c1b5a1bb4a7', GETDATE(), 0, '408', 'Fasting +2Hr 75 g Glucose', 2, 155, 'hours', '< 140 mg/dL (after 2 hours)', 1, '{}', '88f4a011-f56d-44d5-bc9a-bcac6258ec48');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('5ae4f435-9389-428f-a50a-8a5b3f4c9999', GETDATE(), 0, '410', 'Blood Urea', 110, '7-20 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'efed684f-d736-452d-ac6e-ffaa53dd7c09');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('5ae4f435-9389-428f-a50a-8a5b3f4c9999', GETDATE(), 0, '410', 'Blood Urea', 2, 110, 'mg/dL', '7-20 mg/dL', 1, '{}', '1b05172a-6d0c-42cc-8bbc-2be06a37c529');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('e4c9d488-4687-4e39-a4ce-4ee5c6e05d69', GETDATE(), 0, '411', 'Serum Creatinine', 106, '0.6-1.2 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'a5d075d5-3711-4c23-88b4-3626ad0f3ed0');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('e4c9d488-4687-4e39-a4ce-4ee5c6e05d69', GETDATE(), 0, '411', 'Serum Creatinine', 2, 106, 'mg/dL', '0.6-1.2 mg/dL', 1, '{}', 'eee1ec53-7372-438d-8140-8cceded9a529');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('3c47a7ca-767a-4dc1-910c-83a199696411', GETDATE(), 0, '412', 'S. Na+', 100, '135-145 mEq/L', 'mEq/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '8519c048-3e09-4de9-9930-87335ee376ff');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('3c47a7ca-767a-4dc1-910c-83a199696411', GETDATE(), 0, '412', 'S. Na+', 2, 100, 'mEq/L', '135-145 mEq/L', 1, '{}', '9ba7a0a0-b8ac-467a-89f9-a6994e300bcb');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('46c8e0dd-4d7b-43ad-83e2-9b7d311f94cc', GETDATE(), 0, '413', 'S. K+', 189, '3.5-5.0 mEq/L', 'mEq/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'd0139098-29e9-403e-9fe0-4d1aca22babf');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('46c8e0dd-4d7b-43ad-83e2-9b7d311f94cc', GETDATE(), 0, '413', 'S. K+', 2, 189, 'mEq/L', '3.5-5.0 mEq/L', 1, '{}', '4132991e-d3e8-4a1c-8815-19347667bb66');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('a90fb29e-4471-44d8-9b46-4a8e9d0a3e3f', GETDATE(), 0, '414', 'S.Uric Acid', 137, '3.5-7.2 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '316bfd90-3f1d-4f07-93a7-bc6554ac810f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('a90fb29e-4471-44d8-9b46-4a8e9d0a3e3f', GETDATE(), 0, '414', 'S.Uric Acid', 2, 137, 'mg/dL', '3.5-7.2 mg/dL', 1, '{}', '3761a5c3-d42f-4d0f-a7f6-1ec08caaa875');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7f12deda-3a78-4f01-bce5-48b70b8ca1be', GETDATE(), 0, '415', 'Ca++', 154, '8.5-10.5 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '90282d2f-96c0-4319-a2fa-2f440fac14c8');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7f12deda-3a78-4f01-bce5-48b70b8ca1be', GETDATE(), 0, '415', 'Ca++', 2, 154, 'mg/dL', '8.5-10.5 mg/dL', 1, '{}', '21300cf3-8a02-4c0d-aa65-d22ad3adf8a2');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('3d41b026-5ff0-4935-89ee-b1e9b46c0f02', GETDATE(), 0, '416', 'Phosphorus', 112, '2.5-4.5 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'e475ce1d-048a-4f66-a81b-0bdb6b4c6cc9');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('3d41b026-5ff0-4935-89ee-b1e9b46c0f02', GETDATE(), 0, '416', 'Phosphorus', 2, 112, 'mg/dL', '2.5-4.5 mg/dL', 1, '{}', '13208d8d-bae5-48d7-9633-fb359d0d1ea0');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('1165aac2-a761-46b1-9b0c-6ed635e12d89', GETDATE(), 0, '417', 'Urine Na+', 154, '40-220 mEq/24hr', 'hr', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '797e1870-97b1-4faf-9337-e955a5efe2ba');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('1165aac2-a761-46b1-9b0c-6ed635e12d89', GETDATE(), 0, '417', 'Urine Na+', 2, 154, 'hr', '40-220 mEq/24hr', 1, '{}', 'b0a00b18-2d90-4a8e-aa8b-7cf465c0a6f9');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('221895e1-170a-4407-9f48-84873d49b02f', GETDATE(), 0, '418', 'urine k+', 126, '25-125 mEq/24hr', 'hr', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'f1355990-f1d0-4b59-abd0-090415558f7f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('221895e1-170a-4407-9f48-84873d49b02f', GETDATE(), 0, '418', 'urine k+', 2, 126, 'hr', '25-125 mEq/24hr', 1, '{}', '93f086a4-59e9-44f4-b07b-1a81d511970b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('0089e8a3-34cd-452e-97bb-827347c4d43e', GETDATE(), 0, '419', 'Creatinine Clearance', 168, '90-120 mL/min', 'mL/min', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'baf47f63-4e82-49eb-a03e-65242fd8dcfb');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('0089e8a3-34cd-452e-97bb-827347c4d43e', GETDATE(), 0, '419', 'Creatinine Clearance', 2, 168, 'mL/min', '90-120 mL/min', 1, '{}', 'cc43925f-c9b0-40c2-87f8-03b4903520ab');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('03885ba3-316b-4104-9ef0-b2f68adfebb7', GETDATE(), 0, '420', '24hr Urine Protein', 113, '< 150 mg/24hr', 'hr', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'b0ef5576-7338-4951-9cad-49d11a2cefb7');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('03885ba3-316b-4104-9ef0-b2f68adfebb7', GETDATE(), 0, '420', '24hr Urine Protein', 2, 113, 'hr', '< 150 mg/24hr', 1, '{}', '232572cc-e35d-452c-9f10-12dcc358fc41');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('12d5c424-012c-410a-afd6-500613d0a47c', GETDATE(), 0, '421', '24hr Urine Calcium', 146, '100-300 mg/24hr', 'hr', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'b459b5d7-0cd5-40d5-912d-7bc724580508');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('12d5c424-012c-410a-afd6-500613d0a47c', GETDATE(), 0, '421', '24hr Urine Calcium', 2, 146, 'hr', '100-300 mg/24hr', 1, '{}', '839e134b-acaa-4d4c-90e4-346b48272045');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('eb4e50cd-1af8-49ae-a96b-c944afcc0c58', GETDATE(), 0, '430', 'LFTS', 152, 'Panel composite', 'composite', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '236122b6-2994-4ef4-913d-932c1315692f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('eb4e50cd-1af8-49ae-a96b-c944afcc0c58', GETDATE(), 0, '430', 'LFTS', 2, 152, 'composite', 'Panel composite', 1, '{}', 'a01a6f08-3a75-40b7-ad4a-876345adeb71');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7b258e3b-5c5b-4f21-8200-905940c7eb9a', GETDATE(), 0, '431', 'AST (GOT)', 171, '8-40 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '17c02274-5113-45d0-a181-b4be89ca187f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7b258e3b-5c5b-4f21-8200-905940c7eb9a', GETDATE(), 0, '431', 'AST (GOT)', 2, 171, 'U/L', '8-40 U/L', 1, '{}', '0be2f9c4-e787-47bf-8500-60214bc0a42e');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('9cc2227e-f8fe-49b0-a535-2cab383e8e6f', GETDATE(), 0, '432', 'ALT (GPT)', 120, '7-56 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'ba0585cc-a14f-47de-ad96-f465592bd5c2');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('9cc2227e-f8fe-49b0-a535-2cab383e8e6f', GETDATE(), 0, '432', 'ALT (GPT)', 2, 120, 'U/L', '7-56 U/L', 1, '{}', 'dbca5fc3-6452-49d5-bbb1-0e0edf5202bc');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d6dd56b4-5605-40da-8c6b-21833628c9ea', GETDATE(), 0, '433', 'ALK. Phosphatase', 151, '44-147 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'ad41573e-2703-4b06-b064-04b6dbf195b3');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d6dd56b4-5605-40da-8c6b-21833628c9ea', GETDATE(), 0, '433', 'ALK. Phosphatase', 2, 151, 'U/L', '44-147 U/L', 1, '{}', '05d4c3f4-600e-4ebd-8b77-e78df8b14bd2');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('bfbaca6f-2930-4c6a-9dab-34e269b7fe6d', GETDATE(), 0, '434', 'T. Protein', 110, '6.0-8.3 g/dL', 'g/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '685715ae-d29f-421f-9d04-b175ffe53dcf');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('bfbaca6f-2930-4c6a-9dab-34e269b7fe6d', GETDATE(), 0, '434', 'T. Protein', 2, 110, 'g/dL', '6.0-8.3 g/dL', 1, '{}', 'dbe577bb-d2ad-4999-86ed-fc67368383ef');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('919d3aec-d922-4a06-9471-58a21e901364', GETDATE(), 0, '435', 'S. Albumin', 175, '3.5-5.0 g/dL', 'g/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'bd6555b5-b8cb-489f-b76b-6c55cd12ddd2');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('919d3aec-d922-4a06-9471-58a21e901364', GETDATE(), 0, '435', 'S. Albumin', 2, 175, 'g/dL', '3.5-5.0 g/dL', 1, '{}', '2d70d346-93d9-4257-b90b-b927f311d753');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('61e7099a-587a-4ee3-b409-c0802158c694', GETDATE(), 0, '436', 'Total Bilirubin', 156, '0.1-1.2 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '615b8b16-ae7c-448c-a1a5-9731f1aa8091');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('61e7099a-587a-4ee3-b409-c0802158c694', GETDATE(), 0, '436', 'Total Bilirubin', 2, 156, 'mg/dL', '0.1-1.2 mg/dL', 1, '{}', 'be3976aa-c861-47e1-96e8-cc99eb3c5572');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f66ef518-20e5-4043-8290-c6cc09685b4e', GETDATE(), 0, '437', 'D. Bilirubin', 150, '< 0.3 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'a5b61e19-b9a9-4c6a-aecd-43f0514fc64c');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('f66ef518-20e5-4043-8290-c6cc09685b4e', GETDATE(), 0, '437', 'D. Bilirubin', 2, 150, 'mg/dL', '< 0.3 mg/dL', 1, '{}', 'b5ddef0f-aae5-4615-8b83-a4a62d6bc121');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('58f62ffd-f9a9-426b-93ea-be22143f32ad', GETDATE(), 0, '438', 'GGT', 163, '9-48 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '21e05a9a-853a-4d5b-becf-7d8be30457fe');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('58f62ffd-f9a9-426b-93ea-be22143f32ad', GETDATE(), 0, '438', 'GGT', 2, 163, 'U/L', '9-48 U/L', 1, '{}', '41ca366d-6797-448a-84e4-46abbc3af0ea');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('9a12535e-cbe8-4dc7-be73-d2e58b3dbb4d', GETDATE(), 0, '439', 'Serum/CSF.lactate', 169, '0.5-1.0 mmol/L (Serum)', 'Serum', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'e7365983-cd40-4791-a1e0-7681ebd65c03');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('9a12535e-cbe8-4dc7-be73-d2e58b3dbb4d', GETDATE(), 0, '439', 'Serum/CSF.lactate', 2, 169, 'Serum', '0.5-1.0 mmol/L (Serum)', 1, '{}', '31704e9f-1da9-4e8a-a255-e1eea699ae70');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('65c83d70-f268-4985-910e-53569fb5892c', GETDATE(), 0, '440', 'Cholesterol', 111, '< 200 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '238e5bdf-dbf3-4468-8b15-72447c6f3119');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('65c83d70-f268-4985-910e-53569fb5892c', GETDATE(), 0, '440', 'Cholesterol', 2, 111, 'mg/dL', '< 200 mg/dL', 1, '{}', 'fd255842-97af-4a24-af28-f18f3fcda878');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('cacfd6d7-8e37-47b7-9b61-17ad34c38bed', GETDATE(), 0, '441', 'Triglyceride', 158, '< 150 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'b4d4d9b8-5275-426b-9be5-07666c8137fa');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('cacfd6d7-8e37-47b7-9b61-17ad34c38bed', GETDATE(), 0, '441', 'Triglyceride', 2, 158, 'mg/dL', '< 150 mg/dL', 1, '{}', 'b05ba092-567d-4318-a05c-424fd6acdf8a');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7bb80c03-eeec-4a5b-b105-50c52212cf64', GETDATE(), 0, '442', 'HDL- C', 164, '> 40 mg/dL (M) / > 50 mg/dL (F)', NULL, '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '2d86af37-7cdd-460d-b536-d998c5aa2a50');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7bb80c03-eeec-4a5b-b105-50c52212cf64', GETDATE(), 0, '442', 'HDL- C', 2, 164, NULL, '> 40 mg/dL (M) / > 50 mg/dL (F)', 1, '{}', '4cd3458f-7bd4-4172-93bb-bc29e707c328');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('5d8de91c-7c88-45b3-919e-37b41a20a93b', GETDATE(), 0, '443', 'LDL-C', 149, '< 100 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'ad4ae42a-375a-4463-aed0-97030ad2d43a');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('5d8de91c-7c88-45b3-919e-37b41a20a93b', GETDATE(), 0, '443', 'LDL-C', 2, 149, 'mg/dL', '< 100 mg/dL', 1, '{}', '9e52bed1-de28-4676-bff9-30eda4b79bfe');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('61196a3b-47d5-46a7-88c1-a1eea63ab23f', GETDATE(), 0, '450', 'Troponin', 106, '< 0.04 ng/mL (highly lab-specific)', 'specific', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'b503e9c8-b0ad-4e3f-9063-11e53ecd653c');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('61196a3b-47d5-46a7-88c1-a1eea63ab23f', GETDATE(), 0, '450', 'Troponin', 2, 106, 'specific', '< 0.04 ng/mL (highly lab-specific)', 1, '{}', '9966d941-4567-40ed-b8b0-52a6f70e969e');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('98d181b8-f109-42fe-9397-edfa7e7219f4', GETDATE(), 0, '451', 'CK-MB', 191, '< 3 ng/mL', 'ng/mL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'a649fd20-c64d-4806-9f7e-733613361d61');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('98d181b8-f109-42fe-9397-edfa7e7219f4', GETDATE(), 0, '451', 'CK-MB', 2, 191, 'ng/mL', '< 3 ng/mL', 1, '{}', '4baf6a04-ef62-43cd-91d2-9d021b6540fe');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('65aa629b-39a2-4556-a6bd-655cffe50e61', GETDATE(), 0, '452', 'T.CPK', 124, '22-198 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'c0e07010-3e33-4666-9f49-d103c60d4d57');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('65aa629b-39a2-4556-a6bd-655cffe50e61', GETDATE(), 0, '452', 'T.CPK', 2, 124, 'U/L', '22-198 U/L', 1, '{}', 'aef7ddb5-2c30-43ad-a27a-add202eff648');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('6d3d0b57-9878-45e0-b59c-e1021a70de1a', GETDATE(), 0, '453', 'LDH', 189, '140-280 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '4147dc79-a39e-461e-9625-03cb62238f3a');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('6d3d0b57-9878-45e0-b59c-e1021a70de1a', GETDATE(), 0, '453', 'LDH', 2, 189, 'U/L', '140-280 U/L', 1, '{}', '5b0f67b4-46b8-4ab0-8678-6273a5574fd6');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ff780f05-892f-42ef-b74b-5de9d14722d3', GETDATE(), 0, '454', 'D. Dimer', 196, '< 0.50 mg/L FEU', 'FEU', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'dde18eba-eb38-469f-8dcb-05e3b76277ba');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ff780f05-892f-42ef-b74b-5de9d14722d3', GETDATE(), 0, '454', 'D. Dimer', 2, 196, 'FEU', '< 0.50 mg/L FEU', 1, '{}', '8b49754a-2bd4-4ded-9d49-9ccd255b43c3');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7f0ddf4e-4414-42f9-b252-c6fcb6afbee7', GETDATE(), 0, '455', 'Pro.BNP', 123, '< 125 pg/mL', 'pg/mL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '9d8fe281-7e17-492b-9c78-331add0ffb40');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7f0ddf4e-4414-42f9-b252-c6fcb6afbee7', GETDATE(), 0, '455', 'Pro.BNP', 2, 123, 'pg/mL', '< 125 pg/mL', 1, '{}', 'b89877c0-9cee-47b9-b011-9f0f4471f6b2');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('6dd82f28-b008-46d1-a2f2-4e23fd8a188c', GETDATE(), 0, '460', 'S. Amylase', 175, '30-110 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '6de370a7-5032-4dc3-8d0e-fad76c22bfc6');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('6dd82f28-b008-46d1-a2f2-4e23fd8a188c', GETDATE(), 0, '460', 'S. Amylase', 2, 175, 'U/L', '30-110 U/L', 1, '{}', 'c93c832d-08ea-4087-a3bb-49d99a032217');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d896414f-604d-458f-8e06-012b945da29a', GETDATE(), 0, '461', 'U. Amylase', 120, '24-400 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'c0f91aa3-16ba-4753-8a29-256c04ab93df');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d896414f-604d-458f-8e06-012b945da29a', GETDATE(), 0, '461', 'U. Amylase', 2, 120, 'U/L', '24-400 U/L', 1, '{}', '7593361f-e80a-423b-90e4-cebe0cd7a9d9');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('780b3a3a-b29e-416c-97a8-83f96e7ad9f0', GETDATE(), 0, '462', 'S. Lipase', 146, '0-160 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'bf920001-a1ff-404e-a902-06874ee47952');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('780b3a3a-b29e-416c-97a8-83f96e7ad9f0', GETDATE(), 0, '462', 'S. Lipase', 2, 146, 'U/L', '0-160 U/L', 1, '{}', '2895c153-ebc3-408a-bef7-c57b1c9cf033');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('4df4a016-76d2-49d7-bc1b-54e9490f3b07', GETDATE(), 0, '501', 'Acid Phosphatase', 111, '0.1-0.5 U/L', 'U/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'b049852d-e45b-41fa-9108-4626510aa2a7');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('4df4a016-76d2-49d7-bc1b-54e9490f3b07', GETDATE(), 0, '501', 'Acid Phosphatase', 2, 111, 'U/L', '0.1-0.5 U/L', 1, '{}', 'd652719d-8d71-47bb-8382-a336757d3bf1');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('99be3dd1-bd22-4b69-abb1-cd6f8a7b1392', GETDATE(), 0, '502', 'Magnesium', 154, '1.7-2.2 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '73fd3bba-c6ec-4e79-88ff-1f2d46aff201');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('99be3dd1-bd22-4b69-abb1-cd6f8a7b1392', GETDATE(), 0, '502', 'Magnesium', 2, 154, 'mg/dL', '1.7-2.2 mg/dL', 1, '{}', 'e4699867-731b-4cbd-b14a-f8281405608f');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('803a7d23-f49b-4cc2-888c-3d3a7d80b36e', GETDATE(), 0, '503', 'Chloride', 159, '96-106 mEq/L', 'mEq/L', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '67ee6762-1e3e-4372-aac3-7434c93256e0');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('803a7d23-f49b-4cc2-888c-3d3a7d80b36e', GETDATE(), 0, '503', 'Chloride', 2, 159, 'mEq/L', '96-106 mEq/L', 1, '{}', '46702644-bf5a-4467-9abb-e6426d96d97c');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('967506b7-cb52-41d8-843e-1e8fbc30727d', GETDATE(), 0, '504', 'Urine for Bence Jon protein', 150, 'Negative', NULL, '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'f3c6b0e3-e8b0-4167-aab8-549c22b3cad1');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('967506b7-cb52-41d8-843e-1e8fbc30727d', GETDATE(), 0, '504', 'Urine for Bence Jon protein', 2, 150, NULL, 'Negative', 1, '{}', 'ef3f899f-501a-4a74-87af-98156dc58538');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('040a5c96-72b2-4d4e-a017-fddd33c862d7', GETDATE(), 0, '505', 'Blood Gases', 158, 'pH 7.35-7.45; pO2 75-100; pCO2 35-45', 'pCO', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '26aa2719-1bc4-45b4-9a63-dac040323328');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('040a5c96-72b2-4d4e-a017-fddd33c862d7', GETDATE(), 0, '505', 'Blood Gases', 2, 158, 'pCO', 'pH 7.35-7.45; pO2 75-100; pCO2 35-45', 1, '{}', '88687731-05fa-4766-9e42-305cfa5fe4a1');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('51164032-f166-4088-8b69-45a400a61799', GETDATE(), 0, '506', 'Body Fluid Analysis', 136, 'Variable based on fluid type', 'type', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'd8e6366c-8e7d-451b-9260-1c0758d40b8e');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('51164032-f166-4088-8b69-45a400a61799', GETDATE(), 0, '506', 'Body Fluid Analysis', 2, 136, 'type', 'Variable based on fluid type', 1, '{}', 'f25ebdba-b759-4917-a3b9-b43bc73af024');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('e2fff286-ca26-4ea8-83f9-27af0b4169a3', GETDATE(), 0, '507', 'Spot Urine for Protein /creatinine Ratio', 144, '< 0.2 mg/mg', 'mg/mg', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'b4d0beff-091e-439c-8683-7cacdc569578');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('e2fff286-ca26-4ea8-83f9-27af0b4169a3', GETDATE(), 0, '507', 'Spot Urine for Protein /creatinine Ratio', 2, 144, 'mg/mg', '< 0.2 mg/mg', 1, '{}', 'fa887ac9-6d2a-4cc7-a64b-0aa58fd689d5');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('81462484-0f27-4b75-9f50-0c922cb2acd7', GETDATE(), 0, '508', 'CSF Analysis', 193, 'Clear; Protein 15-45 mg/dL; Gluc 40-70 mg/dL', 'mg/dL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'a76c6c61-5598-4fa1-ae31-bf261d44984b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('81462484-0f27-4b75-9f50-0c922cb2acd7', GETDATE(), 0, '508', 'CSF Analysis', 2, 193, 'mg/dL', 'Clear; Protein 15-45 mg/dL; Gluc 40-70 mg/dL', 1, '{}', 'ce34a2af-7c8f-4383-a85e-bc53dbd187ed');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('2dbe84c0-dbd1-4d88-96eb-46b9f19bacde', GETDATE(), 0, '509', 'Serum effusion albumin gradient (SEAG)', 181, '< 1.1 g/dL (Exudate) / > 1.1 g/dL (Transudate)', 'Transudate', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '4205eb86-c6c3-4227-8665-322f4b74c15e');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('2dbe84c0-dbd1-4d88-96eb-46b9f19bacde', GETDATE(), 0, '509', 'Serum effusion albumin gradient (SEAG)', 2, 181, 'Transudate', '< 1.1 g/dL (Exudate) / > 1.1 g/dL (Transudate)', 1, '{}', '434f369b-b24e-4ecb-b83d-6da8e6dbf583');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('a901acd6-1275-465c-b2df-d87838e1f1a9', GETDATE(), 0, '510', 'Serum -Ascitic Albumin Gradient(SAAG)', 123, '< 1.1 g/dL (Non-portal HTN) / > 1.1 g/dL (Portal HTN)', 'HTN', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'ae7ddcc7-6779-41d1-a1c4-81ecb48cd04c');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('a901acd6-1275-465c-b2df-d87838e1f1a9', GETDATE(), 0, '510', 'Serum -Ascitic Albumin Gradient(SAAG)', 2, 123, 'HTN', '< 1.1 g/dL (Non-portal HTN) / > 1.1 g/dL (Portal HTN)', 1, '{}', '01e8e6d1-c97e-49c5-9efd-4fc7701dca40');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('de867d96-5ba4-4b84-aeb8-4fcc49c78541', GETDATE(), 0, '511', 'Seminal fluid analysis', 127, 'Volume >1.5mL', 'mL', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'b5b7b9df-7cfe-419b-ac32-63e210e9733a');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('de867d96-5ba4-4b84-aeb8-4fcc49c78541', GETDATE(), 0, '511', 'Seminal fluid analysis', 2, 127, 'mL', 'Volume >1.5mL', 1, '{}', 'a8819c6b-f58c-4fba-b8e5-49d325d5b11b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('a015763c-ed49-44f4-a99e-412ca9e49823', GETDATE(), 0, '512', 'Spot Urine for calcium/creatinine ratio', 198, '< 0.14 mg/mg', 'mg/mg', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'c9379299-b34c-4cd9-999d-07556c63b044');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('a015763c-ed49-44f4-a99e-412ca9e49823', GETDATE(), 0, '512', 'Spot Urine for calcium/creatinine ratio', 2, 198, 'mg/mg', '< 0.14 mg/mg', 1, '{}', 'f1fdd621-4b6e-4f25-ab77-7327d5629752');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('27123413-ecb9-446a-99b7-1012d430dac9', GETDATE(), 0, '513', 'Spot Urine for Albumin/creatinine ratio', 113, '< 30 mcg/mg', 'mcg/mg', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'dd4bd188-16cc-4a78-9b21-9e417fe3e53b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('27123413-ecb9-446a-99b7-1012d430dac9', GETDATE(), 0, '513', 'Spot Urine for Albumin/creatinine ratio', 2, 113, 'mcg/mg', '< 30 mcg/mg', 1, '{}', 'd7711d33-7f30-4ab7-aacc-3af0c24f295c');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7fa97299-68b1-4057-b827-cc0c6887b270', GETDATE(), 0, '601', 'Urine general', 101, 'Clear', 'Clear', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', 'fec2ed7c-e0a7-4e3e-a75d-0aedaa208b8b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7fa97299-68b1-4057-b827-cc0c6887b270', GETDATE(), 0, '601', 'Urine general', 2, 101, 'Clear', 'Clear', 1, '{}', '368ef08d-8702-41eb-985d-a8b0d0a2947a');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d27c4677-2a1a-4653-bf71-079ccd326e2d', GETDATE(), 0, '602', 'Stool Analysis', 116, 'Formed', 'Formed', '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '47c146b9-e682-4301-a424-eac18fc19e76');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d27c4677-2a1a-4653-bf71-079ccd326e2d', GETDATE(), 0, '602', 'Stool Analysis', 2, 116, 'Formed', 'Formed', 1, '{}', '43fee672-a5ad-4c87-93f2-6ba4842f1243');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f1ac202a-4a23-4416-942a-ab966cd11ac0', GETDATE(), 0, '603', 'Stool For Reducing Subs.', 165, 'Negative', NULL, '290eebaf-c748-4072-90d6-4f06bf2da076', 1, '{}', '34dbc692-b5e0-45dc-801f-80807c78c892');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('f1ac202a-4a23-4416-942a-ab966cd11ac0', GETDATE(), 0, '603', 'Stool For Reducing Subs.', 2, 165, NULL, 'Negative', 1, '{}', 'c2886feb-effc-42ce-9bfc-6b3eb53fac91');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('1a4f674f-02a5-4164-8923-56a16d3e6477', GETDATE(), 0, '605', '(Ag) in stool', 157, 'Negative', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '7f306cd6-62a2-442b-b2d7-dda8a978b181');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('1a4f674f-02a5-4164-8923-56a16d3e6477', GETDATE(), 0, '605', '(Ag) in stool', 2, 157, NULL, 'Negative', 1, '{}', '500800c4-245c-4b7e-bb62-362ef5e3c526');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('e3f8a3d4-757d-46e7-903b-50c0f5b137d4', GETDATE(), 0, '606', 'Urea Breath Test', 166, 'Negative', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '7c88a584-d282-4f4a-a828-ec07d8b20014');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('e3f8a3d4-757d-46e7-903b-50c0f5b137d4', GETDATE(), 0, '606', 'Urea Breath Test', 2, 166, NULL, 'Negative', 1, '{}', 'a4463575-6c19-4101-9660-77429842499b');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('49b222a5-4258-44f7-b5a9-d443104ba13c', GETDATE(), 0, '610', 'BF For Malaria', 176, 'Negative (No parasites seen)', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '0dcf03c9-8223-4739-a9b1-6275434b2c9d');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('49b222a5-4258-44f7-b5a9-d443104ba13c', GETDATE(), 0, '610', 'BF For Malaria', 2, 176, NULL, 'Negative (No parasites seen)', 1, '{}', '1679da9a-37f7-48e0-9446-04b44352e2c8');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('82d7ce5f-6a74-43cf-be10-c3d4a9eb996f', GETDATE(), 0, '611', 'ICT Malaria (Ag)', 106, 'Negative', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '5c6c493a-572d-4719-890a-2c07f0a444d1');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('82d7ce5f-6a74-43cf-be10-c3d4a9eb996f', GETDATE(), 0, '611', 'ICT Malaria (Ag)', 2, 106, NULL, 'Negative', 1, '{}', '6565d1c9-4221-4925-9703-5995de50fbec');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('25458831-c77f-4184-bcdc-c663ead95300', GETDATE(), 0, '612', 'QBC for Malaria', 182, 'Negative', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'a86a46a6-35ff-47ed-8292-dd7af687e9e4');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('25458831-c77f-4184-bcdc-c663ead95300', GETDATE(), 0, '612', 'QBC for Malaria', 2, 182, NULL, 'Negative', 1, '{}', '3fdb9381-f45b-4574-98a6-45dcdb9a23f9');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ede12023-7c7b-4140-b5f1-3fc090303c7d', GETDATE(), 0, '701', 'Blood Culture', 112, 'No growth', 'growth', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'db5ab31a-d45d-4336-b0d0-a77281608f0f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ede12023-7c7b-4140-b5f1-3fc090303c7d', GETDATE(), 0, '701', 'Blood Culture', 2, 112, 'growth', 'No growth', 1, '{}', '984d1907-4ef3-48dc-ad5a-d469c283e3c0');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('fc40a278-06b7-4f65-a10b-6b8c31a96ec9', GETDATE(), 0, '702', 'Urine For C/S', 124, '< 10', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'd84c4a3f-7bb3-4a7e-9535-68b3960df51b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('fc40a278-06b7-4f65-a10b-6b8c31a96ec9', GETDATE(), 0, '702', 'Urine For C/S', 2, 124, NULL, '< 10', 1, '{}', '7348f977-3f25-4422-b0b1-9578dd3c1e9d');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('b1c63c99-7cca-42da-ae4f-ca8e4197dd42', GETDATE(), 0, '703', 'Sputum', 113, 'Normal respiratory flora / No pathogens', 'pathogens', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '49b5e047-ace1-454e-8a4a-8f230505e80f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('b1c63c99-7cca-42da-ae4f-ca8e4197dd42', GETDATE(), 0, '703', 'Sputum', 2, 113, 'pathogens', 'Normal respiratory flora / No pathogens', 1, '{}', 'f5f479b4-43d2-47eb-ae79-06afc697b74f');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('c384c153-55de-4251-bec2-9c8ceb8a1109', GETDATE(), 0, '704', 'Pus', 119, 'No growth', 'growth', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'b98ab354-b427-488b-a10d-664e680a435e');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('c384c153-55de-4251-bec2-9c8ceb8a1109', GETDATE(), 0, '704', 'Pus', 2, 119, 'growth', 'No growth', 1, '{}', 'addd49d3-9736-4203-81a3-ad954bc3ea18');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ed857b4a-24b9-4be3-9cc0-e50940cd3e79', GETDATE(), 0, '705', 'High Vaginal Swab C/S', 167, 'Normal vaginal flora / No pathogens', 'pathogens', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'a246e56a-c0f5-468a-aa65-4d907d69e2cb');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ed857b4a-24b9-4be3-9cc0-e50940cd3e79', GETDATE(), 0, '705', 'High Vaginal Swab C/S', 2, 167, 'pathogens', 'Normal vaginal flora / No pathogens', 1, '{}', '12de1513-26eb-4885-8e12-b035213b0673');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('f88f988c-d682-4621-a2b8-9f3e466656b3', GETDATE(), 0, '706', 'Body Fluid C/S', 166, 'No growth (Sterile)', 'Sterile', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '04fc26b7-4730-4903-a25e-fe4b45510946');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('f88f988c-d682-4621-a2b8-9f3e466656b3', GETDATE(), 0, '706', 'Body Fluid C/S', 2, 166, 'Sterile', 'No growth (Sterile)', 1, '{}', '2dcbbc10-a574-4f01-927d-d5b234400d05');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ff95fb06-0ebf-447c-a6f1-0404c8c40bd1', GETDATE(), 0, '707', 'Other Specimen', 126, 'No growth / Normal flora', 'flora', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '76385608-6ce3-4dbe-872b-9f149e713c7a');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ff95fb06-0ebf-447c-a6f1-0404c8c40bd1', GETDATE(), 0, '707', 'Other Specimen', 2, 126, 'flora', 'No growth / Normal flora', 1, '{}', '51914059-067c-4e79-aee7-64b3a18c35d3');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('b2dac461-4276-46fe-8e2f-40deaf3f7557', GETDATE(), 0, '708', 'ZN Stain For AAFB', 173, 'Negative (No Acid-Fast Bacilli seen)', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '6e0534ea-054a-4766-ae32-9ee8f4457f7b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('b2dac461-4276-46fe-8e2f-40deaf3f7557', GETDATE(), 0, '708', 'ZN Stain For AAFB', 2, 173, NULL, 'Negative (No Acid-Fast Bacilli seen)', 1, '{}', '2c9d7b9a-ed6a-497f-b2bd-1252fe925b80');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('5a2288f3-4a37-451b-9c42-a98dca65cb83', GETDATE(), 0, '709', 'Direct Gram stain', 120, 'No organisms seen', 'seen', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '02b2e4b1-a7e8-452c-96ab-bada6849e86b');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('5a2288f3-4a37-451b-9c42-a98dca65cb83', GETDATE(), 0, '709', 'Direct Gram stain', 2, 120, 'seen', 'No organisms seen', 1, '{}', '779e475b-a45c-4c1f-9393-ca3c765f7bde');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('01400f34-1483-4a79-89a7-5b187eb1bdf8', GETDATE(), 0, '710', 'Wet.preparation', 138, 'Negative (No clues cells', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'c18bed52-99b8-470e-96d8-b03e15457348');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('01400f34-1483-4a79-89a7-5b187eb1bdf8', GETDATE(), 0, '710', 'Wet.preparation', 2, 138, NULL, 'Negative (No clues cells', 1, '{}', 'f5dfbc02-5f2f-4966-a686-6866054830a8');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('7826e68c-3ecb-447a-9c3a-23a6b46eea0c', GETDATE(), 0, '711', 'Skin-Nail-Hair Scraping', 169, 'Negative for fungal elements', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '22cc241e-73c2-4ccf-ae26-1f5a3c67f637');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('7826e68c-3ecb-447a-9c3a-23a6b46eea0c', GETDATE(), 0, '711', 'Skin-Nail-Hair Scraping', 2, 169, NULL, 'Negative for fungal elements', 1, '{}', 'a68e99e1-2cfe-4255-99f9-c34527203f7a');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('ae242940-0f36-4763-a17e-0cee433ff7cf', GETDATE(), 0, '712', 'Throat Swab', 137, 'Normal upper respiratory flora', 'flora', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '77b2df48-2858-4cb0-b102-be433f63780f');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('ae242940-0f36-4763-a17e-0cee433ff7cf', GETDATE(), 0, '712', 'Throat Swab', 2, 137, 'flora', 'Normal upper respiratory flora', 1, '{}', '0d0a10b9-6959-4982-bb1c-a22a3ade770c');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('89c8279b-ca67-45b8-b3c8-29f06ff842ec', GETDATE(), 0, '713', 'Wound Swab', 181, 'No growth', 'growth', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '33d94411-3ec6-4283-a145-013eb44b8934');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('89c8279b-ca67-45b8-b3c8-29f06ff842ec', GETDATE(), 0, '713', 'Wound Swab', 2, 181, 'growth', 'No growth', 1, '{}', 'fb7a1f35-901a-436d-838f-230eb2c2e1d7');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('cf9c6f42-1f10-4a42-abf9-90f1b6d1f2e2', GETDATE(), 0, '714', 'ICT FOR T.B', 168, 'Negative', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', '2b54a8b0-6fa8-435b-a680-3f43520298e5');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('cf9c6f42-1f10-4a42-abf9-90f1b6d1f2e2', GETDATE(), 0, '714', 'ICT FOR T.B', 2, 168, NULL, 'Negative', 1, '{}', '209f611b-f101-42bf-ba19-ae1e40805ea9');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('90904149-ec8c-446a-9150-9b39dcb65f74', GETDATE(), 0, '801', 'Cytopathology', 135, 'Negative for malignancy or atypia', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'cc528515-8bbb-4641-be5e-6c14bba399a9');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('90904149-ec8c-446a-9150-9b39dcb65f74', GETDATE(), 0, '801', 'Cytopathology', 2, 135, NULL, 'Negative for malignancy or atypia', 1, '{}', 'a5d92047-3e39-4bbb-a9eb-46f8007f7e3f');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('e47da1e1-36fc-4b17-ae33-180ae98b5298', GETDATE(), 0, '802', 'Histopathology', 139, 'Benign tissue architecture', 'architecture', 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'a95a8726-e5a4-4a87-a37b-f7a1d83315fc');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('e47da1e1-36fc-4b17-ae33-180ae98b5298', GETDATE(), 0, '802', 'Histopathology', 2, 139, 'architecture', 'Benign tissue architecture', 1, '{}', 'e8855326-bdc0-497e-86f7-673feba15a9e');
INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('d158d980-5303-488a-8b4f-6c659a9aedff', GETDATE(), 0, '803', 'Histopathology (Colon.com mastecto)', 136, 'Negative for malignancy / clean margins', NULL, 'cb6efa6b-6bc0-476d-a483-71bd0bb38d3a', 1, '{}', 'a7107cd2-9a94-49fc-b573-c274937867a7');
INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('d158d980-5303-488a-8b4f-6c659a9aedff', GETDATE(), 0, '803', 'Histopathology (Colon.com mastecto)', 2, 136, NULL, 'Negative for malignancy / clean margins', 1, '{}', 'ba6f8823-9bdd-4155-9344-e4b5b2becaf1');

COMMIT;
SELECT 'Seeding Completed: ' + CAST(@@ROWCOUNT as varchar) + ' records affected.';
