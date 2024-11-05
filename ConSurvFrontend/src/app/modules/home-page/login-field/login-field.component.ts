import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Configuration, UserService } from 'src/app/generated/continuous-surveillance-backend';

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

  constructor(httpClient: HttpClient, private router: Router, private route: ActivatedRoute) {
    this.userService = new UserService(httpClient, [], new Configuration());
  }

  login() {
    if (this.form.valid) {
      this.userService.aPIV1UserControllerLoginPut(this.usernameForm.value, this.passwordForm.value).subscribe(() => {
        this.router.navigate(['user'], { relativeTo: this.route });
      })
    }
  }
}
