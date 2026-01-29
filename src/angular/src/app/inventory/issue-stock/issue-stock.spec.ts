import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueStock } from './issue-stock';

describe('IssueStock', () => {
  let component: IssueStock;
  let fixture: ComponentFixture<IssueStock>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IssueStock]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IssueStock);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
