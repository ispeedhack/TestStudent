import { Component, Inject } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { AuthService } from '../../services/auth.service';
import { faAlignJustify, faChartBar, faShare, faPlay, faHome } from '@fortawesome/free-solid-svg-icons';
import { Test } from 'src/app/interfaces/test';

@Component({
  selector: "test",
  templateUrl: "./test.component.html",
  styleUrls: ['./test.component.css']
})

export class TestComponent {
  test: Test;
  faAlignJustify = faAlignJustify;
  faChartBar = faChartBar;
  faShare = faShare;
  faPlay = faPlay;
  faHome = faHome;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    public auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string) {

    this.test = <Test>{};

    var id = +this.activatedRoute.snapshot.params["id"];
    console.log(id);

    if (id) {
      var url = this.baseUrl + "api/test/" + id;
      this.http.get<Test>(url).subscribe(result => {
          this.test = result;

          var patchUrl = this.baseUrl + "api/test/";
          this.http.patch(patchUrl, { Id: this.test.Id }).subscribe(() => {},
            error => console.error(error));
        },
        error => console.error(error));
    } else {
      console.log("Invalid ID - redirecting to home...");
      this.router.navigate(["home"]);
    }
  }

  onEdit() {
    this.router.navigate(["test/edit", this.test.Id]);
  }

  onDelete() {

    if (confirm("Are you sure to remove this test ?")) {
      var url = this.baseUrl + "api/test/" + this.test.Id;

      this.http.delete(url).subscribe(result => {
          console.log("Test with ID: " + this.test.Id + " was removed.");
          this.router.navigate(["home"]);
        },
        error => console.error(error));
    }
  }

}
