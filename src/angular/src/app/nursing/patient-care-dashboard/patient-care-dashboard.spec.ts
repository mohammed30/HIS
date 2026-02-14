import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientCareDashboard } from './patient-care-dashboard';

describe('PatientCareDashboard', () => {
  let component: PatientCareDashboard;
  let fixture: ComponentFixture<PatientCareDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientCareDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientCareDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
