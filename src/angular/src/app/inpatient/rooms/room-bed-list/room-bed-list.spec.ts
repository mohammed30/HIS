import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomBedList } from './room-bed-list';

describe('RoomBedList', () => {
  let component: RoomBedList;
  let fixture: ComponentFixture<RoomBedList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomBedList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoomBedList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
