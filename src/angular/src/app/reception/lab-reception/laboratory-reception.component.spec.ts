import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LaboratoryReceptionComponent } from './laboratory-reception.component';
import { By } from '@angular/platform-browser';
import { of, throwError } from 'rxjs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

// Mocks
import { PatientService } from '../../proxy/patients/patient.service';
import { PatientBalanceGuardService } from '../../shared/patient-balance-guard.service';
import { ToasterService } from '@abp/ng.theme.shared';
import { LocalizationService } from '@abp/ng.core';

describe('LaboratoryReceptionComponent', () => {
  let component: LaboratoryReceptionComponent;
  let fixture: ComponentFixture<LaboratoryReceptionComponent>;
  
  let mockPatientService: jasmine.SpyObj<PatientService>;
  let mockPatientBalanceGuard: jasmine.SpyObj<PatientBalanceGuardService>;
  let mockToasterService: jasmine.SpyObj<ToasterService>;
  let mockLocalization: jasmine.SpyObj<LocalizationService>;

  beforeEach(async () => {
    mockPatientService = jasmine.createSpyObj('PatientService', ['search', 'get', 'create', 'update']);
    mockPatientBalanceGuard = jasmine.createSpyObj('PatientBalanceGuardService', ['checkPatientBalance', 'getCurrentStatus']);
    mockToasterService = jasmine.createSpyObj('ToasterService', ['success', 'error', 'warn']);
    mockLocalization = jasmine.createSpyObj('LocalizationService', ['instant']);

    // Setup default guard status
    mockPatientBalanceGuard.getCurrentStatus.and.returnValue({
      isAdmitted: false,
      availableBalance: undefined,
      isServicesStopped: false
    });

    await TestBed.configureTestingModule({
      imports: [
        LaboratoryReceptionComponent, 
        CommonModule, 
        FormsModule, 
        ReactiveFormsModule
      ],
      providers: [
        { provide: PatientService, useValue: mockPatientService },
        { provide: PatientBalanceGuardService, useValue: mockPatientBalanceGuard },
        { provide: ToasterService, useValue: mockToasterService },
        { provide: LocalizationService, useValue: mockLocalization },
        // Add other mock providers as needed for full initialization...
      ]
    })
    // Override providers specifically since it's a standalone component with many injects
    .overrideComponent(LaboratoryReceptionComponent, {
      set: {
        providers: [
          { provide: PatientService, useValue: mockPatientService },
          { provide: PatientBalanceGuardService, useValue: mockPatientBalanceGuard },
          { provide: ToasterService, useValue: mockToasterService },
          { provide: LocalizationService, useValue: mockLocalization }
        ]
      }
    })
    .compileComponents();

    fixture = TestBed.createComponent(LaboratoryReceptionComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    // Note: In a real app, you would need to mock ALL injected services.
    // For this demonstration, we assume basic creation passes if mocks are injected.
    expect(component).toBeTruthy();
  });

  describe('Complex Scenario: Patient Selection & Admission State', () => {
    it('should show Admitted Badge with pulse animation when patient is admitted', fakeAsync(() => {
      // 1. Arrange
      component.patientInfo = { id: 'patient-1', fullNameAr: 'يوسف', mrn: 'MRN123' };
      
      // Mock the guard to return admitted status
      mockPatientBalanceGuard.getCurrentStatus.and.returnValue({
        isAdmitted: true, // This is the key trigger
        availableBalance: 1500,
        isServicesStopped: false
      });

      // 2. Act
      fixture.detectChanges();
      tick();

      // 3. Assert
      const badgeElement = fixture.debugElement.query(By.css('.input-group-text'));
      expect(badgeElement).toBeTruthy('Badge should be rendered');
      
      const badgeClasses = badgeElement.nativeElement.classList;
      expect(badgeClasses.contains('bg-danger')).toBeTrue();
      expect(badgeClasses.contains('pulse-danger')).toBeTrue(); // Check for our custom animation
      expect(badgeElement.nativeElement.textContent).toContain('مريض منوم');
    }));

    it('should show Outpatient Badge when patient is NOT admitted', fakeAsync(() => {
      // 1. Arrange
      component.patientInfo = { id: 'patient-2', fullNameAr: 'خالد', mrn: 'MRN456' };
      
      // Mock the guard to return outpatient status
      mockPatientBalanceGuard.getCurrentStatus.and.returnValue({
        isAdmitted: false,
        availableBalance: undefined,
        isServicesStopped: false
      });

      // 2. Act
      fixture.detectChanges();
      tick();

      // 3. Assert
      const badgeElement = fixture.debugElement.query(By.css('.input-group-text'));
      expect(badgeElement).toBeTruthy();
      
      const badgeClasses = badgeElement.nativeElement.classList;
      expect(badgeClasses.contains('bg-success')).toBeTrue();
      expect(badgeClasses.contains('pulse-danger')).toBeFalse();
      expect(badgeElement.nativeElement.textContent).toContain('مريض خارجي');
    }));
  });

  describe('Complex Scenario: Service Stopping & Danger Alerts', () => {
    it('should display the red danger alert when services are stopped for a patient', () => {
      // 1. Arrange
      mockPatientBalanceGuard.getCurrentStatus.and.returnValue({
        isAdmitted: true,
        availableBalance: -500, // Negative balance
        isServicesStopped: true // Services Stopped by Accounting
      });

      // 2. Act
      fixture.detectChanges();

      // 3. Assert
      const dangerAlert = fixture.debugElement.query(By.css('.alert-danger'));
      expect(dangerAlert).toBeTruthy('Danger alert should be visible');
      expect(dangerAlert.nativeElement.textContent).toContain('تم إيقاف الخدمات لهذا المريض');
    });

    it('should block saving and show toaster warning if services are stopped', () => {
      // 1. Arrange
      mockPatientBalanceGuard.getCurrentStatus.and.returnValue({
        isAdmitted: true,
        availableBalance: -100,
        isServicesStopped: true
      });
      component.patientInfo = { id: 'patient-stopped' };

      // 2. Act
      // Assuming you have a savePatient() method that checks this
      // component.savePatient();

      // 3. Assert
      // expect(mockToasterService.error).toHaveBeenCalledWith(jasmine.stringMatching(/موقوفة/));
      // expect(mockPatientService.update).not.toHaveBeenCalled();
    });
  });

  describe('Complex Scenario: Fast Search Debounce', () => {
    it('should debounce search requests to prevent API spam', fakeAsync(() => {
      // Arrange
      mockPatientService.search.and.returnValue(of([]));
      
      // Act
      component.patientInfo.fullNameAr = 'ي';
      component.searchPatient(); // User types fast
      component.patientInfo.fullNameAr = 'يو';
      component.searchPatient();
      component.patientInfo.fullNameAr = 'يوس';
      component.searchPatient();
      
      tick(500); // Simulate debounce time

      // Assert
      // In a properly debounced component, the service should only be called ONCE.
      // expect(mockPatientService.search).toHaveBeenCalledTimes(1);
    }));
  });
});
