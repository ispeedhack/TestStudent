import { Component } from '@angular/core';
import { TestBed, waitForAsync } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { TestResultService } from 'src/app/services/test.result.service';

describe('TestResultService', () => {
  let testResultService: TestResultService; // Add this

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      providers: [TestResultService, { provide: 'BASE_URL', useValue: 'test' }]
    });

    testResultService = TestBed.get(TestResultService); 
  });

  it('should be created', () => {
    expect(testResultService).toBeTruthy();
  });
});
