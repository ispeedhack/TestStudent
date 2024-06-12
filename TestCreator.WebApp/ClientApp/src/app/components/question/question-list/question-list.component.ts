import { Component, Inject, Input, OnChanges, SimpleChanges } from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { faPlus, faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { Question } from 'src/app/interfaces/question';
import { Test } from 'src/app/interfaces/test';

@Component({
  selector: "question-list",
  templateUrl: './question-list.component.html',
  styleUrls: ['./question-list.component.css']
})

export class QuestionListComponent {
  @Input() test: Test;
  questions: Question[];
  title: string;

  faPlus = faPlus;
  faEdit = faEdit;
  faTrash = faTrash;

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {

    this.questions = [];
  }

  ngOnChanges(changes: SimpleChanges) {
    if (typeof changes['test'] !== "undefined") {

      //get all chenges
      var change = changes['test'];

      //Check whether the new value is the first value assigned.
      if (!change.isFirstChange()) {
        this.loadData();
      }
    }
  }

  loadData() {

    var url = this.baseUrl + "api/question?testId=" + this.test.Id;

    this.http.get<Question[]>(url).subscribe(result => {
        this.questions = result;
      },
      error => console.log(error));

  }

  onCreate() {
    this.router.navigate(["/question/create", this.test.Id]);
  }

  onEdit(question: Question) {
    this.router.navigate(["/question/edit", question.Id]);
  }

  onDelete(question: Question) {

    if (confirm("Are you sure to delete selected question ?")) {
      var url = this.baseUrl + "api/question/" + question.Id;

      this.http.delete(url).subscribe(result => {
        console.log("Question " + question.Id + " removed.");

        this.loadData();
      }, error => console.error(error));
    }

  }
}
