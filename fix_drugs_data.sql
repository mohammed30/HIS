-- Clean up corrupted data
DELETE FROM AppDrugs;

-- Re-insert data correctly using AppServiceItems to link IDs
INSERT INTO AppDrugs (Id, Barcode, BrandName, ScientificName, Strength, Form, Manufacturer, BatchNumberPrefix, MinimumStockLevel, ReorderLevel, IsControlled, BinLocation, LegalCategory, ServiceItemId, ExtraProperties, ConcurrencyStamp, CreationTime, IsDeleted)
SELECT 
    NEWID(), 
    d.Barcode, d.BrandName, d.ScientificName, d.Strength, d.Form, d.Manufacturer, d.BatchNumberPrefix, 
    d.MinStock, d.Reorder, d.IsControlled, d.Bin, d.Legal,
    s.Id as ServiceItemId,
    '{}', NEWID(), GETDATE(), 0
FROM (
    VALUES 
    ('6280000001', 'Panadol Advance', 'Paracetamol', '500mg', 'Tablet', 'GSK', 'PAN', 100, 20, 0, 'A1-01', 'GSL'),
    ('6280000002', 'Amoxicillin', 'Amoxicillin', '500mg', 'Capsule', 'Hikma', 'AMX', 50, 10, 0, 'B2-04', 'POM'),
    ('6280000003', 'Ventolin', 'Salbutamol', '100mcg', 'Inhaler', 'GSK', 'VEN', 30, 5, 0, 'C3-12', 'POM'),
    ('6280000004', 'Lipitor', 'Atorvastatin', '20mg', 'Tablet', 'Pfizer', 'LIP', 40, 10, 0, 'D1-05', 'POM'),
    ('6280000005', 'Nexium', 'Esomeprazole', '40mg', 'Tablet', 'AstraZeneca', 'NEX', 60, 15, 0, 'E2-08', 'POM'),
    ('6280000006', 'Augmentin', 'Co-amoxiclav', '1g', 'Tablet', 'GSK', 'AUG', 25, 5, 0, 'B1-02', 'POM'),
    ('6280000007', 'Voltaren', 'Diclofenac', '1%', 'Gel', 'Novartis', 'VOL', 45, 10, 0, 'F1-03', 'P'),
    ('6280000008', 'Brufen', 'Ibuprofen', '400mg', 'Tablet', 'Abbott', 'BRU', 80, 20, 0, 'A2-02', 'P'),
    ('6280000009', 'Zyrtec', 'Cetirizine', '10mg', 'Tablet', 'UCB', 'ZYR', 55, 12, 0, 'G1-06', 'GSL'),
    ('6280000010', 'Glucophage', 'Metformin', '500mg', 'Tablet', 'Merck', 'GLU', 90, 25, 0, 'H1-09', 'POM')
) AS d(Barcode, BrandName, ScientificName, Strength, Form, Manufacturer, BatchNumberPrefix, MinStock, Reorder, IsControlled, Bin, Legal)
JOIN AppServiceItems s ON s.Code = d.Barcode AND s.Category = 6;
