import { TestBed } from '@angular/core/testing';

import { Pharmacy } from './pharmacy';

describe('Pharmacy', () => {
  let service: Pharmacy;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Pharmacy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
