import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompensationItems } from './compensation-items';

describe('CompensationItems', () => {
  let component: CompensationItems;
  let fixture: ComponentFixture<CompensationItems>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompensationItems]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CompensationItems);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
