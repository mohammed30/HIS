import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-credit-limit-gauge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="gauge-container" [title]="tooltipText">
      <svg viewBox="0 0 100 50" class="gauge-svg">
        <!-- Background Track -->
        <path d="M 10 50 A 40 40 0 0 1 90 50" 
              fill="none" 
              stroke="#e0e0e0" 
              stroke-width="12" 
              stroke-linecap="round" />
        
        <!-- Active Value Track -->
        <path d="M 10 50 A 40 40 0 0 1 90 50" 
              fill="none" 
              [attr.stroke]="gaugeColor" 
              stroke-width="12" 
              stroke-linecap="round" 
              [style.stroke-dasharray]="dashArray" 
              [style.stroke-dashoffset]="dashOffset" />
              
        <!-- Needle -->
        <g [style.transform]="needleTransform" style="transform-origin: 50px 50px;">
           <circle cx="50" cy="50" r="4" fill="#333" />
           <polygon points="48,50 52,50 50,15" fill="#333" />
        </g>
      </svg>
      <div class="gauge-text" [style.color]="gaugeColor">
        {{ percentage | number:'1.0-0' }}%
      </div>
      <div class="gauge-subtext text-muted small" *ngIf="showDetails">
        {{ utilizedAmount | number:'1.0-0' }} / {{ creditLimit | number:'1.0-0' }}
      </div>
    </div>
  `,
  styles: [`
    .gauge-container {
      width: 120px;
      text-align: center;
      position: relative;
      display: inline-block;
    }
    .gauge-svg {
      width: 100%;
      height: auto;
      overflow: visible;
    }
    .gauge-text {
      font-size: 14px;
      font-weight: bold;
      margin-top: -10px;
    }
    .gauge-subtext {
      font-size: 10px;
    }
    path {
      transition: stroke-dashoffset 0.5s ease-in-out, stroke 0.3s;
    }
    g {
      transition: transform 0.5s ease-in-out;
    }
  `]
})
export class CreditLimitGaugeComponent implements OnChanges {
  @Input() creditLimit: number = 1000;
  @Input() utilizedAmount: number = 0;
  @Input() showDetails: boolean = true;

  percentage: number = 0;
  dashArray: number = 125.6; // pi * 40 (radius)
  dashOffset: number = 125.6;
  gaugeColor: string = '#28a745'; // success green
  needleTransform: string = 'rotate(-90deg)';
  tooltipText: string = '';

  ngOnChanges() {
    this.updateGauge();
  }

  updateGauge() {
    if (!this.creditLimit || this.creditLimit <= 0) {
      this.percentage = 0;
    } else {
      this.percentage = (this.utilizedAmount / this.creditLimit) * 100;
    }

    // Cap at 100% for visual purposes
    const displayPercentage = Math.min(Math.max(this.percentage, 0), 100);
    
    // Calculate SVG dash offset
    this.dashOffset = this.dashArray - (this.dashArray * displayPercentage / 100);

    // Calculate needle rotation (-90deg to +90deg)
    const degrees = -90 + (180 * displayPercentage / 100);
    this.needleTransform = `rotate(${degrees}deg)`;

    // Set color based on percentage
    if (this.percentage < 50) {
      this.gaugeColor = '#28a745'; // Green
    } else if (this.percentage < 85) {
      this.gaugeColor = '#ffc107'; // Yellow
    } else {
      this.gaugeColor = '#dc3545'; // Red
    }

    this.tooltipText = `الحد الائتماني: ${this.creditLimit}\nالمستخدم: ${this.utilizedAmount}\nالنسبة: ${this.percentage.toFixed(1)}%`;
  }
}
