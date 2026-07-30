import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientDemographics } from './patient-demographics';

describe('PatientDemographics', () => {
  let component: PatientDemographics;
  let fixture: ComponentFixture<PatientDemographics>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientDemographics]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientDemographics);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
