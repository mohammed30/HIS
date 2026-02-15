import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiptVouchers } from './receipt-vouchers';

describe('ReceiptVouchers', () => {
  let component: ReceiptVouchers;
  let fixture: ComponentFixture<ReceiptVouchers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReceiptVouchers]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReceiptVouchers);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
