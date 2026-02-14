import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FluidBalance } from './fluid-balance';

describe('FluidBalance', () => {
  let component: FluidBalance;
  let fixture: ComponentFixture<FluidBalance>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FluidBalance]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FluidBalance);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
