import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentVouchers } from './payment-vouchers';

describe('PaymentVouchers', () => {
  let component: PaymentVouchers;
  let fixture: ComponentFixture<PaymentVouchers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentVouchers]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentVouchers);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
