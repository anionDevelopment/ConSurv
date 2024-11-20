import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { UserService } from '../../../generated/con-surv-backend';

@Component({
  selector: 'app-login-form',
  standalone: false,
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss'
})
export class LoginFormComponent {

  constructor(private userService: UserService) {
  }
  form: FormGroup = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });

  public login(): void {
    const username: string = this.form.get('username')!.value;
    console.log("username: " + username);
    const password: string = this.form.get('password')!.value;
    this.userService.aPIV1UserControllerLoginPut(username, password).subscribe(() => {
      //TODO redirect to /user
    });
  }
}
