import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarePlan } from './care-plan';

describe('CarePlan', () => {
  let component: CarePlan;
  let fixture: ComponentFixture<CarePlan>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarePlan]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarePlan);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
