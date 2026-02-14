import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShiftHandover } from './shift-handover';

describe('ShiftHandover', () => {
  let component: ShiftHandover;
  let fixture: ComponentFixture<ShiftHandover>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShiftHandover]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShiftHandover);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
