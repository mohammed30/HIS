import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyAccountsReport } from './daily-accounts-report';

describe('DailyAccountsReport', () => {
  let component: DailyAccountsReport;
  let fixture: ComponentFixture<DailyAccountsReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DailyAccountsReport]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DailyAccountsReport);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
