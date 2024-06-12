import { Component, Inject } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { User } from 'src/app/interfaces/user';

@Component({
  selector: "register",
  templateUrl: "./register.component.html",
  styleUrls: ['./register.component.css']
})

export class RegisterComponent {
  title: string;
  form: FormGroup;

  constructor(private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject("BASE_URL") private baseUrl: string) {

    this.title = "Registration of new user";

    this.createForm();
  }

  createForm() {
    this.form = this.fb.group({
      Username: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', Validators.required],
      PasswordConfirm: ['', Validators.required],
      DisplayName: ['', Validators.required]
    },
      {
        validator: this.passwordConfirmValidator
      });
  }

  passwordConfirmValidator(control : FormControl) : any {
    let password = control.root.get('Password');
    let passwordConfirm = control.root.get('PasswordConfirm');

    if (password && passwordConfirm) {
      if (password.value != passwordConfirm.value) {
        password.setErrors({
          "PasswordMismatch": true
        });
      } else {
        password.setErrors(null);
      }
    }

  }

  onSubmit() {

    var tempUser = <User>{};
    tempUser.Username = this.form.value.Username;
    tempUser.Password = this.form.value.Password;
    tempUser.Email = this.form.value.Email;
    tempUser.DisplayName = this.form.value.DisplayName;

    var url = this.baseUrl + "api/user";

    this.http.post<User>(url, tempUser).subscribe(result => {
        if (result) {
          var res = result;
          console.log("User " + res.Username + " was successfully created");
          this.router.navigate(["login", { mode: "registration"}]);
        } else {
          this.form.setErrors({
            "register": "Registration failed"
          });
        }
      },
      error => console.log(error));

  }

  onBack() {
    this.router.navigate(["home"]);
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
