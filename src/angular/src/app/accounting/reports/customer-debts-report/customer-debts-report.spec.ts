import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerDebtsReport } from './customer-debts-report';

describe('CustomerDebtsReport', () => {
  let component: CustomerDebtsReport;
  let fixture: ComponentFixture<CustomerDebtsReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerDebtsReport]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerDebtsReport);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
