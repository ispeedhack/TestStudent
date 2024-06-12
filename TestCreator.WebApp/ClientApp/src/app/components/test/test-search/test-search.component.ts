import { Component, Input } from "@angular/core";
import { Router } from "@angular/router";
import { NgForm } from '@angular/forms';

@Component({
  selector: "test-search",
  templateUrl: "./test-search.component.html",
  styleUrls: ['./test-search.component.css']
})

export class TestSearchComponent {
  constructor(private router: Router) {  }

  onSubmit(form: NgForm) {
    let val = form.controls.searchedPhrase.value;
    if (val) {
      form.controls.searchedPhrase.setValue('Text...');
      this.router.navigate(["test/search", val]);
    }
    
  }
}
