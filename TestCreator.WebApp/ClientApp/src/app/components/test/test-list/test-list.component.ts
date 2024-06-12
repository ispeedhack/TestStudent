import { Component, Inject, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Router } from "@angular/router";
import { faRandom, faFire, faSortAlphaDown } from '@fortawesome/free-solid-svg-icons';
import { Test } from 'src/app/interfaces/test';
import * as signalR from '@microsoft/signalr';  

@Component({
  selector: "test-list",
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.css']
}
)

export class TestListComponent implements OnInit {
  @Input() class: string;
  title: string;
  selectedTest: Test;
  tests: Test[];
  url: string;

  faRandom = faRandom;
  faFire = faFire;
  faSortAlphaDown = faSortAlphaDown;

  ngOnInit(): void {

    console.log("TestListComponent create using " + this.class + " class.");

    this.url = this.baseUrl + "api/test?sorting=";

    switch (this.class) {
    case "latest":
    default:
      this.url += "1";
      this.title = "Latest tests";
      break;
    case "random":
      this.url += "0";
      this.title = "Random tests";
      break;
    case "byTitle":
      this.url += "2";
      this.title = "Tests sorted by title";
      break;
    }

    this.getTests();

    const connection = new signalR.HubConnectionBuilder()  
      .configureLogging(signalR.LogLevel.Information)  
      .withUrl(this.baseUrl + 'testsHub')  
      .build(); 

    connection.start().then(function () {  
        console.log('SignalR Connected!');  
      }).catch(function (err) {  
        return console.error(err.toString());  
      });
      
    connection.on("TestCreated", () => {
        if (this.class === "latest"){
          this.getTests();  
        }  
      });
      
    connection.on("TestRemoved", (id: number) => {
        if (this.tests.find(x => x.Id == id) != null){
          this.getTests();  
        }  
      }); 
  }

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {

    this.http = http;
    this.baseUrl = baseUrl;

  }

  getTests(){
    this.http.get<Test[]>(this.url).subscribe(result => {
      this.tests = result;
    },
    error => console.error(error));
  }

  onSelect(test: Test) {
    this.selectedTest = test;
    console.log("Selected test: " + this.selectedTest.Id);
    this.router.navigate(["test", this.selectedTest.Id]);
  }

  getIcon() {
    switch (this.class) {
    case "latest":
    default:
        return faFire;
    case "random":
        return faRandom;
    case "byTitle":
        return faSortAlphaDown;
    }
  }
}
