import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabOrders } from './lab-orders';

describe('LabOrders', () => {
  let component: LabOrders;
  let fixture: ComponentFixture<LabOrders>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabOrders]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabOrders);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
