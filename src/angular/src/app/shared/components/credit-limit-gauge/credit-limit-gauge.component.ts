import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-credit-limit-gauge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="gauge-container" [title]="tooltipText">
      <svg viewBox="0 0 100 60" class="gauge-svg">
        <!-- Segment 1: Red (0-20%) -->
        <path d="M 10 50 A 40 40 0 0 1 90 50" 
              fill="none" 
              stroke="#dc3545" 
              stroke-width="12" 
              stroke-linecap="butt"
              stroke-dasharray="25.13 1000"
              stroke-dashoffset="0" />
              
        <!-- Segment 2: Yellow (20-50%) -->
        <path d="M 10 50 A 40 40 0 0 1 90 50" 
              fill="none" 
              stroke="#ffc107" 
              stroke-width="12" 
              stroke-linecap="butt"
              stroke-dasharray="37.7 1000"
              stroke-dashoffset="-25.13" />

        <!-- Segment 3: Green (50-100%) -->
        <path d="M 10 50 A 40 40 0 0 1 90 50" 
              fill="none" 
              stroke="#28a745" 
              stroke-width="12" 
              stroke-linecap="butt"
              stroke-dasharray="62.83 1000"
              stroke-dashoffset="-62.83" />

        <!-- Needle -->
        <g [style.transform]="needleTransform" style="transform-origin: 50px 50px;" class="gauge-needle">
           <circle cx="50" cy="50" r="4" />
           <polygon points="48,50 52,50 50,15" />
        </g>
      </svg>
      <div class="gauge-text">
        {{ percentage | number:'1.0-0' }}%
      </div>
      <div class="gauge-subtext text-muted small" *ngIf="showDetails" dir="ltr">
        {{ utilizedAmount | number:'1.0-0' }} / {{ creditLimit | number:'1.0-0' }}
      </div>
    </div>
  `,
  styles: [`
    .gauge-container {
      width: 140px;
      text-align: center;
      position: relative;
      display: inline-block;
    }
    .gauge-svg {
      width: 100%;
      height: auto;
      overflow: visible;
    }
    .gauge-needle circle, .gauge-needle polygon {
      fill: var(--theme-text-primary, #212529);
      transition: fill 0.3s ease;
    }
    .gauge-text {
      font-size: 18px;
      font-weight: bold;
      margin-top: -5px;
      color: var(--theme-text-primary, #212529);
    }
    .gauge-subtext {
      font-size: 11px;
    }
    g {
      transition: transform 0.8s cubic-bezier(0.4, 0, 0.2, 1);
    }
  `]
})
export class CreditLimitGaugeComponent implements OnChanges {
  @Input() creditLimit: number = 1000;
  @Input() utilizedAmount: number = 0;
  @Input() showDetails: boolean = true;

  percentage: number = 0;
  needleTransform: string = 'rotate(-90deg)';
  tooltipText: string = '';

  ngOnChanges() {
    this.updateGauge();
  }

  updateGauge() {
    if (!this.creditLimit || this.creditLimit <= 0) {
      this.percentage = 0;
    } else {
      this.percentage = ((this.creditLimit - this.utilizedAmount) / this.creditLimit) * 100;
    }

    const displayPercentage = Math.min(Math.max(this.percentage, 0), 100);
    const degrees = -90 + (180 * displayPercentage / 100);
    this.needleTransform = `rotate(${degrees}deg)`;
    
    this.tooltipText = `الحد الائتماني: ${this.creditLimit}\nالمستخدم: ${this.utilizedAmount}\nالرصيد المتاح: ${this.percentage.toFixed(1)}%`;
  }
}
