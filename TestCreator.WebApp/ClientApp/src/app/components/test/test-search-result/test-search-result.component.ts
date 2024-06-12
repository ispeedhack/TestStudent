import { Component, Inject, Input } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Router, ActivatedRoute } from "@angular/router";
import { faListUl } from '@fortawesome/free-solid-svg-icons';
import { Test } from 'src/app/interfaces/test';

@Component({
    selector: "test-search-result",
    templateUrl: './test-search-result.component.html',
    styleUrls: ['./test-search-result.component.css']
  }
)

export class TestSearchResultComponent {
  title: string;
  selectedTest: Test;
  tests: Test[];

  faListUl = faListUl;

  constructor(private http: HttpClient,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    @Inject('BASE_URL') private baseUrl: string) {

    this.http = http;
    this.baseUrl = baseUrl;

    this.title = "Search results: ";

    var searchedPhrase = this.activatedRoute.snapshot.params["text"];

    if (searchedPhrase) {
      this.title += searchedPhrase;
      var url = this.baseUrl + "api/test";
      let params = new HttpParams().set("keyword", searchedPhrase);

      this.http.get<Test[]>(url, { params: params }).subscribe(result => {
          this.tests = result;
        },
        error => console.error(error));
    } 

  }

  onSelect(test: Test) {
    this.selectedTest = test;
    console.log("Selected test: " + this.selectedTest.Id);
    this.router.navigate(["test", this.selectedTest.Id]);
  }

}
