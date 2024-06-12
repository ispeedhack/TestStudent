import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { TestAttemptResult } from 'src/app/interfaces/test.attempt.result';

@Injectable()
export class TestResultService {

  private result: TestAttemptResult;

  constructor(private router: Router,
    @Inject('BASE_URL') private baseUrl: string) { }

  putResult(result: TestAttemptResult) {
    this.result = result;
  }

  getResult(testId: number) {
    if (testId) {
      if (this.result.TestId === testId) {
        return this.result;
      } else {
        console.log("No result found for Tests with ID: " + testId + ". Redirecting to test page...");
        this.router.navigate(["test", testId]);
      }
      
    } else {
      console.log("Invalid Test ID. Redirecting to home...");
      this.router.navigate(["home"]);
    }
  }

}
