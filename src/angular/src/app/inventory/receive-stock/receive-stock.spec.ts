import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveStock } from './receive-stock';

describe('ReceiveStock', () => {
  let component: ReceiveStock;
  let fixture: ComponentFixture<ReceiveStock>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReceiveStock]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReceiveStock);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
