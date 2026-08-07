$content = Get-Content -Path 'C:\Users\Mohammed\source\repos\HIS\src\angular\src\app\pharmacy\pos\pharmacy-pos.component.ts' -Raw
$content = $content.Replace("          <button class="pos-tab" [class.active]="activeTab==='dispensed-list'" (click)="setTab('dispensed-list')">
            <i class="fas fa-check-double"></i> تم الصرف
          </button>", "          <button class="pos-tab" [class.active]="activeTab==='dispensed-list'" (click)="setTab('dispensed-list')">
            <i class="fas fa-check-double"></i> تم الصرف
          </button>
          <button class="pos-tab" [class.active]="activeTab==='pending-returns'" (click)="setTab('pending-returns')">
            <i class="fas fa-undo"></i> طلبات المرتجعات الداخلية
            <span *ngIf="pendingReturnsCount > 0" class="tab-badge green">{{ pendingReturnsCount }}</span>
          </button>")
$content = $content.Replace("*ngIf="activeTab === 'pending-approval' || activeTab === 'to-dispense' || activeTab === 'refunded-list' || activeTab === 'dispensed-list'"", "*ngIf="activeTab === 'pending-approval' || activeTab === 'to-dispense' || activeTab === 'refunded-list' || activeTab === 'dispensed-list' || activeTab === 'pending-returns'"")
Set-Content -Path 'C:\Users\Mohammed\source\repos\HIS\src\angular\src\app\pharmacy\pos\pharmacy-pos.component.ts' -Value $content -Encoding UTF8
