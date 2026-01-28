import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceLists } from './price-lists';

describe('PriceLists', () => {
  let component: PriceLists;
  let fixture: ComponentFixture<PriceLists>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceLists]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriceLists);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
