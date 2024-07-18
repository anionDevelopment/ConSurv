import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { UserService } from 'src/app/generated/continuous-surveillance-backend';

@Component({
  selector: 'app-login-field',
  templateUrl: './login-field.component.html',
  styleUrls: ['./login-field.component.scss']
})
export class LoginFieldComponent {

  userService: UserService;
  usernameForm: FormControl = new FormControl('');
  passwordForm: FormControl = new FormControl('');
  form: FormGroup = new FormGroup({
    username: this.usernameForm,
    password: this.passwordForm,
  });

  constructor(httpClient: HttpClient) {
    this.userService = new UserService(httpClient);
  }

  login() {
    if (this.form.valid) {
      this.userService.putApiV0UserControllerLogin(this.usernameForm.value, this.passwordForm.value).subscribe(() => {
        console.log("logged in");
      })
    }
  }
}
