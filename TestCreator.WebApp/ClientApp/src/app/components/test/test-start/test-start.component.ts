import { Component, Inject } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpParams } from "@angular/common/http";
import { AuthService } from '../../../services/auth.service';
import { TestResultService } from '../../../services/test.result.service';
import { faArrowRight, faArrowLeft, faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { TestAttempt } from 'src/app/interfaces/test.attempt';
import { TestAttemptEntry } from 'src/app/interfaces/test.attempt.entry';
import { TestAttemptResult } from 'src/app/interfaces/test.attempt.result';

@Component({
  selector: "test-start",
  templateUrl: "./test-start.component.html",
  styleUrls: ['./test-start.component.css']
})

export class TestStartComponent {
  testAttempt: TestAttempt; 
  title: string;
  radioSelectedId: number;
  selectedTestAttemptEntry: TestAttemptEntry;
  selectedTestAttemptEntryIndex: number = 0;

  faArrowRight = faArrowRight;
  faArrowLeft = faArrowLeft;
  faCheckCircle = faCheckCircle;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    public testResult: TestResultService,
    @Inject('BASE_URL') private baseUrl: string) {

    var id = +this.activatedRoute.snapshot.params["id"];
    console.log(id);

    if (id) {
      var url = this.baseUrl + "api/testAttempt/" + id;
      this.http.get<TestAttempt>(url).subscribe(result => {
        this.testAttempt = result;
        this.title = this.testAttempt.Title;

        if (this.testAttempt.TestAttemptEntries.length > 0) {
          this.selectedTestAttemptEntry = this.testAttempt.TestAttemptEntries[this.selectedTestAttemptEntryIndex];
        }
          
        },
        error => console.error(error));
    } else {
      console.log("Invalid ID - redirecting to home...");
      this.router.navigate(["home"]);
    }
  }

  onNext() {
    if (this.testAttempt.TestAttemptEntries.length > this.selectedTestAttemptEntryIndex + 1) {
      this.selectedTestAttemptEntryIndex += 1;
      this.selectedTestAttemptEntry = this.testAttempt.TestAttemptEntries[this.selectedTestAttemptEntryIndex];
    }
  }

  onPrevious() {
    if (this.selectedTestAttemptEntryIndex > 0) {
      this.selectedTestAttemptEntryIndex -= 1;
      this.selectedTestAttemptEntry = this.testAttempt.TestAttemptEntries[this.selectedTestAttemptEntryIndex];
    }
  }

  onFinish() {
    var url = this.baseUrl + "api/testAttempt";

    if (this.testAttempt) {

      this.http.post<TestAttemptResult>(url, this.testAttempt).subscribe(result => {
        if (result) {
            this.testResult.putResult(result);
            this.router.navigate(["test/result", result.TestId]);
          } else {
            console.log("Error during calculating results - redirecting to home...");
            this.router.navigate(["home"]);
          }
        },
        error => console.error(error));
    }
  }

  handleChange(evt) {
    var target = evt.target;
    if (target.checked) {
      for (let i = 0; i < this.selectedTestAttemptEntry.Answers.length; i++) {
        if (this.selectedTestAttemptEntry.Answers[i].Id !== this.radioSelectedId) {
          this.selectedTestAttemptEntry.Answers[i].Checked = false;
        } else {
          this.selectedTestAttemptEntry.Answers[i].Checked = true;
        }
      } 
    } 
  }
}
