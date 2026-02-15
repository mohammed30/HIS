import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractClaims } from './contract-claims';

describe('ContractClaims', () => {
  let component: ContractClaims;
  let fixture: ComponentFixture<ContractClaims>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContractClaims]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ContractClaims);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
