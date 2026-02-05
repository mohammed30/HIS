import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DispensingWorkflow } from './dispensing-workflow';

describe('DispensingWorkflow', () => {
  let component: DispensingWorkflow;
  let fixture: ComponentFixture<DispensingWorkflow>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DispensingWorkflow]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DispensingWorkflow);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
