import { Component, Inject, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { Validators, FormGroup, FormBuilder } from "@angular/forms";
import { Result } from 'src/app/interfaces/result';

@Component({
  selector: "result-edit",
  templateUrl: './result-edit.component.html',
  styleUrls: ['./result-edit.component.css']
})

export class ResultEditComponent {
  title: string;
  result: Result;
  form: FormGroup;

  editMode: boolean;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {

    this.result = <Result>{};

    var id = +this.activatedRoute.snapshot.params["id"];

    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");

    if (this.editMode) {

      var url = this.baseUrl + "api/result/" + id;

      this.http.get<Result>(url).subscribe(res => {
        this.result = res;
        this.title = "Editing - " + this.result.Text;
          this.updateForm();
        },
        error => console.error(error));
    } else {
      this.result.TestId = id;
      this.title = "Create new result";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required],
      MinValue: ['', Validators.pattern(/^\d*$/)],
      MaxValue: ['', Validators.pattern(/^\d*$/)]
    });
  }

  updateForm() {
    this.form.setValue({
      Text: this.result.Text,
      MinValue: this.result.MinValue,
      MaxValue: this.result.MaxValue,
    });
  }

  onSubmit() {

    var tempResult = <Result>{};
    tempResult.Text = this.form.value.Text;
    tempResult.MinValue = this.form.value.MinValue;
    tempResult.MaxValue = this.form.value.MaxValue;
    tempResult.TestId = this.result.TestId;

    var url = this.baseUrl + "api/result";

    if (this.editMode) {

      this.http.put<Result>(url, tempResult).subscribe(res => {
        var v = res;
          console.log("Result " + v.Id + " was updated");
        this.router.navigate(["test/edit", tempResult.TestId]);
        },
        error => console.log(error));
    } else {
      this.http.post<Result>(url, tempResult).subscribe(res => {
        var v = res;
        console.log("Result " + v.Id + " was created");
        this.router.navigate(["test/edit", tempResult.TestId]);
      });
    }
  }

  onBack() {
    this.router.navigate(["test/edit", this.result.TestId]);
  }

  getFormControl(name: string) {
    return this.form.get(name);
  }

  isValid(name: string) {
    var control = this.getFormControl(name);
    return control && control.valid;
  }

  isChanged(name: string) {
    var control = this.getFormControl(name);
    return control && (control.dirty || control.touched);
  }

  hasError(name: string) {
    var control = this.getFormControl(name);
    return control && (control.dirty || control.touched) && !control.valid;
  }
}
