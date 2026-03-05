import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Penalties } from './penalties';

describe('Penalties', () => {
  let component: Penalties;
  let fixture: ComponentFixture<Penalties>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Penalties]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Penalties);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
