import { Component, Inject, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { AuthService } from '../../services/auth.service';
import {  FormBuilder, FormGroup, Validators } from "@angular/forms";

@Component({
  selector: "login",
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent {
  title: string;
  form: FormGroup;


  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private authService: AuthService,
    @Inject('BASE_URL') private baseUrl: string) {

    var afterRegistration = this.activatedRoute.snapshot.params["mode"];

    if (afterRegistration && afterRegistration === "registration") {
      this.title = "Registration successful, now you can log in";
    } else {
      this.title = "Log in";
    }  

    this.createForm();

  }

  createForm() {
    this.form = this.fb.group({
      Username: ['', Validators.required],
      Password: ['', Validators.required]
    });
  }

  onSubmit() {

    var url = this.baseUrl + "api/token/auth";
    var username = this.form.value.Username;
    var password = this.form.value.Password;


    this.authService.login(username, password).subscribe(result => {
        //alert("Login successful - USERNAME: " + username + " TOKEN: " + this.authService.getAuth()!.token);

        this.router.navigate(["home"]);
      },
      error => {
        console.log(error);
        this.form.setErrors({ "auth": "Invalid username or password" });
      });

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
