import { Component, Inject, Input, OnChanges, SimpleChanges } from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { faPlus, faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { Answer } from 'src/app/interfaces/answer';
import { Question } from 'src/app/interfaces/question';

@Component({
  selector: "answer-list",
  templateUrl: './answer-list.component.html',
  styleUrls: ['./answer-list.component.css']
})

export class AnswerListComponent {
  @Input() question: Question;
  answers: Answer[];
  title: string;

  faPlus = faPlus;
  faEdit = faEdit;
  faTrash = faTrash;

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {

    this.answers = [];
  }

  ngOnChanges(changes: SimpleChanges) {
    if (typeof changes['question'] !== "undefined") {

      //get all chenges
      var change = changes['question'];

      //Check whether the new value is the first value assigned.
      if (!change.isFirstChange()) {
        this.loadData();
      }
    }
  }

  loadData() {

    var url = this.baseUrl + "api/answer?questionId=" + this.question.Id;

    this.http.get<Answer[]>(url).subscribe(result => {
      this.answers = result;
      },
      error => console.log(error));

  }

  onCreate() {
    this.router.navigate(["/answer/create", this.question.Id]);
  }

  onEdit(answer: Answer) {
    this.router.navigate(["/answer/edit", answer.Id]);
  }

  onDelete(answer: Answer) {

    if (confirm("Are you sure to delete selected answer ?")) {
      var url = this.baseUrl + "api/answer/" + answer.Id;

      this.http.delete(url).subscribe(result => {
        console.log("Answer " + answer.Id + " removed.");

        this.loadData();
      }, error => console.error(error));
    }

  }
}
