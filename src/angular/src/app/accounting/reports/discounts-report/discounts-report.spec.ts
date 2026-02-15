import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscountsReport } from './discounts-report';

describe('DiscountsReport', () => {
  let component: DiscountsReport;
  let fixture: ComponentFixture<DiscountsReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DiscountsReport]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DiscountsReport);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
