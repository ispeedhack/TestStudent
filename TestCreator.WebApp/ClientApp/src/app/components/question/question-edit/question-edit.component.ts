import { Component, Inject, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { Validators, FormBuilder, FormGroup } from "@angular/forms";
import { Question } from 'src/app/interfaces/question';

@Component({
  selector: "question-edit",
  templateUrl: './question-edit.component.html',
  styleUrls: ['./question-edit.component.css']
})

export class QuestionEditComponent {
  title: string;
  question: Question;
  form: FormGroup;

  editMode: boolean;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {

    this.question = <Question>{};

    this.createForm();

    var id = +this.activatedRoute.snapshot.params["id"];

    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");

    if (this.editMode) {

      var url = this.baseUrl + "api/question/" + id;

      this.http.get<Question>(url).subscribe(result => {
          this.question = result;
        this.title = "Editing - " + this.question.Text;

          this.updateForm();
        },
        error => console.error(error));
    } else {
      this.question.TestId = id;
      this.title = "Create new question";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required]
    });
  }


  updateForm() {
    this.form.setValue({
      Text: this.question.Text || ''
    });
  }

  onSubmit() {

    var tempQuestion = <Question>{};
    tempQuestion.Text = this.form.value.Text;
    tempQuestion.TestId = this.form.value.TestId;

    var url = this.baseUrl + "api/question";

    if (this.editMode) {

      tempQuestion.Id = this.question.Id;

      this.http.put<Question>(url, tempQuestion).subscribe(result => {
          var v = result;
          console.log("Question " + v.Id + " was updated");
        this.router.navigate(["test/edit", tempQuestion.TestId]);
        },
        error => console.log(error));
    } else {
      this.http.post<Question>(url, tempQuestion).subscribe(result => {
        var v = result;
        console.log("Question " + v.Id + " was created");
        this.router.navigate(["test/edit", tempQuestion.TestId]);
      })
    }
  }

  onBack() {
    this.router.navigate(["test/edit", this.question.TestId]);
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
