import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../shared/user.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { LoginRequest, LoginResponse } from '../shared/user.model';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent {
  loginFormSubmitted = false;
  isLoginFailed = false;
  errorMessage = '';

  loginForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    rememberMe: new FormControl(true)
  });

  constructor(private router: Router, private authService: UserService, private spinner: NgxSpinnerService) {}

  get lf() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.loginFormSubmitted = true;
    if (this.loginForm.invalid) {
      return;
    }

    this.spinner.show();

    const request: LoginRequest = {
      username: this.loginForm.value.username!,
      password: this.loginForm.value.password!
    };

    this.authService.login(request).subscribe(
      (res) => {
        console.log('Login response:', res);

        if (res.code === 200) {
          localStorage.setItem('token', res.data);
          this.router.navigate(['/dashboard/dashboard1']);
        } else {
          this.isLoginFailed = true;
          this.errorMessage = res.message;
        }
        this.spinner.hide();
      },
      (error) => {
        this.isLoginFailed = true;
        this.errorMessage = 'Đăng nhập thất bại. Vui lòng thử lại!';
        this.spinner.hide();
        console.error('Login failed:', error);
      }
    );
  }
}
