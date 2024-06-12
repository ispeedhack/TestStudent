import { Component, Inject, Input, OnChanges, SimpleChanges } from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { faPlus, faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { Result } from 'src/app/interfaces/result';
import { Test } from 'src/app/interfaces/test';

@Component({
  selector: "result-list",
  templateUrl: './result-list.component.html',
  styleUrls: ['./result-list.component.css']
})

export class ResultListComponent {
  @Input() test: Test;
  results: Result[];
  title: string;

  faPlus = faPlus;
  faEdit = faEdit;
  faTrash = faTrash;

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {

    this.results = [];
  }

  ngOnChanges(changes: SimpleChanges) {
    if (typeof changes['test'] !== "undefined") {

      //get all changes
      var change = changes['test'];

      //Check whether the new value is the first value assigned.
      if (!change.isFirstChange()) {
        this.loadData();
      }
    }
  }

  loadData() {

    var url = this.baseUrl + "api/result?testId=" + this.test.Id;

    this.http.get<Result[]>(url).subscribe(result => {
        this.results = result;
      },
      error => console.log(error));

  }

  onCreate() {
    this.router.navigate(["/result/create", this.test.Id]);
  }

  onEdit(result: Result) {
    this.router.navigate(["/result/edit", result.Id]);
  }

  onDelete(result: Result) {

    if (confirm("Are you sure to delete selected result ?")) {
      var url = this.baseUrl + "api/result/" + result.Id;

      this.http.delete(url).subscribe(res => {
        console.log("Result " + result.Id + " removed.");

        this.loadData();
      }, error => console.error(error));
    }

  }
}
