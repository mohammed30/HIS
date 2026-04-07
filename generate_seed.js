const fs = require('fs');
const path = require('path');

const csvPath = path.join(__dirname, 'tests.csv');
const sqlPath = path.join(__dirname, 'seed_lab_data.sql');

const content = fs.readFileSync(csvPath, 'utf8');
const lines = content.split('\n');

let sql = `
-- Seed Lab Test Categories and Tests
BEGIN TRANSACTION;

-- Clean existing data if needed (Optional)
-- DELETE FROM [AppLabTests];
-- DELETE FROM [AppLabTestCategories];

`;

let mainCategoryId = null;
let subCategoryId = null;

const categories = [];
const tests = [];

function generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

const priceRanges = {
    'HAEMATOLOGY': [50, 150],
    'ENDOCRINOLOGY': [200, 450],
    'IMMUNOLOGY': [250, 500],
    'CHEMISTRY': [100, 300],
    'MISCELLANEOUS': [150, 400],
    'MICRO': [80, 200],
    'CULTURE': [300, 700],
    'HISTO': [600, 2000]
};

let currentMainCatName = '';

for (let line of lines) {
    const parts = line.split(',').map(p => p.trim().replace(/^"|"$/g, ''));
    if (parts.length < 2) continue;

    const col1 = parts[0];
    const col2 = parts[1];
    const col3 = parts[2] || '';

    // Main Category
    if (col1.match(/^\d$/)) {
        mainCategoryId = generateGuid();
        subCategoryId = null;
        currentMainCatName = col2.split(' ')[0].toUpperCase();
        categories.push({ id: mainCategoryId, code: col1, name: col2, parentId: null, order: categories.length + 1 });
    } 
    // Sub Category
    else if (col1 === '' && col2 && col2 === col2.toUpperCase() && isNaN(col2)) {
        subCategoryId = generateGuid();
        categories.push({ id: subCategoryId, code: col2.substring(0, 5), name: col2, parentId: mainCategoryId, order: categories.length + 1 });
    }
    // Test
    else if (col1.match(/^\d{3}$/)) {
        const catId = subCategoryId || mainCategoryId;
        const range = col3;
        
        // Indicative price
        const r = priceRanges[currentMainCatName] || [100, 200];
        const price = Math.floor(Math.random() * (r[1] - r[0] + 1) + r[0]);

        // Simple unit extraction
        let unit = null;
        const unitMatch = range.match(/[a-zA-Z\/%^]{1,}/g);
        if (unitMatch && !range.includes('Negative') && !range.includes('Varies')) {
             unit = unitMatch[unitMatch.length - 1];
             if (unit.length < 2) unit = null;
        }

        tests.push({
            id: generateGuid(),
            code: col1,
            name: col2,
            price: price,
            range: range,
            unit: unit,
            categoryId: catId
        });
    }
}

// Generate SQL Inserts
categories.forEach(c => {
    sql += `INSERT INTO [AppLabTestCategories] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [ParentId], [SortOrder], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('${c.id}', GETDATE(), 0, '${c.code}', '${c.name.replace(/'/g, "''")}', ${c.parentId ? `'${c.parentId}'` : 'NULL'}, ${c.order}, 1, '{}', '${generateGuid()}');\n`;
});

tests.forEach(t => {
    sql += `INSERT INTO [AppLabTests] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Price], [ReferenceRange], [Unit], [CategoryId], [IsActive], [ExtraProperties], [ConcurrencyStamp]) 
    VALUES ('${t.id}', GETDATE(), 0, '${t.code}', '${t.name.replace(/'/g, "''")}', ${t.price}, '${t.range.replace(/'/g, "''")}', ${t.unit ? `'${t.unit}'` : 'NULL'}, '${t.categoryId}', 1, '{}', '${generateGuid()}');\n`;

    // Also Insert into AppServiceItems so it's available for billing/reception
    sql += `INSERT INTO [AppServiceItems] ([Id], [CreationTime], [IsDeleted], [Code], [Name], [Category], [Price], [Unit], [ReferenceRange], [IsActive], [Discriminator], [ExtraProperties], [ConcurrencyStamp])
    VALUES ('${t.id}', GETDATE(), 0, '${t.code}', '${t.name.replace(/'/g, "''")}', 2, ${t.price}, ${t.unit ? `'${t.unit}'` : 'NULL'}, '${t.range.replace(/'/g, "''")}', 1, 'ServiceItem', '{}', '${generateGuid()}');\n`;
});

sql += `
COMMIT;
SELECT 'Seeding Completed: ' + CAST(@@ROWCOUNT as varchar) + ' records affected.';
`;

fs.writeFileSync(sqlPath, sql);
console.log(`Generated SQL with ${categories.length} categories and ${tests.length} tests.`);
