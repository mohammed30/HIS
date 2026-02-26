import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SurgicalOperations } from './surgical-operations';

describe('SurgicalOperations', () => {
  let component: SurgicalOperations;
  let fixture: ComponentFixture<SurgicalOperations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SurgicalOperations]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SurgicalOperations);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
