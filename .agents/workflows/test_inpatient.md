---
description: How to test inpatient room booking
---

This workflow tests the Inpatient Tab's Rooms and Beds dropdowns.

// turbo-all
1. Build the Angular app to verify no compilation errors.
```powershell
cd c:\Code30\HIS\src\angular
ng build
```

2. Please manually verify in the UI:
- Go to 'ادارة التنويم' (Inpatient Management) tab.
- Choose a Room Type from `نوع الغرفة`.
- Ensure rooms load in `رقم الغرفة`.
- Select a room.
- Ensure only available beds load in `السرير`.
