import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreditLimitGaugeComponent } from './credit-limit-gauge.component';
import { By } from '@angular/platform-browser';

describe('CreditLimitGaugeComponent', () => {
  let component: CreditLimitGaugeComponent;
  let fixture: ComponentFixture<CreditLimitGaugeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreditLimitGaugeComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(CreditLimitGaugeComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Percentage Calculation Scenarios', () => {
    it('should calculate exactly 50% when utilized is half of limit', () => {
      component.creditLimit = 1000;
      component.utilizedAmount = 500;
      component.updateGauge();
      expect(component.percentage).toBe(50);
    });

    it('should calculate 0% and handle divide by zero when creditLimit is 0', () => {
      component.creditLimit = 0;
      component.utilizedAmount = 500;
      component.updateGauge();
      expect(component.percentage).toBe(0);
    });

    it('should calculate 0% when creditLimit is negative', () => {
      component.creditLimit = -500;
      component.utilizedAmount = 100;
      component.updateGauge();
      expect(component.percentage).toBe(0);
    });

    // Complex Scenario 1: Over-utilization (Negative Available Balance)
    it('should calculate negative percentage but cap needle at -90deg', () => {
      component.creditLimit = 1000;
      component.utilizedAmount = 1500; // Over limit by 500
      component.updateGauge();
      
      expect(component.percentage).toBe(-50); // Real calculation
      expect(component.needleTransform).toBe('rotate(-90deg)'); // Visual cap at 0%
    });

    // Complex Scenario 2: Refunded more than used (Over 100%)
    it('should calculate >100% percentage but cap needle at 90deg', () => {
      component.creditLimit = 1000;
      component.utilizedAmount = -500; // Customer paid in advance / refund
      component.updateGauge();
      
      expect(component.percentage).toBe(150); // Real calculation
      expect(component.needleTransform).toBe('rotate(90deg)'); // Visual cap at 100%
    });
  });

  describe('Needle Rotation Angles', () => {
    it('should rotate needle to -90deg when balance is empty (0%)', () => {
      component.creditLimit = 1000;
      component.utilizedAmount = 1000;
      component.updateGauge();
      expect(component.needleTransform).toBe('rotate(-90deg)');
    });

    it('should rotate needle to 0deg when balance is exactly half (50%)', () => {
      component.creditLimit = 1000;
      component.utilizedAmount = 500;
      component.updateGauge();
      expect(component.needleTransform).toBe('rotate(0deg)');
    });

    it('should rotate needle to 90deg when balance is full (100%)', () => {
      component.creditLimit = 1000;
      component.utilizedAmount = 0;
      component.updateGauge();
      expect(component.needleTransform).toBe('rotate(90deg)');
    });
  });

  describe('DOM and UI Binding', () => {
    it('should update tooltip text correctly after calculation', () => {
      component.creditLimit = 2000;
      component.utilizedAmount = 500;
      component.updateGauge();
      fixture.detectChanges();

      const container = fixture.debugElement.query(By.css('.gauge-container'));
      expect(container.nativeElement.getAttribute('title')).toContain('الحد الائتماني: 2000');
      expect(container.nativeElement.getAttribute('title')).toContain('المستخدم: 500');
      expect(container.nativeElement.getAttribute('title')).toContain('الرصيد المتاح: 75.0%');
    });

    it('should not render subtext if showDetails is false', () => {
      component.showDetails = false;
      fixture.detectChanges();
      
      const subtext = fixture.debugElement.query(By.css('.gauge-subtext'));
      expect(subtext).toBeNull();
    });
  });
});
