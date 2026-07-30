import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BillingSummary } from './billing-summary';

describe('BillingSummary', () => {
  let component: BillingSummary;
  let fixture: ComponentFixture<BillingSummary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BillingSummary]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BillingSummary);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
