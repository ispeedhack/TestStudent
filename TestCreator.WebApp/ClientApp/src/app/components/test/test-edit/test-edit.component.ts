import { Component, Inject, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { Validators, FormGroup, FormBuilder } from "@angular/forms";
import { Test } from 'src/app/interfaces/test';

@Component({
  selector: "test-edit",
  templateUrl: './test-edit.component.html',
  styleUrls: ['./test-edit.component.css']
})

export class TestEditComponent {
  title: string;
  test: Test;
  form: FormGroup;
  activityLog: string;

  editMode: boolean;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {

    this.test = <Test>{};

    this.createForm();

    var id = +this.activatedRoute.snapshot.params["id"];
    if (id) {
      this.editMode = true;

      var url = this.baseUrl + "api/test/" + id;
      this.http.get<Test>(url).subscribe(result => {
          this.test = result;
        this.title = "Editing - " + this.test.Title;

          this.updateForm();
        },
        error => console.error(error));

    } else {
      this.editMode = false;
      this.title = "Create new test";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Title: ['', Validators.required],
      Description: '',
      Text: ''
    });

    this.activityLog = '';
    this.log("Form was sucessfully initialized");

    this.form.valueChanges.subscribe(val => {
      if (!this.form.dirty) {
        this.log("Form model is loaded.");
      } else {
        this.log("Form was updated.");
      }
    });

    this.form.get("Text")!.valueChanges.subscribe(val => {
      if (!this.form.dirty) {
        this.log("Control \'Text\' loaded with default value.");
      } else {
        this.log("Control \'Text\' was changed by user.");
      }
    })
  }


  log(text: string) {
    this.activityLog += "[" + new Date().toLocaleString() + "]" + text + "<br />";
  }

  updateForm() {
    this.form.setValue({
      Title: this.test.Title,
      Description: this.test.Decription || '',
      Text: this.test.Text || ''
    });
  }

  onSubmit() {

    var tempTest = <Test>{};

    tempTest.Title = this.form.value.Title;
    tempTest.Decription = this.form.value.Decription;
    tempTest.Text = this.form.value.Text;

    var url = this.baseUrl + "api/test";

    if (this.editMode) {

      tempTest.Id = this.test.Id;

      this.http.put<Test>(url, tempTest).subscribe(result => {
          var res = result;
        console.log("Test " + res.Id + " was updated");
        this.router.navigate(["home"]);
        },
        error => console.error(error));
    } else {
      this.http.post<Test>(url, tempTest).subscribe(result => {
        var res = result;
        console.log("Test " + res.Id + " was created");
          this.router.navigate(["home"]);
        },
        error => console.error(error));
    }
  }

  onBack() {
    this.router.navigate(["home"]);
  }

  getFormControl(name : string) {
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
