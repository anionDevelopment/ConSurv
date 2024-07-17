import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-login-field',
  templateUrl: './login-field.component.html',
  styleUrls: ['./login-field.component.scss']
})
export class LoginFieldComponent {

  form: FormGroup = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });

  login() {
    if (this.form.valid) {
      //TODO login
    }
  }
}
