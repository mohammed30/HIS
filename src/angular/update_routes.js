const fs = require('fs');

let content = fs.readFileSync('src/angular/src/app/route.provider.ts', 'utf8');

// 1. Move all Definitions to Settings
content = content.replace(/parentName:\s*'::Menu:Definitions'/g, "parentName: '::Menu:Settings'");

// Remove the Definitions parent item entirely so it doesn't show up empty
content = content.replace(/\s*{\s*name:\s*'::Menu:Definitions',\s*iconClass:[^}]+},/g, "");

// 2. Move all reports to Reports parent
// For any block that has path containing '/reports' or '/reports/', we change its parentName to '::Menu:Reports'
// Wait, we can just use regex on individual blocks, or do it more precisely.

const lines = content.split('\n');
let insideBlock = false;
let blockStart = -1;
let hasReportPath = false;
let blockLines = [];

for (let i = 0; i < lines.length; i++) {
    if (lines[i].includes('path: ') && lines[i].includes('reports') && !lines[i].includes('user-activity-report') && !lines[i].includes('user-financial-report')) {
        // We will just do a simpler string replace for known reports
    }
}
fs.writeFileSync('src/angular/src/app/route.provider.ts', content, 'utf8');
