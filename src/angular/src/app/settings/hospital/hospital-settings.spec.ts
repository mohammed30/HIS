import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HospitalSettings } from './hospital-settings';

describe('HospitalSettings', () => {
  let component: HospitalSettings;
  let fixture: ComponentFixture<HospitalSettings>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HospitalSettings]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HospitalSettings);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
