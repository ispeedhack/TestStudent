import { Component, Inject, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Answer } from 'src/app/interfaces/answer';

@Component({
  selector: "answer-edit",
  templateUrl: './answer-edit.component.html',
  styleUrls: ['./answer-edit.component.css']
})

export class AnswerEditComponent {
  title: string;
  answer: Answer;
  form: FormGroup;

  editMode: boolean;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {

    this.answer = <Answer>{};

    this.createForm();

    var id = +this.activatedRoute.snapshot.params["id"];

    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");

    if (this.editMode) {

      var url = this.baseUrl + "api/answer/" + id;

      this.http.get<Answer>(url).subscribe(result => {
        this.answer = result;
        this.title = "Editing - " + this.answer.Text;

          this.updateForm();
        },
        error => console.error(error));
    } else {
      this.answer.QuestionId = id;
      this.title = "Create new answer";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required],
      Value: ['',
        [Validators.required,
          Validators.min(-5),
          Validators.max(5)]
      ],
    });
  }

  updateForm() {
    this.form.setValue({
      Text: this.answer.Text || '',
      Value: this.answer.Value || 0,
    });
  }


  onSubmit() {

    var tempAnswer = <Answer>{};
    tempAnswer.Text = this.form.value.Text;
    tempAnswer.Value = this.form.value.Value;
    tempAnswer.QuestionId = this.answer.QuestionId;

    var url = this.baseUrl + "api/answer";

    if (this.editMode) {
      tempAnswer.Id = this.answer.Id;
      this.http.put<Answer>(url, tempAnswer).subscribe(result => {
          var v = result;
        console.log("Answer " + v.Id + " was updated");
        this.router.navigate(["question/edit", tempAnswer.QuestionId]);
        },
        error => console.log(error));
    } else {
      this.http.post<Answer>(url, tempAnswer).subscribe(result => {
        var v = result;
        console.log("Answer " + v.Id + " was created");
        this.router.navigate(["question/edit", tempAnswer.QuestionId]);
      })
    }
  }

  onBack() {
    this.router.navigate(["question/edit", this.answer.QuestionId]);
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
