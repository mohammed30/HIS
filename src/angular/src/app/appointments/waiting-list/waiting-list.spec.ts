import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WaitingList } from './waiting-list';

describe('WaitingList', () => {
  let component: WaitingList;
  let fixture: ComponentFixture<WaitingList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WaitingList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WaitingList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
