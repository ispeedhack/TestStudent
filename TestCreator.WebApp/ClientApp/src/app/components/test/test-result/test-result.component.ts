import { Component, Inject, Input } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../../../services/auth.service';
import { TestResultService } from '../../../services/test.result.service';
import { faRedo } from '@fortawesome/free-solid-svg-icons';
import { TestAttemptResult } from 'src/app/interfaces/test.attempt.result';

@Component({
  selector: "test-result",
  templateUrl: "./test-result.component.html",
  styleUrls: ['./test-result.component.css']
})


export class TestResultComponent {
  result: TestAttemptResult;

  faRedo = faRedo;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    public resultService: TestResultService,
    @Inject('BASE_URL') private baseUrl: string) {

    var id = +this.activatedRoute.snapshot.params["testId"];

    if (id) {
      this.result = resultService.getResult(id);
      if (!this.result) {
        console.log("No results available for test with ID: " + id + " - redirecting to test page...");
        this.router.navigate(["test", id]);
      }
    } else {
      console.log("Unable to parse parameter \"TestId\" - redirecting to home...");
      this.router.navigate(["home"]);
    }

  }
}
